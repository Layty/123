using System;
using System.Diagnostics;
using System.IO.Ports;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MySerialPortMaster;

namespace 三相智慧能源网关调试软件.ViewModels
{
    public class SerialPortViewModel : ObservableObject
    {

        public SerialPortMaster SerialPortMaster { get; set; }

        public SerialPortViewModel(SerialPortMaster serialPortMaster)
        {
            SerialPortMaster = serialPortMaster;

            {

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
                serialPortMaster.SerialPortConfigCaretaker.SaveSerialPortConfigToJsonFile(SerialPortMaster
                    .CreateCurrentSerialPortConfig);
            });
            OpenCalcCommand = new RelayCommand(() => { Process.Start("compmgmt.msc"); });
            OpenOrCloseCommand = new RelayCommand(() =>
            {
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
                    StrongReferenceMessenger.Default.Send(e.Message, "Snackbar");
                }
            });
        }


        public RelayCommand OpenOrCloseCommand { get; set; }


        #region 串口参数资源集合

        public string[] PortNamesCollection => SerialPort.GetPortNames();
        public int[] BaudRatesCollection => new[] { 300, 1200, 2400, 4800, 9600, 19200, 38400, 115200 };
        public Array ParityCollection => Enum.GetValues(typeof(Parity));
        public StopBits[] StopBitsCollection => new[] { StopBits.One, StopBits.OnePointFive, StopBits.Two };
        public int[] DataBitsCollection => new[] { 6, 7, 8 };

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


        #endregion

        #region 其他

        public RelayCommand OpenCalcCommand { get; set; }

        #endregion


    }
}