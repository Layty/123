using System;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MySerialPortMaster;
using 三相智慧能源网关调试软件.Converters;
using 三相智慧能源网关调试软件.DLMS.HDLC;
using 三相智慧能源网关调试软件.DLMS.HDLC.IEC21EMode;
using 三相智慧能源网关调试软件.DLMS.Wrapper;

namespace 三相智慧能源网关调试软件.DLMS
{
    public enum CommunicationType
    {
        SerialPort,
        NetWork
    }

    public class DLMSClient
    {
        private EModeFrameMaker _eModeFrameFrameMaker;
        public HdlcFrameMaker HdlcFrameMaker { get; set; }

        public NetFrameMaker NetFrameMaker { get; set; }
        public bool IsAuthenticationRequired { get; set; }

        private readonly SerialPortMaster _portMaster;
        private readonly TcpServerHelper _socket;
        public Socket CurrentSocket { get; set; }

        public MyDLMSSettings MyDlmsSettings { get; set; }

        private async Task<byte[]> HowToSendData(byte[] sendBytes)
        {
            if (MyDlmsSettings.CommunicationType == CommunicationType.SerialPort)
            {
                return await _portMaster.SendAndReceiveReturnDataAsync(sendBytes);
            }
            else if (MyDlmsSettings.CommunicationType == CommunicationType.NetWork)
            {
                _socket.SendDataToClient(CurrentSocket, sendBytes);
            }

            return null;
        }

        public async Task<byte[]> SNRMRequest()
        {
            IsAuthenticationRequired = false;
            if (MyDlmsSettings.InterfaceType != InterfaceType.HDLC)
            {
                return null;
            }

            return await HowToSendData(HdlcFrameMaker.SNRMRequest());
        }

        public void ParseUaResponse(byte[] replyData)
        {
            if (replyData.Length == 0)
            {
                MyDlmsSettings.DlmsInfo.ReceiveMaxInfoValue = 128;
                MyDlmsSettings.DlmsInfo.TransmitMaxInfoValue = 128;
                MyDlmsSettings.DlmsInfo.ReceiveMaxWindowSize = 1;
                MyDlmsSettings.DlmsInfo.TransmitMaxWindowSize = 1;
                return;
            }

            var buff = replyData.Skip(8).ToArray();
            var buff1 = (buff.Take(buff.Length - 3).ToArray());
            // Gurux.DLMS.GXDLMS.ParseSnrmUaResponse(data, Settings);
            MyDlmsSettings.Connected = ConnectionState.Hdlc;
        }

        public async Task<byte[]> AarqRequest()
        {
            byte[] bytes = new byte[] { };
            if (MyDlmsSettings.InterfaceType == InterfaceType.HDLC)
            {
//                bytes = await _portMaster.SendAndReceiveReturnDataAsync(hdlcFrameMaker.AarqRequest());
                bytes = await HowToSendData(HdlcFrameMaker.AarqRequest());
            }
            else if (MyDlmsSettings.InterfaceType == InterfaceType.WRAPPER)
            {
//                _socket.SendDataToClient(CurrentSocket, _netFrameMaker.AarqRequest());
              await  HowToSendData(NetFrameMaker.AarqRequest());
            }

            return bytes;
        }


        public async Task<byte[]> GetRequest(byte[] getAttributeBytes)
        {
            byte[] bytes = new byte[] { };
            if (MyDlmsSettings.InterfaceType == InterfaceType.HDLC)
            {
                bytes = await HowToSendData(HdlcFrameMaker.GetRequest(getAttributeBytes));
            }
            else if (MyDlmsSettings.InterfaceType == InterfaceType.WRAPPER)
            {
              await  HowToSendData(NetFrameMaker.GetRequest(getAttributeBytes));
            }

            return bytes;
        }


        public async Task SetRequest(byte[] setAttributeBytes)
        {
            if (MyDlmsSettings.InterfaceType == InterfaceType.HDLC)
            {
                await _portMaster.SendAndReceiveReturnDataAsync(HdlcFrameMaker.SetRequest(setAttributeBytes));
            }
            else if (MyDlmsSettings.InterfaceType == InterfaceType.WRAPPER)
            {
                _socket.SendDataToClient(CurrentSocket, NetFrameMaker.SetRequest(setAttributeBytes));
            }
        }


