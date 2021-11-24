using Microsoft.Extensions.Logging;
using Prism.Commands;
using Prism.Mvvm;
using System.Text;

namespace JobMaster.ViewModels
{
    public class NetLoggerViewModel : BindableBase
    {
        public DelegateCommand ClearServerBufferCommand { get; set; }
        private readonly ILogger _logger;

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
                    _logger.LogTrace(value);
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


        public NetLoggerViewModel(ILogger logger)
        {
            _logger = logger;
            ClearServerBufferCommand = new DelegateCommand(() => { ClearBuffer(); });
        }
        #region 重新封装Nlog
        public void LogTrace(string message)
        {
            _logger.LogTrace(message);
        }
        public void LogDebug(string message)
        {
            _logger.LogDebug(message);
        }
        public void LogInfo(string message)
        {
            Log = message + "\r\n";
            _logger.LogInformation(message);
        }
        public void LogFront(string message)
        {
            Log = message+"\r\n";
            _logger.LogTrace(message);
        }
        public void LogWarn(string message)
        {
            Log = message + "\r\n";
            _logger.LogWarning(message);
        }
        public void LogError(string message)
        {
            Log = message + "\r\n";
            _logger.LogError(message);
        }
        #endregion
    }
}