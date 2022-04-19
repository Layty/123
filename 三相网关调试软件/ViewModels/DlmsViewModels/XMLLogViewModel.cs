using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NLog;

namespace 三相智慧能源网关调试软件.ViewModels.DlmsViewModels
{
    public class XMLLogViewModel : ObservableObject
    {
        public Logger Logger = LogManager.GetLogger("XML");

        private readonly StringBuilder _xmlLogStringBuilder = new StringBuilder();


        public string XmlLog
        {
            get
            {
                return _xmlLogStringBuilder.ToString();
            }
            set
            {
                if (_xmlLogStringBuilder.Length > _keepMaxSendAndReceiveDataLength)
                {
                    _xmlLogStringBuilder.Clear();
                }

                if (IsEnableWriteLogToFile)
                {
                    Logger.Trace(value);
                }

                _xmlLogStringBuilder.Append(value);
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }

        public bool IsEnableWriteLogToFile
        {
            get => _isEnableWriteLogToFile;
            set
            {
                _isEnableWriteLogToFile = value;
                OnPropertyChanged();
            }
        }

        private bool _isEnableWriteLogToFile;


        public RelayCommand ClearAllDataCommand { get; set; }


        public XMLLogViewModel()
        {
            ClearAllDataCommand = new RelayCommand(() =>
            {
                _xmlLogStringBuilder.Clear();
                XmlLog = "";
            });
        }
    }
}