        public async Task<byte[]> ActionRequest(byte[] actionRequestBytes)
        {
            if (MyDlmsSettings.InterfaceType == InterfaceType.HDLC)
            {
                await _portMaster.SendAndReceiveReturnDataAsync(HdlcFrameMaker.ActionRequest(actionRequestBytes));
            }
            else if (MyDlmsSettings.InterfaceType == InterfaceType.WRAPPER)
            {
                _socket.SendDataToClient(CurrentSocket, NetFrameMaker.ActionRequest(actionRequestBytes));
            }

            return null;
        }


        public async Task<byte[]> DisconnectRequest(bool force)
        {
            //if (!force &&/* MyDlmsSettings.Connected == ConnectionState.None*/)
            //{
            //    return null;
            //}
            byte[] result = null;
            if (MyDlmsSettings.InterfaceType == InterfaceType.HDLC)
            {
                result = await _portMaster.SendAndReceiveReturnDataAsync(HdlcFrameMaker.DisconnectRequest());
            }
            else if (force || MyDlmsSettings.InterfaceType == InterfaceType.WRAPPER)
            {
                _socket.SendDataToClient(CurrentSocket, NetFrameMaker.ReleaseRequest());
            }

            return result;
        }

        //通用组帧
        //网络
        private void InitSerialPortParams(SerialPortMaster serialPortMaster)
        {
            serialPortMaster.DataBits = 8;
            serialPortMaster.StopBits = StopBits.One;
            serialPortMaster.Parity = Parity.None;
        }

        private void BackupPortPara()
        {
            var memento = _portMaster.CreateMySerialPortConfig;
            _caretaker.Dictionary["before"] = memento;
            _portMaster.SerialPortLogger.IsSendDataDisplayFormat16 = false;
            _portMaster.SerialPortLogger.IsReceiveFormat16 = false;
        }

        private void LoadBackupPortPara()
        {
            _portMaster.LoadSerialPortConfig(_caretaker.Dictionary["before"]);
            _portMaster.SerialPortLogger.IsSendDataDisplayFormat16 = true;
            _portMaster.SerialPortLogger.IsReceiveFormat16 = true;
        }

        private void Init21ESerialPort()
        {
            MyDlmsSettings.RequestBaud = _portMaster.BaudRate;
            _eModeFrameFrameMaker = new EModeFrameMaker(MyDlmsSettings.RequestBaud, "");
            _portMaster.BaudRate = 300;
            _portMaster.DataBits = 7;
            _portMaster.StopBits = StopBits.One;
            _portMaster.Parity = Parity.Even;
        }

        public DLMSClient(SerialPortMaster serialPortMaster, MyDLMSSettings settings)
        {
            _portMaster = serialPortMaster;
            InitSerialPortParams(_portMaster);
            MyDlmsSettings = settings;
            HdlcFrameMaker = new HdlcFrameMaker(MyDlmsSettings);
            MyDlmsSettings.CommunicationType = CommunicationType.SerialPort;
        }

        public DLMSClient(TcpServerHelper socket, MyDLMSSettings settings)
        {
            _socket = socket;
            MyDlmsSettings = settings;
            NetFrameMaker = new NetFrameMaker(MyDlmsSettings);
            MyDlmsSettings.CommunicationType = CommunicationType.NetWork;
        }


        private readonly SerialPortConfigCaretaker _caretaker = new SerialPortConfigCaretaker();


        public Task<byte[]> SetEnterUpGradeMode()
        {
            return _portMaster.SendAndReceiveReturnDataAsync(HdlcFrameMaker.SetEnterUpGradeMode(256));
        }

        public Task<bool> Execute21ENegotiate()
        {
            return Task.Run(async () =>
            {
                BackupPortPara();
                Init21ESerialPort();
                byte[] array =
                    await _portMaster.SendAndReceiveReturnDataAsync(_eModeFrameFrameMaker.GetRequestFrameBytes());
                if (array.Length != 0 && EModeParser.CheckServerFrameWisEquals2(array))
                {
                    _portMaster.Send(_eModeFrameFrameMaker.GetConfirmFrameBytes());
                    Thread.Sleep(200);
                    _portMaster.BaudRate = MyDlmsSettings.RequestBaud; //需要修改波特率 ，再去接收
                    array = _portMaster.TryToReadReceiveData();
                    if (array.Length != 0 && EModeParser.CheckServerFrameZisEqualsClient(array))
                    {
                        _portMaster.SerialPortLogger.AddInfo("协商成功");
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
    }
}