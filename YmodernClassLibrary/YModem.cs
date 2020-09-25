using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace FileTransmit
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
    /// YModem���յ�����Ϣ����
    /// </summary>
    enum YModemMessageType
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
    /// YModem���յ�����Ϣ����
    /// </summary>
    internal class YModemMessage
    {
        public YModemMessage(YModemMessageType type)
            : this(type, null)
        {
        }

        public YModemMessage(YModemMessageType type, object value)
        {
            MessageType = type;
            Value = value;
        }


        public YModemMessageType MessageType { get; }

        public object Value { get; }
    }

    /// <summary>
    /// YModem���Ͳ���
    /// </summary>
    internal enum YModemSendStage
    {
        WaitReceiveRequestFileInfo, //�ȴ����ն������ļ�ͷ
        WaitReceiveRequestFirstPacket,
        PacketSending,
        WaitReceiveAnswerEndTransmit, // �ȴ����շ�Ӧ��EOT
        WaitReceiveNextFileReq, // �ȵȽ��շ�������һ���ļ�
    }

    /// <summary>
    /// YModem���ղ���
    /// </summary>
    internal enum YModemReceiveStage
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
    }

    public class YModem : IFileTransmit, ITransmitUart, INotifyPropertyChanged
    {
        /// <summary>
        /// ���ݿ���ʼ�ַ�
        /// SOHҲ��ASCII���һ�������ַ������ƣ�Start of Heading����ȡֵΪʮ�����Ƶ�0x01��
        /// ͨ����ʾ�ɡ�Ҳ����Ϊ�ָ��������ַ�ͨѶ�����У�����FIXЭ�飨������Ϣ����Э�飩���ֶ�֮�������SOH���ָ���
        /// </summary>
        private readonly byte SOH = 0x01;
        /// <summary>
        /// 1024�ֽڿ�ʼ
        /// </summary>
        private readonly byte STX = 0x02;
        /// <summary>
        /// �ļ��������
        /// </summary>
        private readonly byte EOT = 0x04;
        /// <summary>
        /// ȷ��Ӧ��
        /// </summary>
        private readonly byte ACK = 0x06;
        /// <summary>
        /// ���ִ���
        /// </summary>
        private readonly byte NAK = 0x15;
        /// <summary>
        /// ȡ������
        /// </summary>
        private readonly byte CAN = 0x18;
        /// <summary>
        /// ��д��ĸC
        /// </summary>
        private readonly byte KEY_C = 0x43; 

        private int RetryMax;

        YModemInfo yModemInfo = new YModemInfo();

        public bool IsStart
        {
            get => _isStart;
            private set
            {
                _isStart = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsStart"));
            }
        }

        private bool _isStart;


        private int reTryCount;
        private ManualResetEvent waitReceiveEvent = new ManualResetEvent(false);
        private YModemReceiveStage ReceiveStage;
        private YModemSendStage SendStage;
        private Queue<YModemMessage> msgQueue = new Queue<YModemMessage>();

        public YModem(TransmitMode transType, YModemType yModemType, int reTryCount)
        {
            RetryMax = reTryCount;
            yModemInfo.Type = yModemType;
            yModemInfo.TransMode = transType;
        }

        public void Start()
        {
            IsStart = true;
            reTryCount = 0;
            ReceiveStage = YModemReceiveStage.WaitForFileInfo;
            SendStage = YModemSendStage.WaitReceiveRequestFileInfo;
            Task.Run(TransThreadHandler);
            if (yModemInfo.TransMode == TransmitMode.Receive)
            {
                StartReceive?.Invoke(yModemInfo, null);
            }
        }

        public void Stop()
        {
            if (yModemInfo.TransMode == TransmitMode.Receive)
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

            EndOfTransmit?.Invoke(yModemInfo, null);
        }


        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="data"></param>
        private void ParseReceivedMessage(byte[] data)
        {
            YModemMessage receivedMessage;

            if (data == null)
            {
                receivedMessage = null;
            }
            else
            {
                if (data[0] == STX || data[0] == SOH) //����SOH
                {
                    receivedMessage = new YModemMessage(YModemMessageType.PACKET_ERROR);
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

                        int calCheckCode = -1;
                        byte[] packet = new byte[packetLen];

                        Array.Copy(data, 3, packet, 0, packetLen);

                        var frameCheckCode = (data[3 + packetLen] << 8) + data[3 + packetLen + 1];
                        calCheckCode = Convert.ToUInt16(DataCheck.GetCRC(CRCType.CRC16_XMODEM, packet) & 0xFFFF);


                        if (frameCheckCode == calCheckCode)
                        {
                            receivedMessage = new YModemMessage(YModemMessageType.PACKET,
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
                            receivedMessage = new YModemMessage(YModemMessageType.EOT);
                        }
                        else if (b == CAN)
                        {
                            receivedMessage = new YModemMessage(YModemMessageType.CAN);
                        }
                        else if (b == NAK)
                        {
                            receivedMessage = new YModemMessage(YModemMessageType.NAK);
                        }
                        else if (b == ACK)
                        {
                            receivedMessage = new YModemMessage(YModemMessageType.ACK);
                        }
                        else if (b == KEY_C)
                        {
                            receivedMessage = new YModemMessage(YModemMessageType.KEY_C);
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
            SendToUartEvent?.Invoke(yModemInfo, new SendToUartEventArgs(data));
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
            SendStage = YModemSendStage.WaitReceiveAnswerEndTransmit;
        }


        private void SendNoFilesToSend()
        {
            //int packetLen = yModemInfo.Type == YModemType.FileTransProtocol ? 128 : 1024;
            byte[] endPacket = new byte[3 + 128 + 2];
            endPacket[0] = 0x01;
            endPacket[1] = 0x00;
            endPacket[2] = 0xFF;
            SendFrameToUart(endPacket);

            EndOfTransmit?.Invoke(yModemInfo, null);

            IsStart = false;
        }

        private void TransThreadHandler()
        {
            while (IsStart)
            {
                if (yModemInfo.TransMode == TransmitMode.Send)
                {
                    SendHandler();
                }
                else if (yModemInfo.TransMode == TransmitMode.Receive)
                {
                    ReceiveHandler();
                }
            }
        }

        private void SendHandler()
        {
            YModemMessage msg;
            if (msgQueue.Count > 0)
            {
                msg = msgQueue.Dequeue();
                if (msg != null)
                {
                    reTryCount = 0;
                    switch (msg.MessageType)
                    {
                        case YModemMessageType.NAK:
                            if (SendStage == YModemSendStage.WaitReceiveAnswerEndTransmit)
                            {
                                SendEOT();
                            }
                            else
                            {
                                // ֪ͨ�ط�
                                ReSendPacket?.Invoke(yModemInfo, null);
                            }

                            break;
                        case YModemMessageType.KEY_C:
                            if (SendStage == YModemSendStage.WaitReceiveRequestFileInfo)
                            {
                                // ֪ͨ��ͷһ��CRC
                                StartSend?.Invoke(yModemInfo, null);
                            }
                            else if (SendStage == YModemSendStage.WaitReceiveRequestFirstPacket) //�ȴ���һ��
                            {
                                SendStage = YModemSendStage.PacketSending;
                                // ֪ͨ����һ��
                                SendNextPacket?.Invoke(yModemInfo, null);
                            }
                            else if (SendStage == YModemSendStage.WaitReceiveNextFileReq) //���շ�������һ���ļ�
                            {
                                SendNoFilesToSend();
                            }


                            break;
                        case YModemMessageType.ACK:
                            if (SendStage == YModemSendStage.WaitReceiveRequestFileInfo)
                            {
                                SendStage = YModemSendStage.WaitReceiveRequestFirstPacket; //�ȴ����շ������һ������
                            }
                            else if (SendStage == YModemSendStage.PacketSending)
                            {
                                // ֪ͨ����һ��
                                SendNextPacket?.Invoke(yModemInfo, null);
                            }
                            else if (SendStage == YModemSendStage.WaitReceiveAnswerEndTransmit)
                            {
                                SendStage = YModemSendStage.WaitReceiveNextFileReq; //�ȴ����շ�������һ���ļ�
                            }

                            break;
                        case YModemMessageType.CAN:
                            // ֪ͨ��ֹ
                            AbortTransmit?.Invoke(yModemInfo, null);

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
                        //֪ͨ���ճ�ʱ
                        TransmitTimeOut?.Invoke(yModemInfo, null);
                    }
                }
            }
        }

        private void ReceiveHandler()
        {
            if (ReceiveStage == YModemReceiveStage.WaitForFileInfo ||
                ReceiveStage == YModemReceiveStage.WaitForFirstPacket)
            {
                SendKEYC();
            }

            if (msgQueue.Count > 0)
            {
                YModemMessage msg = msgQueue.Dequeue();
                if (msg != null)
                {
                    reTryCount = 0;

                    switch (msg.MessageType)
                    {
                        case YModemMessageType.PACKET:

                            PacketEventArgs e = msg.Value as PacketEventArgs;
                            if (ReceiveStage == YModemReceiveStage.WaitForFileInfo)
                            {
                                if (e.PacketNo == 0)
                                {
                                    ReceiveStage = YModemReceiveStage.WaitForFirstPacket;
                                    SendACK();

                                    ReceivedPacket?.Invoke(yModemInfo, new PacketEventArgs(e.PacketNo, e.Packet));
                                }

                                //else
                                //{
                                //    SendNAK();
                                //}
                            }

                            else if (ReceiveStage == YModemReceiveStage.WaitForFirstPacket ||
                                     ReceiveStage == YModemReceiveStage.PacketReceiving)
                            {
                                if (ReceiveStage == YModemReceiveStage.WaitForFirstPacket)
                                {
                                    ReceiveStage = YModemReceiveStage.PacketReceiving;
                                }

                                SendACK();

                                ReceivedPacket?.Invoke(yModemInfo, new PacketEventArgs(e.PacketNo, e.Packet));

                                // ֪ͨ����һ��
                                SendNextPacket?.Invoke(yModemInfo, null);
                            }


                            break;
                        case YModemMessageType.PACKET_ERROR:
                            SendNAK();
                            // ֪ͨ�ط�
                            ReSendPacket?.Invoke(yModemInfo, null);

                            break;
                        case YModemMessageType.EOT:
                            SendACK();
                            // ֪ͨ���
                            EndOfTransmit?.Invoke(yModemInfo, null);

                            break;
                        case YModemMessageType.CAN:
                            SendACK();
                            // ֪ͨ��ֹ
                            AbortTransmit?.Invoke(yModemInfo, null);

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
                        //֪ͨ���ճ�ʱ
                        TransmitTimeOut?.Invoke(yModemInfo, null);
                    }
                }
            }
        }


        #region IFileTransmit ��Ա

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
            int checkLen = 2;

            var packetLen = yModemInfo.Type == YModemType.YModem_1K ? 1024 : 128;

            var data = new byte[3 + packetLen + checkLen];

            data[0] = SOH;
            if (yModemInfo.Type == YModemType.YModem_1K)
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


        #region ITransmitUart ��Ա

        public event SendToUartEventHandler SendToUartEvent;


        public void ReceivedFromUart(byte[] data)
        {
            ParseReceivedMessage(data);
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
    }
}