using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Ports;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MySerialPortMaster;
using 三相智慧能源网关调试软件.Properties;
using Common = 三相智慧能源网关调试软件.Commom.Common;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class SerialPortViewModel : ViewModelBase
    {
        public SerialPortViewModel()
        {
            if (IsInDesignMode)
            {
                SerialPortConfigCaretaker =
                    new SerialPortConfigCaretaker(Settings.Default.SerialPortViewModelConfigFilePath);
                var config = SerialPortConfigCaretaker.DefaultConfig;
                SerialPortMasterModel = new SerialPortMaster(config);
            }
            else
            {
                SerialPortConfigCaretaker =
                    new SerialPortConfigCaretaker(Settings.Default.SerialPortViewModelConfigFilePath);
                var config = SerialPortConfigCaretaker.LoadSerialPortParamsByReadSerialPortConfigFile();
                SerialPortMasterModel = new SerialPortMaster(config);
                
                ClearSendDataCommand = new RelayCommand<SenderModel>(ClearSendData);
                ClearReceiveDataCommand = new RelayCommand(() => { SerialPortMasterModel.ClearDataReceiveBytes(); });
                ClearHistoryDataCommand = new RelayCommand(ClearHistoryText);
                ClearAllDataCommand = new RelayCommand(ClearAllText);
                ClearAllCountsCommand = new RelayCommand(() =>
                {
                    SerialPortMasterModel.ClearSendCount();
                    SerialPortMasterModel.ClearReceiveCount();
                });
                ClearSendCountCommand = new RelayCommand(() => SerialPortMasterModel.ClearSendCount());
                ClearReceivedCountCommand = new RelayCommand(() => SerialPortMasterModel.ClearReceiveCount());
            }

            SendersCollection = new ObservableCollection<SenderModel> {new SenderModel {SendText = "1234"}};
            SelectCommand = new RelayCommand<SenderModel>(SelectSendText);
            SendTextCommand = new RelayCommand(() =>
                {
                    SerialPortMasterModel.CurrentSendBytes = Common.StringToByte(SenderModel.SendText);
                    SerialPortMasterModel.Send();
                }
            );
            SaveSerialPortConfigFileCommand = new RelayCommand(() =>
            {
                SerialPortConfigCaretaker.SaveSerialPortConfigDataToJsonFile(SerialPortMasterModel
                    .CreateMySerialPortConfig);
            });
            OpenCalcCommand = new RelayCommand(() => { Process.Start("compmgmt.msc"); });
        }

        public SerialPortMaster SerialPortMasterModel { get; set; }
        private SerialPortConfigCaretaker SerialPortConfigCaretaker { get; set; }


        #region 串口参数资源集合

        public string[] PortNamesCollection => SerialPort.GetPortNames();
        public int[] BaudRatesCollection => new[] {300, 1200, 2400, 4800, 9600, 19200, 38400};
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
            //SerialPortMasterModel.CurrentSendBytes = senderModel.SendText.StringToByte();
            //SerialPortMasterModel.Send();
        }

        #endregion


        /// <summary>
        /// 清空发送区
        /// </summary>
        public void ClearSendData(SenderModel senderModel)
        {
            SenderModel.SendText = string.Empty;
            SerialPortMasterModel.CurrentSendBytes = new byte[] { };
        }


        /// <summary>
        /// 清空发送区、接收区和收发历史缓存区
        /// </summary>
        private void ClearHistoryText()
        {
            SerialPortMasterModel.SendAndReceiveDataStringBuilderCollections.Clear();
            SerialPortMasterModel.SendAndReceiveDataCollections = string.Empty;
        }

        /// <summary>
        /// 清空发送区、接收区和收发历史缓存区,及次数等
        /// </summary>
        private void ClearAllText()
        {
            SerialPortMasterModel.ClearDataReceiveBytes();
            SerialPortMasterModel.ClearReceiveCount();
            SerialPortMasterModel.ClearSendBuff();
            SerialPortMasterModel.ClearSendCount();
            SerialPortMasterModel.SendAndReceiveDataStringBuilderCollections.Clear();
            SerialPortMasterModel.SendAndReceiveDataCollections = string.Empty;
        }
    }
}