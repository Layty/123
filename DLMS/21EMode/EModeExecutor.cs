using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using MySerialPortMaster;

namespace 三相智慧能源网关调试软件.DLMS._21EMode
{
    public class EModeExecutor
    {
        private readonly SerialPortMaster _opticalPortMaster;

        private readonly EModeMaker _eModeFrameMaker;

        private readonly int _requestBaud;

        private readonly SerialPortConfigCaretaker _caretaker = new SerialPortConfigCaretaker();

        public EModeExecutor(SerialPortMaster serialOpticalPortMaster, string addr)
        {
            _opticalPortMaster = serialOpticalPortMaster;
            _requestBaud = serialOpticalPortMaster.BaudRate;
            _eModeFrameMaker = new EModeMaker(_requestBaud, addr);
        }

        private void Init21ESerialPort()
        {
            _opticalPortMaster.BaudRate = 300;
            _opticalPortMaster.DataBits = 7;
            _opticalPortMaster.StopBits = StopBits.One;
            _opticalPortMaster.Parity = Parity.Even;
        }

        public Task<bool> Execute()
        {
            return Task.Run(async delegate
            {
                BackupPortPara();
                Init21ESerialPort();
                byte[] requestFrameBytes = _eModeFrameMaker.GetRequestFrameBytes();
                //_opticalPortMaster.SendDataWithLocker(requestFrameBytes);
                //byte[] array = _opticalPortMaster.TryToReadReceiveData();
                byte[] array = await _opticalPortMaster.SendAndReceiveReturnDataAsync(requestFrameBytes);
                if (array.Length == 0 || !EModeParser.CheckServerFrameWisEquals2(array))
                {
                    LoadBackupPortPara();
                    return false;
                }


                byte[] confirmFrameBytes = _eModeFrameMaker.GetConfirmFrameBytes();
                _opticalPortMaster.Send(confirmFrameBytes);
                Thread.Sleep(200);
                _opticalPortMaster.BaudRate = _requestBaud; //需要修改波特率 ，再去接收
                array = _opticalPortMaster.TryToReadReceiveData();
                if (array.Length == 0 || !EModeParser.CheckServerFrameZisEqualsClient(array))
                {
                    LoadBackupPortPara();
                    return false;
                }

                LoadBackupPortPara();
                return true;
            });
        }

        private void BackupPortPara()
        {
            var memento = _opticalPortMaster.CreateMySerialPortConfig;
            _caretaker.Dictionary["before"] = memento;
            _opticalPortMaster.IsSendFormat16 = false;
            _opticalPortMaster.IsReceiveFormat16 = false;
        }

        private void LoadBackupPortPara()
        {
            _opticalPortMaster.LoadSerialPortConfig(_caretaker.Dictionary["before"]);
            _opticalPortMaster.IsSendFormat16 = true;
            _opticalPortMaster.IsReceiveFormat16 = true;
        }
    }
}