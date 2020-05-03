using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using 三相智慧能源网关调试软件.Model;

namespace 三相智慧能源网关调试软件.DLMS._21EMode
{
    public class EModeExecutor
    {
        private readonly MySerialPort _port;

        private readonly EModeMaker _eModeFrameMaker;

        private readonly int _requestBaud;
        private readonly MySerialPortConfigCaretaker _caretaker = new MySerialPortConfigCaretaker();
        public EModeExecutor(MySerialPort mySerialPort, string addr)
        {
            _port = mySerialPort;
            _requestBaud = mySerialPort.BaudRate;
            _eModeFrameMaker = new EModeMaker(_requestBaud, addr);
        }

        public Task<bool> Execute()
        {
            return Task.Run( async delegate
            {
                BackupPortPara();
                Init21ESerialPort();
                byte[] requestFrameBytes = _eModeFrameMaker.GetRequestFrameBytes();
                //_port.SendDataWithLocker(requestFrameBytes);
                //byte[] array = _port.TryToReadReceiveData();
                byte[] array =await _port.SendAndReceiveReturnDataAsync(requestFrameBytes);
                if (array.Length == 0|| !EModeParser.CheckServerFrameWisEquals2(array))
                {
                    LoadBackupPortPara();
                    return false;
                }


                byte[] confirmFrameBytes = _eModeFrameMaker.GetConfirmFrameBytes();
                _port.Send(confirmFrameBytes);
                Thread.Sleep(200);
                _port.BaudRate = _requestBaud;//需要修改波特率 ，再去接收
                array = _port.TryToReadReceiveData();
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
            var memento = _port.CreateMySerialPortConfig;
            _caretaker.Dictionary["before"] = memento;
            _port.IsSendFormat16 = false;
            _port.IsReceiveFormat16 = false;
        }

        private void LoadBackupPortPara()
        {
            _port.LoadSerialPortConfig(_caretaker.Dictionary["before"]);
            _port.IsSendFormat16 = true;
            _port.IsReceiveFormat16 = true;
        }

        private void Init21ESerialPort()
        {
            _port.BaudRate = 300;
            _port.DataBits = 7;
            _port.StopBits = StopBits.One;
            _port.Parity = Parity.Even;
        }

    }
}