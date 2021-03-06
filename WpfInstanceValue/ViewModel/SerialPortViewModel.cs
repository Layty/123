using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MySerialPortMaster;
using WpfInstanceValue.Properties;

namespace WpfInstanceValue.ViewModel
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
                SerialPortMaster = new SerialPortMaster(config);
            }
            else
            {
                SerialPortConfigCaretaker =
                    new SerialPortConfigCaretaker(Settings.Default.SerialPortViewModelConfigFilePath);
                var config = SerialPortConfigCaretaker.LoadSerialPortParamsByReadSerialPortConfigFile();
                SerialPortMaster = new SerialPortMaster(config);

                ClearReceiveDataCommand = new RelayCommand(() =>
                {
                    SerialPortMaster.SerialPortLogger.ClearDataReceiveBytes();
                });
                ClearHistoryDataCommand =
                    new RelayCommand(() => SerialPortMaster.SerialPortLogger.ClearHistoryText());
                ClearAllDataCommand = new RelayCommand(() => SerialPortMaster.SerialPortLogger.ClearAllText());
                ClearAllCountsCommand = new RelayCommand(() =>
                {
                    SerialPortMaster.SerialPortLogger.ClearSendCount();
                    SerialPortMaster.SerialPortLogger.ClearReceiveCount();
                });
                ClearSendCountCommand = new RelayCommand(() => SerialPortMaster.SerialPortLogger.ClearSendCount());
                ClearReceivedCountCommand =
                    new RelayCommand(() => SerialPortMaster.SerialPortLogger.ClearReceiveCount());


            }

        
            SaveSerialPortConfigFileCommand = new RelayCommand(() =>
            {
                SerialPortConfigCaretaker.SaveSerialPortConfigDataToJsonFile(SerialPortMaster
                    .CreateMySerialPortConfig);
            });
            OpenCalcCommand = new RelayCommand( () => {  Process.Start("compmgmt.msc"); });
            OpenOrCloseCommand=new RelayCommand( ()=> {
                try
                {
                    if (!SerialPortMaster.IsOpen)
                    {
                        SerialPortMaster.Open();
                    }
                    else
                    {
                        SerialPortMaster.Close();
                    }
                    SaveSerialPortConfigFileCommand.Execute(null);
                }
                catch (Exception e)
                {
//                    var view = new 三相智慧能源网关调试软件.MyControl.MessageBox() { Message = e.Message, Title = e.Source };
                    //  await DialogHost.Show(view, "SerialPortPage");
                    MessageBox.Show(e.Message);
                }
               
            });
        }
    
        public SerialPortMaster SerialPortMaster
        {
            get => _serialPortMaster;
            set
            {
                _serialPortMaster = value;
                RaisePropertyChanged();
            }
        }

        private SerialPortMaster _serialPortMaster;
        private SerialPortConfigCaretaker SerialPortConfigCaretaker { get; set; }


        public RelayCommand OpenOrCloseCommand
        {
            get => _openOrCloseCommand;
            set { _openOrCloseCommand = value; RaisePropertyChanged(); }
        }
        private RelayCommand _openOrCloseCommand;


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

    
        public RelayCommand SendTextCommand { get; set; }
       
       

    

       

       
        #endregion
    }
}