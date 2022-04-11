using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NLog;
using System.Text;

namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{
    public class XMLLogViewModel : ObservableObject
    {
        public Logger Logger = LogManager.GetLogger("XML");

        private readonly StringBuilder _xmlLogStringBuilder = new StringBuilder();
        //        public string XmlLogAppend  
        //        {
        //            get => _xmlLogStringBuilder.ToString();
        //            set { _xmlLogStringBuilder.Append(value); RaisePropertyChanged(); }
        //        }

        public string XmlLog
        {
            get
            {
                //                return _XmlLogString;
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

                //                _XmlLogString = value;
                _xmlLogStringBuilder.Append(value);
                OnPropertyChanged();
            }
        }

        //        private string _XmlLogString;

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