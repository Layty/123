using System.Text;
using GalaSoft.MvvmLight;
using NLog;

namespace 三相智慧能源网关调试软件.Model
{
    public class MyNetLogModel : ObservableObject
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();
        public bool IsStartWriteLogToFile
        {
            get => _isStartWriteLogToFile;
            set { _isStartWriteLogToFile = value; RaisePropertyChanged(); }
        }

        private bool _isStartWriteLogToFile;
        public StringBuilder NetLogStringBuilder = new StringBuilder();

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