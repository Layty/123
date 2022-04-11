using DataNotification.Commom;
using CommunityToolkit.Mvvm.ComponentModel;
using NLog;
using System;
using System.Net.Sockets;
using System.Text;

namespace DataNotification.Model
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }
        public void HandlerSendData(Socket socket, byte[] sendBytes)
        {
            //根据当前设置的显示格式，进行存储
            var currentSendData = IsSendDataDisplayFormat16
                ? sendBytes.ByteToString(" ")
                : Encoding.ASCII.GetString(sendBytes);
            Log = $"{DateTime.Now:yy-MM-dd HH:mm:ss fff} SendDataEvent 向 {socket.RemoteEndPoint}发送数据 => {currentSendData}{Environment.NewLine}";
        }
        public void HandlerReceiveData(Socket socket, byte[] receiveBytes)
        {
            //根据当前设置的显示格式，进行存储
            var dataReceiveForShow =
                IsReceiveFormat16 ? receiveBytes.ByteToString(" ") : Encoding.ASCII.GetString(receiveBytes);
            Log = $"{DateTime.Now:yy-MM-dd HH:mm:ss fff} ReceiveDataEvent 收到 {socket.RemoteEndPoint}数据 <= {dataReceiveForShow}{Environment.NewLine}";
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
                OnPropertyChanged();
            }
        }

        public int KeepMaxSendAndReceiveDataLength
        {
            get => _keepMaxSendAndReceiveDataLength;
            set
            {
                _keepMaxSendAndReceiveDataLength = value;
                OnPropertyChanged();
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