using System;
using System.Net.Sockets;
using System.Text;
using GalaSoft.MvvmLight;
using NLog;
using 三相智慧能源网关调试软件.Commom;

namespace 三相智慧能源网关调试软件.Model
{
    public class MyNetLogModel : ObservableObject
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();

        public bool IsStartWriteLogToFile
        {
            get => _isStartWriteLogToFile;
            set
            {
                _isStartWriteLogToFile = value;
                RaisePropertyChanged();
            }
        }

        private bool _isStartWriteLogToFile;
        public StringBuilder NetLogStringBuilder = new StringBuilder();
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
        public void HandlerSendData(Socket socket, byte[] sendBytes)
        {
            //根据当前设置的显示格式，进行存储
            var CurrentSendData = IsSendDataDisplayFormat16
                ? sendBytes.ByteToString()
                : Encoding.ASCII.GetString(sendBytes);
            Log = $"{DateTime.Now} => SendDataEvent 向{socket.RemoteEndPoint}发送数据{CurrentSendData}{Environment.NewLine}";
        }
        public void HandlerReceiveData(Socket socket, byte[] receiveBytes)
        {
            //根据当前设置的显示格式，进行存储
            var   DataReceiveForShow =
                IsReceiveFormat16 ? receiveBytes.ByteToString() : Encoding.ASCII.GetString(receiveBytes);
            Log = $"{DateTime.Now} ReceiveDataEvent 收到 {socket.RemoteEndPoint}数据 <={DataReceiveForShow}{Environment.NewLine}";
        }
        public string Log
        {
            get => NetLogStringBuilder.ToString();

            set
            {
                if (NetLogStringBuilder.Length > _keepMaxSendAndReceiveDataLength)
                {
                    NetLogStringBuilder.Clear();
                }

                if (IsStartWriteLogToFile)
                {
                    _logger.Trace(value);
                }

                NetLogStringBuilder.Append(value);
                RaisePropertyChanged();
            }
        }

        public int KeepMaxSendAndReceiveDataLength
        {
            get => _keepMaxSendAndReceiveDataLength;
            set
            {
                _keepMaxSendAndReceiveDataLength = value;
                RaisePropertyChanged();
            }
        }

        private int _keepMaxSendAndReceiveDataLength = 5000;


        public void ClearBuffer()
        {
            NetLogStringBuilder.Clear();
            Log = string.Empty;
        }
    }
}