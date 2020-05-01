using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Text;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.Model;
using 三相智慧能源网关调试软件.Properties;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class SerialPortViewModel : ViewModelBase
    {
        public SerialPortViewModel()
        {
            if (IsInDesignMode)
            {
                MySerialPortConfigCaretaker =
                    new MySerialPortConfigCaretaker(Settings.Default.SerialPortViewModelConfigFilePath);
                var config = MySerialPortConfigCaretaker.DefaultConfig;

                SerialPortModel = new MySerialPort();
                SerialPortModel.LoadSerialPortConfig(config);

                SendersCollection = new ObservableCollection<SenderModel>()
                    {new SenderModel() {SendText = "1234"}};
            }
            else
            {
               
                MySerialPortConfigCaretaker =
                    new MySerialPortConfigCaretaker(Settings.Default.SerialPortViewModelConfigFilePath);
                var config = MySerialPortConfigCaretaker.LoadSerialPortParamsByReadSerialPortConfigFile();
                SerialPortModel = new MySerialPort();
                SerialPortModel.LoadSerialPortConfig(config);

                ClearSendDataCommand = new RelayCommand<SenderModel>(ClearSendData);
                ClearReceiveDataCommand = new RelayCommand(ClearReceiveData);
                ClearHistoryDataCommand = new RelayCommand(ClearHistoryText);
                ClearAllDataCommand = new RelayCommand(ClearAllText);
                ClearAllCountsCommand = new RelayCommand(() =>
                {
                    SerialPortModel.SendFrameCount = 0;
                    SerialPortModel.SendBytesCount = 0;
                    SerialPortModel.ReceiveFrameCount = 0;
                    SerialPortModel.ReceiveBytesCount = 0;
                });
                ClearSendCountCommand = new RelayCommand(() => SerialPortModel.SendFrameCount = 0);
                ClearReceivedCountCommand = new RelayCommand(() => SerialPortModel.ReceiveFrameCount = 0);
            }

            SendersCollection = new ObservableCollection<SenderModel>()
                {new SenderModel() {SendText = "1234"}};
            SelectCommand = new RelayCommand<SenderModel>(SelectSendText);
            SendTextCommand = new RelayCommand(() =>
                {
                    SerialPortModel.SendBytes = SenderModel.SendText.StringToByte();
                    SerialPortModel.SendDataTaskMVVM();
                }
            );
            SaveSerialPortConfigFileCommand = new RelayCommand(() =>
            {
                MySerialPortConfigCaretaker.SaveSerialPortConfigDataToJsonFile(SerialPortModel
                    .CreateMySerialPortConfig);
            });
            OpenCalcCommand = new RelayCommand(() => { Process.Start("compmgmt.msc"); });
        }

        public MySerialPort SerialPortModel { get; set; }
        private MySerialPortConfigCaretaker MySerialPortConfigCaretaker { get; set; }


        #region 串口参数资源集合

        public string[] PortNamesCollection => SerialPort.GetPortNames();
        public int[] BaudRatesCollection => new[] {1200, 2400, 4800, 9600, 19200, 38400};
        public Array ParityCollection => Enum.GetValues(typeof(Parity));
        public StopBits[] StopBitsCollection => new[] {StopBits.One, StopBits.OnePointFive, StopBits.Two};
        public int[] DataBitsCollection => new[] {6, 7, 8};

        #endregion

        #region 清空命令

        public RelayCommand ClearReceiveDataCommand { get; set; }
        public RelayCommand ClearHistoryDataCommand { get; set; }
        public RelayCommand ClearAllDataCommand { get; set; }
        public RelayCommand ClearAllCountsCommand { get; set; }
        public RelayCommand ClearSendCountCommand { get; set; }
        public RelayCommand ClearReceivedCountCommand { get; set; }

        #endregion

        #region 保存串口配置信息值Json文件

        public RelayCommand SaveSerialPortConfigFileCommand { get; set; }
        public RelayCommand SaveSerialPortLogCommand { get; set; }

        #endregion

        #region 其他

        public RelayCommand OpenCalcCommand { get; set; }

        #endregion

        #region 发送区

        public RelayCommand<SenderModel> ClearSendDataCommand { get; set; }
        public RelayCommand SendTextCommand { get; set; }
        private ObservableCollection<SenderModel> _sendersCollection;
        public RelayCommand<SenderModel> SelectCommand { get; set; }

        public ObservableCollection<SenderModel> SendersCollection
        {
            get => _sendersCollection;
            set
            {
                _sendersCollection = value;
                RaisePropertyChanged();
            }
        }

        private SenderModel _senderModel = new SenderModel();

        public SenderModel SenderModel
        {
            get => _senderModel;
            set
            {
                _senderModel = value;
                RaisePropertyChanged();
            }
        }

        private void SelectSendText(SenderModel senderModel)
        {
            SenderModel = senderModel;
            //SerialPortModel.SendBytes = senderModel.SendText.StringToByte();
            //SerialPortModel.SendDataTaskMVVM();
        }

        #endregion


        /// <summary>
        /// 清空发送区
        /// </summary>
        public void ClearSendData(SenderModel senderModel)
        {
            SenderModel.SendText = string.Empty;
            SerialPortModel.SendStringDataForShow = string.Empty;
        }

        /// <summary>
        /// 清空接收区
        /// </summary>
        private void ClearReceiveData()
        {
            SerialPortModel.DataReceiveForShow = string.Empty;
        }

        /// <summary>
        /// 清空发送区、接收区和收发历史缓存区
        /// </summary>
        private void ClearHistoryText()
        {
            SerialPortModel.SendAndReceiveDataStringBuilderCollections = new StringBuilder();
            SerialPortModel.SendAndReceiveDataCollections = string.Empty;
        }

        /// <summary>
        /// 清空发送区、接收区和收发历史缓存区,及次数等
        /// </summary>
        private void ClearAllText()
        {
            SerialPortModel.SendStringDataForShow = string.Empty;
            SerialPortModel.DataReceiveForShow = string.Empty;
            SerialPortModel.SendAndReceiveDataStringBuilderCollections = new StringBuilder();
            SerialPortModel.SendAndReceiveDataCollections = string.Empty;
            SerialPortModel.SendFrameCount = 0;
            SerialPortModel.ReceiveFrameCount = 0;
        }
    }
}