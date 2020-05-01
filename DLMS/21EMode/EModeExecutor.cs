using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using 三相智慧能源网关调试软件.Model;

namespace 三相智慧能源网关调试软件.DLMS._21EMode
{
    public class EModeExecutor
    {
        private readonly MySerialPort _port;

        private readonly MySerialPort _backupPort = new MySerialPort();

        private readonly EModeMaker _eModeFrameMaker;

        private readonly int _requestBaud;

        public EModeExecutor(MySerialPort mySerialPort, string addr)
        {
            _port = mySerialPort;
            _requestBaud = mySerialPort.BaudRate;
            _eModeFrameMaker = new EModeMaker(_requestBaud, addr);
        }

        public Task<bool> Execute()
        {
            return Task.Run(delegate
            {
                if (!_port.IsOpen)
                {
                    _port.Open();
                }
                Init21ESerialPort();
                byte[] requestFrameBytes = _eModeFrameMaker.GetRequestFrameBytes();
                _port.SendDataWithLocker(requestFrameBytes);
                byte[] array = _port.TryToReadReceiveData();
                if (array.Length == 0)
                {
                    Release21ESerialPort();
                    return false;
                }
                if (!EModeParser.CheckServerFrameWisEquals2(array))
                {
                    Release21ESerialPort();
                    return false;
                }
                byte[] comfirmFrameBytes = _eModeFrameMaker.GetComfirmFrameBytes();
                _port.SendDataWithLocker(comfirmFrameBytes);
                Thread.Sleep(200);
                _port.BaudRate = _requestBaud;
                array = _port.TryToReadReceiveData();
                if (!EModeParser.CheckServerFrameZisEqualsClient(array))
                {
                    Release21ESerialPort();
                    return false;
                }
                Release21ESerialPort();
                return true;
            });
        }

        private void RollbackSerialPortParams()
        {
            _port.BaudRate = _backupPort.BaudRate;
            _port.DataBits = _backupPort.DataBits;
            _port.StopBits = _backupPort.StopBits;
            _port.Parity = _backupPort.Parity;
        }

        private void Init21ESerialPort()
        {
            _port.IsSendDataShowString = true;
            _backupPort.BaudRate = _port.BaudRate;
            _backupPort.DataBits = _port.DataBits;
            _backupPort.StopBits = _port.StopBits;
            _backupPort.Parity = _port.Parity;
            _port.BaudRate = 300;
            _port.DataBits = 7;
            _port.StopBits = StopBits.One;
            _port.Parity = Parity.Even;
        }

        private void Release21ESerialPort()
        {
            RollbackSerialPortParams();
            _port.IsSendDataShowString = false;
            //_port.DiscardInBuffer();
            //_port.DiscardOutBuffer();
            _port.Close();
            //_port.Dispose();
        }
    }
}