using System.Text;
using GalaSoft.MvvmLight;
using NLog;

namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{
    public class XMLLogViewModel : ObservableObject
    {
        public Logger Logger = LogManager.GetCurrentClassLogger();
        private StringBuilder _xmlLog = new StringBuilder();
        public string XmlLogAppend  
        {
            get => _xmlLog.ToString();
            set { _xmlLog.Append(value); RaisePropertyChanged(); }
        }

        public string XmlLog
        {
            get => _XmlLog;
            set { _XmlLog = value; RaisePropertyChanged(); }
        }
        private string _XmlLog;

        private int _keepMaxSendAndReceiveDataLength = 5000;

        /// <summary>
        /// 最大支持的串口历史记录帧长度
        /// </summary>
        public int KeepMaxSendAndReceiveDataLength
        {
            get => _keepMaxSendAndReceiveDataLength;
            set
            {
                if (_xmlLog.Length > _keepMaxSendAndReceiveDataLength)
                {
                    _xmlLog.Clear();
                }

                if (IsEnableWriteLogToFile)
                {
                    Logger.Trace(value);
                }
                _keepMaxSendAndReceiveDataLength = value;
                RaisePropertyChanged();
            }
        }
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
    }
}