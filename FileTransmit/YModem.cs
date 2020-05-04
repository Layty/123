using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace 三相智慧能源网关调试软件.FileTransmit
{
    public enum YModemCheckMode
    {
        CheckSum,
        CRC16
    }

    public enum YModemType
    {
        YModem,
        YModem_1K
    }

    /// <summary>
    /// YModem接收到的消息类型
    /// </summary>
    enum YmodemMessageType : int
    {
        KEY_C,
        ACK,
        NAK,
        EOT,
        PACKET,
        PACKET_ERROR,
        CAN
    }

    /// <summary>
    /// YModem接收到的消息内容
    /// </summary>
    internal class YmodemMessage
    {
        private YmodemMessageType _MessageType;
        private object _Value;


        public YmodemMessage(YmodemMessageType type)
            : this(type, null)
        {
        }

        public YmodemMessage(YmodemMessageType type, object value)
        {
            _MessageType = type;
            _Value = value;
        }


        public YmodemMessageType MessageType
        {
            get { return _MessageType; }
        }

        public object Value
        {
            get { return _Value; }
        }
    }

    /// <summary>
    /// Xmodem发送步骤
    /// </summary>
    internal enum YmodemSendStage : int
    {
        WaitReceiveRequestFileInfo, //等待接收端请求文件头
        WaitReceiveRequestFirstPacket,
        PacketSending,
        WaitReceiveAnswerEndTransmit, // 等待接收方应答EOT
        WaitReceiveNextFileReq, // 等等接收方请求下一个文件
    }

    /// <summary>
    /// Xmodem接收步骤
    /// </summary>
    internal enum YmodemReceiveStage : int
    {
        WaitForFileInfo,
        WaitForFirstPacket,
        PacketReceiving,
    }

    public class YModemInfo
    {
        public YModemInfo()
            : this(YModemType.YModem, TransmitMode.Send)
        {
        }

        public YModemInfo(YModemType type, TransmitMode transType)
        {
            Type = type;
            TransMode = transType;
        }


        public YModemType Type { get; set; }

        public TransmitMode TransMode { get; set; }

        //public YModemCheckMode CheckMode
        //{
        //    get { return _CheckMode; }
        //    set { _CheckMode = value; }
        //}
    }

    public class YModem : ObservableObject,IFileTransmit, ITransmitUart 
    {
        /// <summary>
        /// SOH也是ASCII码的一个控制字符的名称（Start of Heading），取值为十六进制的0x01，
        /// 通常表示成。也常作为分隔符用在字符通讯报文中，例如FIX协议（金融信息交换协议）中字段之间就是用SOH做分隔符
        /// </summary>
        private readonly byte SOH = 0x01;//数据块起始字符

        private readonly byte STX = 0x02;//1028字节开始

        private readonly byte EOT = 0x04; //文件传输结束

        private readonly byte ACK = 0x06;//确认应答

        private readonly byte NAK = 0x15;//出现错误
        private readonly byte CAN = 0x18;//取消传输
        private readonly byte KEY_C = 0x43; //大写字母C

        private int RetryMax;

        YModemInfo ymodemInfo = new YModemInfo();

        public bool IsStart
        {
            get => _isStart;
            set { _isStart = value;RaisePropertyChanged(); }
        }
        private bool _isStart;


        private int reTryCount;
        private ManualResetEvent waitReceiveEvent = new ManualResetEvent(false);
        private YmodemReceiveStage ReceiveStage;
        private YmodemSendStage SendStage;
        private Queue<YmodemMessage> msgQueue = new Queue<YmodemMessage>();

        public YModem(TransmitMode transType, YModemType ymodemType, int reTryCount)
        {
            RetryMax = reTryCount;
            ymodemInfo.Type = ymodemType;
            ymodemInfo.TransMode = transType;
        }

        public void Start()
        {
            IsStart = true;
            reTryCount = 0;
            ReceiveStage = YmodemReceiveStage.WaitForFileInfo;
            SendStage = YmodemSendStage.WaitReceiveRequestFileInfo;
            Task.Run(TransThreadHandler);
            if (ymodemInfo.TransMode == TransmitMode.Receive)
            {
                StartReceive?.Invoke(ymodemInfo, null);
            }
        }

        public void Stop()
        {
            if (ymodemInfo.TransMode == TransmitMode.Receive)
            {
                Abort();
            }
            else
            {
                SendEOT();
            }
        }

        public void Abort()
        {
            IsStart = false;
            SendCAN();

            EndOfTransmit?.Invoke(ymodemInfo, null);
        }


        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="data"></param>
        private void ParseReceivedMessage(byte[] data)
        {
            YmodemMessage receivedMessage;

            if (data == null)
            {
                receivedMessage = null;
            }
            else
            {
                if (data[0] == STX || data[0] == SOH) //等于SOH
                {
                    receivedMessage = new YmodemMessage(YmodemMessageType.PACKET_ERROR);
                    int packetLen = 0;
                    if (data[0] == STX)
                    {
                        packetLen = 1024;
                    }
                    else if (data[0] == SOH)
                    {
                        packetLen = 128;
                    }

                    int checkDataLen = 2;
                    if (packetLen + 3 + checkDataLen == data.Length)
                    {
                        int packetNo = 0;
                        if (data[1] == Convert.ToByte((~data[2]) & 0xFF))
                        {
                            packetNo = data[1];
                        }

                        int frameCheckCode = 0;
                        int calCheckCode = -1;
                        byte[] packet = new byte[packetLen];

                        Array.Copy(data, 3, packet, 0, packetLen);

                        frameCheckCode = (data[3 + packetLen] << 8) + data[3 + packetLen + 1];
                        calCheckCode = Convert.ToUInt16(DataCheck.GetCRC(CRCType.CRC16_XMODEM, packet) & 0xFFFF);


                        if (frameCheckCode == calCheckCode)
                        {
                            receivedMessage = new YmodemMessage(YmodemMessageType.PACKET,
                                new PacketEventArgs(packetNo, packet));
                        }
                    }
                }
                else
                {
                    foreach (byte b in data)
                    {
                        receivedMessage = null;

                        if (b == EOT)
                        {
                            receivedMessage = new YmodemMessage(YmodemMessageType.EOT);
                        }
                        else if (b == CAN)
                        {
                            receivedMessage = new YmodemMessage(YmodemMessageType.CAN);
                        }
                        else if (b == NAK)
                        {
                            receivedMessage = new YmodemMessage(YmodemMessageType.NAK);
                        }
                        else if (b == ACK)
                        {
                            receivedMessage = new YmodemMessage(YmodemMessageType.ACK);
                        }
                        else if (b == KEY_C)
                        {
                            receivedMessage = new YmodemMessage(YmodemMessageType.KEY_C);
                        }
                        else
                        {
                        }

                        if (receivedMessage != null)
                        {
                            msgQueue.Enqueue(receivedMessage);
                        }
                    }
                }
            }

            waitReceiveEvent.Set();
        }


        private void SendFrameToUart(byte data)
        {
            byte[] bytes = new byte[1];
            bytes[0] = data;
            SendFrameToUart(bytes);
        }

        private void SendFrameToUart(byte[] data)
        {
            SendToUartEvent?.Invoke(ymodemInfo, new SendToUartEventArgs(data));
        }


        private void SendACK()
        {
            SendFrameToUart(ACK);
        }

        private void SendNAK()
        {
            SendFrameToUart(NAK);
        }

        private void SendKEYC()
        {
            SendFrameToUart(KEY_C);
        }

        private void SendCAN()
        {
            byte[] bytes = new byte[5];
            for (int i = 0; i < 5; i++)
            {
                bytes[i] = CAN;
            }

            SendFrameToUart(bytes);
        }

        private void SendEOT()
        {
            SendFrameToUart(EOT);
            SendStage = YmodemSendStage.WaitReceiveAnswerEndTransmit;
        }


        private void SendNoFilesToSend()
        {
            //int packetLen = ymodemInfo.Type == YModemType.YModem ? 128 : 1024;
            byte[] endPacket = new byte[3 + 128 + 2];
            endPacket[0] = 0x01;
            endPacket[1] = 0x00;
            endPacket[2] = 0xFF;
            SendFrameToUart(endPacket);

            EndOfTransmit?.Invoke(ymodemInfo, null);

            IsStart = false;
        }

        void TransThreadHandler()
        {
            while (IsStart)
            {
                if (ymodemInfo.TransMode == TransmitMode.Send)
                {
                    SendHandler();
                }
                else if (ymodemInfo.TransMode == TransmitMode.Receive)
                {
                    ReceiveHandler();
                }
            }
        }

        void SendHandler()
        {
            YmodemMessage msg;
            if (msgQueue.Count > 0)
            {
                msg = msgQueue.Dequeue();
                if (msg != null)
                {
                    reTryCount = 0;
                    switch (msg.MessageType)
                    {
                        case YmodemMessageType.NAK:
                            if (SendStage == YmodemSendStage.WaitReceiveAnswerEndTransmit)
                            {
                                SendEOT();
                            }
                            else
                            {
                                // 通知重发
                                ReSendPacket?.Invoke(ymodemInfo, null);
                            }

                            break;
                        case YmodemMessageType.KEY_C:
                            if (SendStage == YmodemSendStage.WaitReceiveRequestFileInfo)
                            {
                                // 通知发头一包CRC
                                StartSend?.Invoke(ymodemInfo, null);
                            }
                            else if (SendStage == YmodemSendStage.WaitReceiveRequestFirstPacket) //等待第一包
                            {
                                SendStage = YmodemSendStage.PacketSending;
                                // 通知发下一包
                                SendNextPacket?.Invoke(ymodemInfo, null);
                            }
                            else if (SendStage == YmodemSendStage.WaitReceiveNextFileReq) //接收方请求下一个文件
                            {
                                SendNoFilesToSend();
                            }


                            break;
                        case YmodemMessageType.ACK:
                            if (SendStage == YmodemSendStage.WaitReceiveRequestFileInfo)
                            {
                                SendStage = YmodemSendStage.WaitReceiveRequestFirstPacket; //等待接收方请求第一包数据
                            }
                            else if (SendStage == YmodemSendStage.PacketSending)
                            {
                                // 通知发下一包
                                SendNextPacket?.Invoke(ymodemInfo, null);
                            }
                            else if (SendStage == YmodemSendStage.WaitReceiveAnswerEndTransmit)
                            {
                                SendStage = YmodemSendStage.WaitReceiveNextFileReq; //等待接收方请求下一个文件
                            }

                            break;
                        case YmodemMessageType.CAN:
                            // 通知中止
                            AbortTransmit?.Invoke(ymodemInfo, null);

                            break;
                        default:

                            break;
                    }
                }
            }
            else
            {
                if (waitReceiveEvent.WaitOne(3000))
                {
                    waitReceiveEvent.Reset();
                }
                else
                {
                    reTryCount++;
                    if (reTryCount > RetryMax)
                    {
                        IsStart = false;
                        //通知接收超时
                        TransmitTimeOut?.Invoke(ymodemInfo, null);
                    }
                }
            }
        }

        void ReceiveHandler()
        {
            if (ReceiveStage == YmodemReceiveStage.WaitForFileInfo ||
                ReceiveStage == YmodemReceiveStage.WaitForFirstPacket)
            {
                SendKEYC();
            }

            if (msgQueue.Count > 0)
            {
                YmodemMessage msg = msgQueue.Dequeue();
                if (msg != null)
                {
                    reTryCount = 0;

                    switch (msg.MessageType)
                    {
                        case YmodemMessageType.PACKET:

                            PacketEventArgs e = msg.Value as PacketEventArgs;
                            if (ReceiveStage == YmodemReceiveStage.WaitForFileInfo)
                            {
                                if (e.PacketNo == 0)
                                {
                                    ReceiveStage = YmodemReceiveStage.WaitForFirstPacket;
                                    SendACK();

                                    ReceivedPacket?.Invoke(ymodemInfo, new PacketEventArgs(e.PacketNo, e.Packet));
                                }

                                //else
                                //{
                                //    SendNAK();
                                //}
                            }

                            else if (ReceiveStage == YmodemReceiveStage.WaitForFirstPacket ||
                                     ReceiveStage == YmodemReceiveStage.PacketReceiving)
                            {
                                if (ReceiveStage == YmodemReceiveStage.WaitForFirstPacket)
                                {
                                    ReceiveStage = YmodemReceiveStage.PacketReceiving;
                                }

                                SendACK();

                                ReceivedPacket?.Invoke(ymodemInfo, new PacketEventArgs(e.PacketNo, e.Packet));

                                // 通知发下一包
                                SendNextPacket?.Invoke(ymodemInfo, null);
                            }


                            break;
                        case YmodemMessageType.PACKET_ERROR:
                            SendNAK();
                            // 通知重发
                            ReSendPacket?.Invoke(ymodemInfo, null);

                            break;
                        case YmodemMessageType.EOT:
                            SendACK();
                            // 通知完成
                            EndOfTransmit?.Invoke(ymodemInfo, null);

                            break;
                        case YmodemMessageType.CAN:
                            SendACK();
                            // 通知中止
                            AbortTransmit?.Invoke(ymodemInfo, null);

                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                if (waitReceiveEvent.WaitOne(3000))
                {
                    waitReceiveEvent.Reset();
                }
                else
                {
                    reTryCount++;
                    if (reTryCount > RetryMax)
                    {
                        IsStart = false;
                        //通知接收超时
                        TransmitTimeOut?.Invoke(ymodemInfo, null);
                    }
                }
            }
        }


        #region IFileTransmit 成员

        public event EventHandler StartSend;

        public event EventHandler StartReceive;

        public event EventHandler SendNextPacket;

        public event EventHandler ReSendPacket;

        public event EventHandler AbortTransmit;

        public event EventHandler EndOfTransmit;

        public event EventHandler TransmitTimeOut;

        public event PacketEventHandler ReceivedPacket;

        public void SendPacket(PacketEventArgs packet)
        {
            int packetLen = 0;
            int checkLen = 0;
            byte[] data;

            checkLen = 2;

            if (ymodemInfo.Type == YModemType.YModem_1K)
            {
                packetLen = 1024;
            }
            else
            {
                packetLen = 128;
            }

            data = new byte[3 + packetLen + checkLen];

            data[0] = SOH;
            if (ymodemInfo.Type == YModemType.YModem_1K)
            {
                data[0] = STX;
            }

            data[1] = Convert.ToByte(packet.PacketNo & 0xFF);
            data[2] = Convert.ToByte((~data[1]) & 0xFF);
            Array.Copy(packet.Packet, 0, data, 3, packetLen);

            UInt16 crc = Convert.ToUInt16(DataCheck.GetCRC(CRCType.CRC16_XMODEM, packet.Packet) & 0xFFFF);
            data[3 + packetLen] = Convert.ToByte(crc >> 8);
            data[3 + packetLen + 1] = Convert.ToByte(crc & 0xFF);


            SendFrameToUart(data);
        }

        #endregion


        #region ITransmitUart 成员

        public event SendToUartEventHandler SendToUartEvent;

        public void ReceivedFromUart(byte[] data)
        {
            ParseReceivedMessage(data);
        }

        #endregion
    }
}