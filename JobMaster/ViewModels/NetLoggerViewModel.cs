using Microsoft.Extensions.Logging;

using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace JobMaster.ViewModels
{
    public partial class NetLoggerViewModel : ObservableObject
    {

        private readonly ILogger _logger;

        [ObservableProperty]
        private bool _isStartWriteLogToFile;

        public StringBuilder NetLogStringBuilder = new StringBuilder();
        /// <summary>
        /// 发送区是否16进制显示
        /// </summary>
        [ObservableProperty]
        private bool _isSendDataDisplayFormat16 = true;

        /// <summary>
        /// 接收区16进制
        /// </summary>
        [ObservableProperty]
        private bool _isReceiveFormat16 = true;
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
                OnPropertyChanged();
            }
        }


        [ObservableProperty]
        private int _keepMaxSendAndReceiveDataLength = 5000;

        [ICommand]
        public void ClearBuffer()
        {
            NetLogStringBuilder.Clear();
            Log = string.Empty;
        }


        public NetLoggerViewModel(ILogger logger)
        {
            _logger = logger;

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
            Log = message + "\r\n";
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