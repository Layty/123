using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight;
using NLog;

namespace 三相智慧能源网关调试软件.MyControl
{
    /// <summary>
    /// XMLLogControl.xaml 的交互逻辑
    /// </summary>
    public partial class XMLLogControl : UserControl
    {
        public XMLLogControl()
        {
            InitializeComponent();
        }
    }

    public class DMLSXMLLog: ObservableObject
    {
        public Logger Logger = LogManager.GetCurrentClassLogger();
        private StringBuilder _xmlLog  = new StringBuilder();
        public string XmlLog
        {
            get => _xmlLog.ToString();
            set { _xmlLog.Append(value) ; RaisePropertyChanged(); }
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
