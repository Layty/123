using System;
using System.Text;
using GalaSoft.MvvmLight;
using NLog;

namespace MySerialPortMaster
{
    /*
        联系邮箱：694965217@qq.com
        创建时间：2020/05/07 8:56:16
        主要用途：记录串口通信日志
        更改记录：
    */
    public class SerialPortLogger : ObservableObject
    {
        /*--------------------------字段---------------------------------*/
        public StringBuilder SendAndReceiveDataStringBuilder { get; set; }
        public StringBuilder SendDataStringBuilder { get; set; }
        public StringBuilder ReceiveDataStringBuilder { get; set; }

        public string ErrMgs { get; set; }

        public bool IsRecordTime { get; set; }

        /*--------------------------属性---------------------------------*/


        private int _sendFrameCount;

        /// <summary>
        /// 发送帧数
        /// </summary>
        public int SendFrameCount
        {
            get => _sendFrameCount;
            private set
            {
                _sendFrameCount = value;
                RaisePropertyChanged();
            }
        }

        private int _sendBytesCount;

        /// <summary>
        /// 发送字节数
        /// </summary>
        public int SendBytesCount
        {
            get => _sendBytesCount;
            private set
            {
                _sendBytesCount = value;
                RaisePropertyChanged();
            }
        }


        private int _receiveFrameCount;

        /// <summary>
        /// 接收帧数
        /// </summary>
        public int ReceiveFrameCount
        {
            get => _receiveFrameCount;
            private set
            {
                _receiveFrameCount = value;
                RaisePropertyChanged();
            }
        }

        private int _receiveBytesCount;

        /// <summary>
        /// 接收字节数
        /// </summary>
        public int ReceiveBytesCount
        {
            get => _receiveBytesCount;
            private set
            {
                _receiveBytesCount = value;
                RaisePropertyChanged();
            }
        }

        private bool _isReceiveFormat16 = true;

        /// <summary>
        /// 接收区16进制
        /// </summary>
        public bool IsReceiveFormat16
        {
            get => _isReceiveFormat16;
            set
            {
                _isReceiveFormat16 = value;
                RaisePropertyChanged();
            }
        }

        private string _dataReceiveShowBytes;

        /// <summary>
        /// 接收区数据
        /// </summary>
        public string DataReceiveForShow
        {
            get => _dataReceiveShowBytes;
            private set
            {
                _dataReceiveShowBytes = value;
                RaisePropertyChanged();
            }
        }


        private byte[] _currentReceiveBytes;

        public byte[] CurrentReceiveBytes
        {
            get => _currentReceiveBytes;
            private set
            {
                _currentReceiveBytes = value;
                RaisePropertyChanged();
            }
        }

        private bool _isSendDataDisplayFormat16 = true;

        /// <summary>
        /// 发送区是否16进制显示
        /// </summary>
        public bool IsSendDataDisplayFormat16
        {
            get => _isSendDataDisplayFormat16;
            set
            {
                _isSendDataDisplayFormat16 = value;
                RaisePropertyChanged();
            }
        }


        private byte[] _currentSendBytes;

        /// <summary>
        /// 当前或最近一条发送数据的字节数组
        /// </summary>
        public byte[] CurrentSendBytes
        {
            get => _currentSendBytes;
            set
            {
                _currentSendBytes = value;
                RaisePropertyChanged();
            }
        }

        private string _currentSendData;

        public string CurrentSendData
        {
            get => _currentSendData;
            private set
            {
                _currentSendData = value;
                RaisePropertyChanged();
            }
        }


        private int _keepMaxSendAndReceiveDataLength = 5000;

        /// <summary>
        /// 最大支持的串口历史记录帧长度
        /// </summary>
        public int KeepMaxSendAndReceiveDataLength
        {
            get => _keepMaxSendAndReceiveDataLength;
            set
            {
                _keepMaxSendAndReceiveDataLength = value;
                RaisePropertyChanged();
            }
        }


        public void AddInfo(string info)
        {
            SendAndReceiveDataCollections = info;
        }

        public Logger Logger = LogManager.GetCurrentClassLogger();


        public bool IsEnableWriteLogToFile
        {
            get => _isEnableWriteLogToFile;
            set
            {
                _isEnableWriteLogToFile = value;
                RaisePropertyChanged();
            }
        }

        private bool _isEnableWriteLogToFile;

        public StringBuilder SendAndReceiveDataStringBuilderCollections { get; set; } = new StringBuilder();

        public string SendAndReceiveDataCollections
        {
            get => SendAndReceiveDataStringBuilderCollections.ToString();
            private set
            {
                if (SendAndReceiveDataStringBuilderCollections.Length > _keepMaxSendAndReceiveDataLength)
                {
                    SendAndReceiveDataStringBuilderCollections.Clear();
                }

                if (IsEnableWriteLogToFile)
                {
                    Logger.Trace(value);
                }

                SendAndReceiveDataStringBuilderCollections.Append(value);
                RaisePropertyChanged();
            }
        }


        public void ClearSendBuff() => CurrentSendBytes = new byte[] { };
        public void ClearDataReceiveBytes() => CurrentReceiveBytes = new byte[] { };


        public void ClearReceiveCount()
        {
            ReceiveBytesCount = 0;
            ReceiveFrameCount = 0;
        }

        /*-------------------------构造器--------------------------------*/
        public SerialPortLogger()
        {
        }

        /*------------------------公有方法-------------------------------*/
        public void StartCatchLogToFile()
        {
        }

        public void PauseCatchLogToFile()
        {
        }

        public void StopCatchLogToFile()
        {
        }


        public void ClearSendCount()
        {
            SendBytesCount = 0;
            SendFrameCount = 0;
        }

        public void HandlerSendData(byte[] sendBytes)
        {
            SendFrameCount++; //累加帧数
            SendBytesCount += sendBytes.Length; //累加字节数
            CurrentSendBytes = sendBytes;
            //根据当前设置的显示格式，进行存储
            CurrentSendData = IsSendDataDisplayFormat16
                ? sendBytes.ByteToString()
                : Encoding.ASCII.GetString(sendBytes);
            SendAndReceiveDataCollections = $"{DateTime.Now:yyyy-MM-dd hh:mm:ss fff} => {CurrentSendData}{Environment.NewLine}";
        }

        public void HandlerReceiveData(byte[] receiveBytes)
        {
            ReceiveFrameCount++;
            ReceiveBytesCount += receiveBytes.Length;
            CurrentReceiveBytes = receiveBytes;
            //根据当前设置的显示格式，进行存储
            DataReceiveForShow =
                IsReceiveFormat16 ? receiveBytes.ByteToString() : Encoding.ASCII.GetString(receiveBytes);
            SendAndReceiveDataCollections = $"{DateTime.Now:yyyy-MM-dd hh:mm:ss fff} <= {DataReceiveForShow}{Environment.NewLine}";
        }

        /// <summary>
        /// 清空发送区、接收区和收发历史缓存区
        /// </summary>
        public void ClearHistoryText()
        {
            SendAndReceiveDataStringBuilderCollections.Clear();
            SendAndReceiveDataCollections = string.Empty;
        }

        /// <summary>
        /// 清空发送区、接收区和收发历史缓存区,及次数等
        /// </summary>
        public void ClearAllText()
        {
            ClearDataReceiveBytes();
            ClearReceiveCount();
            ClearSendBuff();
            ClearSendCount();
            SendAndReceiveDataStringBuilderCollections.Clear();
            SendAndReceiveDataCollections = string.Empty;
        }

        /*------------------------私有方法-------------------------------*/
    }
}