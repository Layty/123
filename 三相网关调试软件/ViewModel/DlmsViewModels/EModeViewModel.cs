using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MySerialPortMaster;
using 三相智慧能源网关调试软件.DLMS.HDLC.IEC21EMode;

namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{
    public class EModeViewModel:ViewModelBase
    {
        private readonly SerialPortMaster _opticalPortMaster;

        private  EModeFrame _eModeFrame;

        public int NegotiateBaud
        {
            get => _negotiateBaud;
            set { _negotiateBaud = value; RaisePropertyChanged(); }
        }
        private int _negotiateBaud;
        public int StartBaud
        {
            get => _startBaud;
            set { _startBaud = value; RaisePropertyChanged(); }
        }
        private int _startBaud=300;

        public string MeterAddress
        {
            get => _meterAddress;
            set { _meterAddress = value; RaisePropertyChanged(); }
        }
        private string _meterAddress="";

        private readonly SerialPortConfigCaretaker _caretaker = new SerialPortConfigCaretaker();

        public EModeViewModel(DLMSSettingsViewModel settings)
        {
            StartBaud = settings.StartBaud;
            NegotiateBaud = settings.NegotiateBaud;
        }
        public EModeViewModel(SerialPortMaster serialOpticalPortMaster, string addr="")
        {
            _opticalPortMaster = serialOpticalPortMaster;
         
        }
        public EModeViewModel(SerialPortMaster serialOpticalPortMaster, EModeFrame eModeFrame)
        {
            _opticalPortMaster = serialOpticalPortMaster;
            _eModeFrame = eModeFrame;
        }
        /// <summary>
        /// 执行21e协商
        /// </summary>
        /// <returns></returns>

        public Task<bool> Execute21ENegotiateAsync()
        {
            return Task.Run(async () =>
            {
                BackupPortPara();
                Init21ESerialPort();
                byte[] array = await _opticalPortMaster.SendAndReceiveReturnDataAsync(_eModeFrame.GetRequestFrameBytes());
                if (array.Length != 0 && EModeParser.CheckServerFrameWisEquals2(array))
                {
                    _opticalPortMaster.Send(_eModeFrame.GetConfirmFrameBytes());
                    Thread.Sleep(200);
                    _opticalPortMaster.BaudRate = _negotiateBaud; //需要修改波特率 ，再去接收
                    array = _opticalPortMaster.TryToReadReceiveData();
                    if (array.Length != 0 && EModeParser.CheckServerFrameZisEqualsClient(array))
                    {
                        _opticalPortMaster.SerialPortLogger.AddInfo("协商成功");
                        LoadBackupPortPara();
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    LoadBackupPortPara();
                    return false;
                }



            });
        }

        /// <summary>
        /// 初始化21E的串口实例
        /// </summary>
        private void Init21ESerialPort()
        {
            _negotiateBaud = _opticalPortMaster.BaudRate;
            _eModeFrame = new EModeFrame(_negotiateBaud);
            _opticalPortMaster.BaudRate = StartBaud;
            _opticalPortMaster.DataBits = 7;
            _opticalPortMaster.StopBits = StopBits.One;
            _opticalPortMaster.Parity = Parity.Even;
        }


        /// <summary>
        /// 备份当前串口参数，用于后续恢复
        /// </summary>
        private void BackupPortPara()
        {
            var memento = _opticalPortMaster.CreateMySerialPortConfig;
            _caretaker.Dictionary["before"] = memento;
            _opticalPortMaster.SerialPortLogger.IsSendDataDisplayFormat16 = false;
            _opticalPortMaster.SerialPortLogger.IsReceiveFormat16 = false;
        }

        /// <summary>
        /// 恢复备份的串口参数
        /// </summary>
        private void LoadBackupPortPara()
        {
            _opticalPortMaster.LoadSerialPortConfig(_caretaker.Dictionary["before"]);
            _opticalPortMaster.SerialPortLogger.IsSendDataDisplayFormat16 = true;
            _opticalPortMaster.SerialPortLogger.IsReceiveFormat16 = true;
        }
    }
}