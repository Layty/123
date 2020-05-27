using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using MySerialPortMaster;

namespace 三相智慧能源网关调试软件.DLMS.HDLC.IEC21EMode
{
    public class EModeExecutor
    {
        private readonly SerialPortMaster _opticalPortMaster;

        private  EModeFrameMaker _eModeFrameFrameMaker;

        private  int _requestBaud;

        private readonly SerialPortConfigCaretaker _caretaker = new SerialPortConfigCaretaker();

        public EModeExecutor(SerialPortMaster serialOpticalPortMaster, string addr="")
        {
            _opticalPortMaster = serialOpticalPortMaster;
        }
        public EModeExecutor(SerialPortMaster serialOpticalPortMaster, EModeFrameMaker eModeFrameFrameMaker)
        {
            _opticalPortMaster = serialOpticalPortMaster;
            _eModeFrameFrameMaker = eModeFrameFrameMaker;
        }
        private void Init21ESerialPort()
        {
            _requestBaud = _opticalPortMaster.BaudRate;
            _eModeFrameFrameMaker = new EModeFrameMaker(_requestBaud, "");
            _opticalPortMaster.BaudRate = 300;
            _opticalPortMaster.DataBits = 7;
            _opticalPortMaster.StopBits = StopBits.One;
            _opticalPortMaster.Parity = Parity.Even;
        }

        public Task<bool> Execute21ENegotiate()
        {
            return Task.Run(async delegate
            {
                BackupPortPara();
                Init21ESerialPort();
                byte[] array = await _opticalPortMaster.SendAndReceiveReturnDataAsync(_eModeFrameFrameMaker.GetRequestFrameBytes());
                if (array.Length != 0 && EModeParser.CheckServerFrameWisEquals2(array))
                {
                    _opticalPortMaster.Send(_eModeFrameFrameMaker.GetConfirmFrameBytes());
                    Thread.Sleep(200);
                    _opticalPortMaster.BaudRate = _requestBaud; //需要修改波特率 ，再去接收
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

        private void BackupPortPara()
        {
            var memento = _opticalPortMaster.CreateMySerialPortConfig;
            _caretaker.Dictionary["before"] = memento;
            _opticalPortMaster.SerialPortLogger.IsSendDataDisplayFormat16 = false;
            _opticalPortMaster.SerialPortLogger.IsReceiveFormat16 = false;
        }

        private void LoadBackupPortPara()
        {
            _opticalPortMaster.LoadSerialPortConfig(_caretaker.Dictionary["before"]);
            _opticalPortMaster.SerialPortLogger.IsSendDataDisplayFormat16 = true;
            _opticalPortMaster.SerialPortLogger.IsReceiveFormat16 = true;
        }
    }
}