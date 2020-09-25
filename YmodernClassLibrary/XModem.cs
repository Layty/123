using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FileTransmit
{

    public enum XModemCheckMode
    {
        CheckSum,
        CRC16
    }

    public enum XModemType
    {
        XModem,
        XModem_1K
    }

    /// <summary>
    /// XModem接收到的消息类型
    /// </summary>
    enum XModemMessageType : int
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
    /// XModem接收到的消息内容
    /// </summary>
    internal class XModemMessage
    {
        public XModemMessage(XModemMessageType type)
            : this(type, null)
        {

        }

        public XModemMessage(XModemMessageType type, object value)
        {
            MessageType = type;
            Value = value;
        }


        public XModemMessageType MessageType { get; }

        public object Value { get; }
    }

    /// <summary>
    /// XModem发送步骤
    /// </summary>
    internal enum XModemSendStage
    {
        WaitReceiveRequest,   //等待接收端请求
        PacketSending,
        WaitReceiveAnswerEndTransmit
    }

    /// <summary>
    /// XModem接收步骤
    /// </summary>
    internal enum XModemReceiveStage
    {
        WaitForFirstPacket,
        PacketReceiving,
    }

    public class XModemInfo
    {
        public XModemInfo()
            : this(XModemType.XModem, TransmitMode.Send, XModemCheckMode.CheckSum)
        {

        }

        public XModemInfo(XModemType type, TransmitMode transType, XModemCheckMode checkType)
        {
            Type = type;
            TransMode = transType;
            CheckMode = checkType;
        }


        public XModemType Type { get; set; }

        public TransmitMode TransMode { get; set; }

        public XModemCheckMode CheckMode { get; set; }
    }

    public class XModem : IFileTransmit, ITransmitUart
    {
        /// <summary>
        /// 数据块起始字符
        /// SOH也是ASCII码的一个控制字符的名称（Start of Heading），取值为十六进制的0x01，
        /// 通常表示成。也常作为分隔符用在字符通讯报文中，例如FIX协议（金融信息交换协议）中字段之间就是用SOH做分隔符
        /// </summary>
        private readonly byte SOH = 0x01;
        /// <summary>
        /// 1024字节开始
        /// </summary>
        private readonly byte STX = 0x02;
        /// <summary>
        /// 文件传输结束
        /// </summary>
        private readonly byte EOT = 0x04;
        /// <summary>
        /// 确认应答
        /// </summary>
        private readonly byte ACK = 0x06;
        /// <summary>
        /// 出现错误
        /// </summary>
        private readonly byte NAK = 0x15;
        /// <summary>
        /// 取消传输
        /// </summary>
        private readonly byte CAN = 0x18;
        /// <summary>
        /// 大写字母C
        /// </summary>
        private readonly byte KEY_C = 0x43;

        private int RetryMax = 6;
        XModemInfo xmodemInfo = new XModemInfo();

        public bool IsStart { get; private set; }
        private int reTryCount;
        private ManualResetEvent waitReceiveEvent = new ManualResetEvent(false);

        private XModemReceiveStage ReceiveStage;
        private XModemSendStage SendStage;
        private Queue<XModemMessage> msgQueue = new Queue<XModemMessage>();

        public XModem(TransmitMode transType, XModemType xModemType, int reTryCount)
        {
            RetryMax = reTryCount;

            xmodemInfo.CheckMode = XModemCheckMode.CheckSum;
            xmodemInfo.Type = xModemType;
            xmodemInfo.TransMode = transType;
        }

        public void Start()
        {
            IsStart = true;
            reTryCount = 0;

            ReceiveStage = XModemReceiveStage.WaitForFirstPacket;
            SendStage = XModemSendStage.WaitReceiveRequest;
            msgQueue.Clear();

            Task.Run(TransThreadHandler);
            if (xmodemInfo.TransMode == TransmitMode.Receive)
            {
                StartReceive?.Invoke(xmodemInfo, null);
            }
        }

        public void Stop()
        {
            if (xmodemInfo.TransMode == TransmitMode.Receive)
            {
                Abort();
            }
            else
            {
                SendEOT();
            }

            EndOfTransmit?.Invoke(xmodemInfo, null);
        }

        public void Abort()
        {
            IsStart = false;
            SendCAN();

            EndOfTransmit?.Invoke(xmodemInfo, null);
        }


        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="data"></param>
        private void ParseReceivedMessage(byte[] data)
        {
            XModemMessage receivedMessage;

            if (data == null)
            {
                receivedMessage = null;
            }
            else
            {
                if (data[0] == STX || data[0] == SOH)
                {
                    receivedMessage = new XModemMessage(XModemMessageType.PACKET_ERROR);
                    int packetLen = 0;
                    if (data[0] == STX)
                    {
                        packetLen = 1024;
                    }
                    else if (data[0] == SOH)
                    {
                        packetLen = 128;
                    }

                    int checkDataLen = xmodemInfo.CheckMode == XModemCheckMode.CheckSum ? 1 : 2;
                    if (packetLen + 3 + checkDataLen == data.Length)
                    {
                        int packetNo = 0;
                        if (data[1] == Convert.ToByte((~data[2]) & 0xFF))
                        {
                            packetNo = data[1];
                        }

                        byte[] packet = new byte[packetLen];

                        Array.Copy(data, 3, packet, 0, packetLen);

                        int frameCheckCode;
                        var calCheckCode = -1;
                        if (xmodemInfo.CheckMode == XModemCheckMode.CheckSum)
                        {
                            frameCheckCode = data[3 + packetLen];
                            calCheckCode = Convert.ToByte(DataCheck.GetCheckSum(packet) & 0xFF);
                        }
                        else
                        {
                            frameCheckCode = (data[3 + packetLen] << 8) + data[3 + packetLen + 1];
                            calCheckCode = Convert.ToUInt16(DataCheck.GetCRC(CRCType.CRC16_XMODEM, packet) & 0xFFFF);
                        }

                        if (frameCheckCode == calCheckCode)
                        {
                            receivedMessage = new XModemMessage(XModemMessageType.PACKET, new PacketEventArgs(packetNo, packet));
                        }

                    }
                    msgQueue.Enqueue(receivedMessage);
                }
                else
                {
                    foreach (byte b in data)
                    {
                        receivedMessage = null;
                        if (b == EOT)
                        {
                            receivedMessage = new XModemMessage(XModemMessageType.EOT);
                        }
                        else if (b == CAN)
                        {
                            receivedMessage = new XModemMessage(XModemMessageType.CAN);
                        }
                        else if (b == NAK)
                        {
                            receivedMessage = new XModemMessage(XModemMessageType.NAK);
                        }
                        else if (b == ACK)
                        {
                            receivedMessage = new XModemMessage(XModemMessageType.ACK);
                        }
                        else if (b == KEY_C)
                        {
                            receivedMessage = new XModemMessage(XModemMessageType.KEY_C);
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
            SendToUartEvent?.Invoke(xmodemInfo, new SendToUartEventArgs(data));
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
            SendStage = XModemSendStage.WaitReceiveAnswerEndTransmit;
        }




        void TransThreadHandler()
        {
            while (IsStart)
            {
                if (xmodemInfo.TransMode == TransmitMode.Send)
                {
                    SendHandler();
                }
                else if (xmodemInfo.TransMode == TransmitMode.Receive)
                {
                    ReceiveHandler();
                }
            }
        }

        void SendHandler()
        {
            XModemMessage msg = null;
            lock (msgQueue)
            {
                if (msgQueue.Count > 0)
                {
                    msg = msgQueue.Dequeue();
                }
            }

            if (msg != null)
            {
                reTryCount = 0;

                switch (msg.MessageType)
                {
                    case XModemMessageType.NAK:
                        if (SendStage == XModemSendStage.WaitReceiveRequest)
                        {
                            SendStage = XModemSendStage.PacketSending;

                            xmodemInfo.CheckMode = XModemCheckMode.CheckSum;
                            StartSend?.Invoke(xmodemInfo, null);
                        }
                        else if (SendStage == XModemSendStage.WaitReceiveAnswerEndTransmit)
                        {
                            SendEOT();
                        }
                        else
                        {
                            // 通知重发或发头一包
                            ReSendPacket?.Invoke(xmodemInfo, null);
                        }
                        break;

                    case XModemMessageType.KEY_C:
                        if (SendStage == XModemSendStage.WaitReceiveRequest)
                        {
                            SendStage = XModemSendStage.PacketSending;
                            // 通知发头一包CRC
                            xmodemInfo.CheckMode = XModemCheckMode.CRC16;
                            StartSend?.Invoke(xmodemInfo, null);
                        }
                        break;

                    case XModemMessageType.ACK:
                        if (SendStage == XModemSendStage.PacketSending)
                        {
                            // 通知发下一包
                            SendNextPacket?.Invoke(xmodemInfo, null);
                        }
                        else if (SendStage == XModemSendStage.WaitReceiveAnswerEndTransmit)
                        {
                            // 通知中止
                            //if (AbortTransmit != null)
                            //{
                            //    AbortTransmit(xmodemInfo, null);
                            //}
                            EndOfTransmit?.Invoke(xmodemInfo, null);
                            IsStart = false;
                        }
                        break;

                    case XModemMessageType.CAN:
                        // 通知中止
                        AbortTransmit?.Invoke(xmodemInfo, null);
                        break;

                    default:
                        break;
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
                        TransmitTimeOut?.Invoke(xmodemInfo, null);
                    }
                }
            }

        }

        void ReceiveHandler()
        {
            if (ReceiveStage == XModemReceiveStage.WaitForFirstPacket)
            {
                if (reTryCount % 2 == 0)
                {
                    xmodemInfo.CheckMode = XModemCheckMode.CheckSum;
                    SendKEYC();
                }
                else
                {
                    xmodemInfo.CheckMode = XModemCheckMode.CRC16;
                    SendNAK();
                }
            }


            XModemMessage msg = null;
            lock (msgQueue)
            {
                if (msgQueue.Count > 0)
                {
                    msg = msgQueue.Dequeue();
                }

            }
            if (msg != null)
            {
                reTryCount = 0;

                switch (msg.MessageType)
                {
                    case XModemMessageType.PACKET:
                        ReceiveStage = XModemReceiveStage.PacketReceiving;
                        SendACK();
                        if (ReceivedPacket != null)
                        {
                            PacketEventArgs e = msg.Value as PacketEventArgs;
                            ReceivedPacket(xmodemInfo, new PacketEventArgs(e.PacketNo, e.Packet));
                        }

                        // 通知发下一包
                        SendNextPacket?.Invoke(xmodemInfo, null);
                        break;
                    case XModemMessageType.PACKET_ERROR:
                        SendNAK();
                        // 通知重发
                        ReSendPacket?.Invoke(xmodemInfo, null);
                        break;
                    case XModemMessageType.EOT:
                        SendACK();
                        // 通知完成
                        EndOfTransmit?.Invoke(xmodemInfo, null);
                        break;
                    case XModemMessageType.CAN:
                        SendACK();
                        // 通知中止
                        AbortTransmit?.Invoke(xmodemInfo, null);
                        break;
                    default:
                        break;
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
                        TransmitTimeOut?.Invoke(xmodemInfo, null);
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
            int checkLen = xmodemInfo.CheckMode == XModemCheckMode.CheckSum ? 1 : 2;

            int packetLen = xmodemInfo.Type == XModemType.XModem_1K ? 1024 : 128;

            var data = new byte[3 + packetLen + checkLen];

            data[0] = SOH;
            if (xmodemInfo.Type == XModemType.XModem_1K)
            {
                data[0] = STX;
            }

            data[1] = Convert.ToByte(packet.PacketNo & 0xFF);
            data[2] = Convert.ToByte((~data[1]) & 0xFF);
            Array.Copy(packet.Packet, 0, data, 3, packetLen);

            if (xmodemInfo.CheckMode == XModemCheckMode.CheckSum)
            {
                data[3 + packetLen] = Convert.ToByte(DataCheck.GetCheckSum(packet.Packet) & 0xFF);
            }
            else
            {
                ushort crc = Convert.ToUInt16(DataCheck.GetCRC(CRCType.CRC16_XMODEM, packet.Packet) & 0xFFFF);
                data[3 + packetLen] = Convert.ToByte(crc >> 8);
                data[3 + packetLen + 1] = Convert.ToByte(crc & 0xFF);
            }

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
