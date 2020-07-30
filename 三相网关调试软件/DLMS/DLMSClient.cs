using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MySerialPortMaster;
using 三相智慧能源网关调试软件.DLMS.HDLC;
using 三相智慧能源网关调试软件.DLMS.HDLC.Enums;
using 三相智慧能源网关调试软件.DLMS.HDLC.IEC21EMode;
using 三相智慧能源网关调试软件.DLMS.Wrapper;
using 三相智慧能源网关调试软件.ViewModel;

namespace 三相智慧能源网关调试软件.DLMS
{
    public class DLMSClient : ViewModelBase
    {
        private EModeFrameMaker _eModeFrameFrameMaker;
        public HdlcFrameMaker HdlcFrameMaker { get; set; }
        public NetFrameMaker NetFrameMaker { get; set; }
        public bool IsAuthenticationRequired { get; set; }
        private SerialPortMaster PortMaster { get; set; }
        private TcpServerHelper _socket { get; set; }
        public Socket CurrentSocket { get; set; }

        public MyDLMSSettings MyDlmsSettings { get; set; }

        private async Task<byte[]> HowToSendData(byte[] sendBytes)
        {
            var returnPduBytes = new byte[] { };
            if (MyDlmsSettings.CommunicationType == CommunicationType.SerialPort)
            {
                var hdlcReceiveData = await PortMaster.SendAndReceiveReturnDataAsync(sendBytes);
                //取消FC校验 和 0x7E尾
                //TODO :校验CRC
                var pduAndFcsBytes = hdlcReceiveData.Skip(11).ToArray();
                returnPduBytes = pduAndFcsBytes.Take(pduAndFcsBytes.Length - 3).ToArray();
            }
            else if (MyDlmsSettings.CommunicationType == CommunicationType.NetWork)
            {
                var wrapperReceiveData = await _socket.SendDataToClientAndWaitReceiveData(CurrentSocket, sendBytes);

                returnPduBytes = wrapperReceiveData.Skip(8).ToArray();
            }

            return returnPduBytes;
        }

        public async Task<byte[]> InitRequest()
        {
            byte[] bytes = null;
            if (MyDlmsSettings.CommunicationType != CommunicationType.NetWork)
            {
                if (MyDlmsSettings.StartProtocolType == StartProtocolType.IEC21E)
                {
                    var flag21E = await Execute21ENegotiate();
                    if (flag21E)
                    {
                        bytes = await SNRMRequest();
                        bytes = await AarqRequest();
                    }
                }
                else
                {
                    bytes = await SNRMRequest();
                    bytes = await AarqRequest();
                }
            }
            else
            {
                bytes = await AarqRequest();
            }

            return bytes;
        }

        public async Task<bool> Execute21ENegotiate()
        {
            bool isSucceed = false;
            if (MyDlmsSettings.StartProtocolType == StartProtocolType.IEC21E)
            {
                var EModeExecutor = new EModeExecutor(PortMaster, "");
                var keepAutoDataReceived = false;
                if (PortMaster.IsAutoDataReceived)
                {
                    keepAutoDataReceived = true;
                    PortMaster.IsAutoDataReceived = false;
                }

                isSucceed = await EModeExecutor.Execute21ENegotiate();
                if (keepAutoDataReceived)
                {
                    PortMaster.IsAutoDataReceived = true;
                }
            }

            return isSucceed;
        }

        /// <summary>
        /// 只有HDLC-46才需要SNRM
        /// </summary>
        /// <returns></returns>
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
                bytes = await HowToSendData(HdlcFrameMaker.AarqRequest());
            }
            else if (MyDlmsSettings.InterfaceType == InterfaceType.WRAPPER)
            {
                bytes = await HowToSendData(NetFrameMaker.AarqRequest());
            }

            return bytes;
        }


        public async Task<byte[]> GetRequest(byte[] getAttributeBytes)
        {
            byte[] bytes = new byte[] { };
            if (MyDlmsSettings.InterfaceType == InterfaceType.HDLC)
            {
                bytes = await HowToSendData(HdlcFrameMaker.BuildPduRequestBytes(getAttributeBytes));
            }
            else if (MyDlmsSettings.InterfaceType == InterfaceType.WRAPPER)
            {
                bytes = await HowToSendData(NetFrameMaker.BuildPduRequestBytes(getAttributeBytes));
            }

            return bytes;
        }


        public async Task<byte[]> SetRequest(byte[] setAttributeBytes)
        {
            byte[] bytes = { };
            if (MyDlmsSettings.InterfaceType == InterfaceType.HDLC)
            {
                bytes = await HowToSendData(HdlcFrameMaker.BuildPduRequestBytes(setAttributeBytes));
//                bytes = await _portMaster.SendAndReceiveReturnDataAsync(
//                    HdlcFrameMaker.BuildPduRequestBytes(setAttributeBytes));
            }
            else if (MyDlmsSettings.InterfaceType == InterfaceType.WRAPPER)
            {
                bytes = await HowToSendData(NetFrameMaker.BuildPduRequestBytes(setAttributeBytes));
//                bytes = await _socket.SendDataToClientAndWaitReceiveData(CurrentSocket,
//                    NetFrameMaker.BuildPduRequestBytes(setAttributeBytes));
            }

            return bytes;
        }


        public async Task<byte[]> ActionRequest(byte[] actionRequestBytes)
        {
            if (MyDlmsSettings.InterfaceType == InterfaceType.HDLC)
            {
                await PortMaster.SendAndReceiveReturnDataAsync(
                    HdlcFrameMaker.BuildPduRequestBytes(actionRequestBytes));
            }
            else if (MyDlmsSettings.InterfaceType == InterfaceType.WRAPPER)
            {
                _socket.SendDataToClient(CurrentSocket, NetFrameMaker.BuildPduRequestBytes(actionRequestBytes));
            }

            return null;
        }


        public async Task<byte[]> DisconnectRequest(bool force = true)
        {
            byte[] result = null;
            if (force && (MyDlmsSettings.InterfaceType == InterfaceType.HDLC))
            {
                result = await HowToSendData(HdlcFrameMaker.DisconnectRequest());
            }
            else if (force && MyDlmsSettings.InterfaceType == InterfaceType.WRAPPER)
            {
                result = await HowToSendData(NetFrameMaker.ReleaseRequest());
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
            var memento = PortMaster.CreateMySerialPortConfig;
            _caretaker.Dictionary["before"] = memento;
            PortMaster.SerialPortLogger.IsSendDataDisplayFormat16 = false;
            PortMaster.SerialPortLogger.IsReceiveFormat16 = false;
        }

        private void LoadBackupPortPara()
        {
            PortMaster.LoadSerialPortConfig(_caretaker.Dictionary["before"]);
            PortMaster.SerialPortLogger.IsSendDataDisplayFormat16 = true;
            PortMaster.SerialPortLogger.IsReceiveFormat16 = true;
        }

        private void Init21ESerialPort()
        {
            MyDlmsSettings.RequestBaud = PortMaster.BaudRate;
            _eModeFrameFrameMaker = new EModeFrameMaker(MyDlmsSettings.RequestBaud, "");
            PortMaster.BaudRate = 300;
            PortMaster.DataBits = 7;
            PortMaster.StopBits = StopBits.One;
            PortMaster.Parity = Parity.Even;
        }


        public RelayCommand InitRequestCommand
        {
            get => _initRequestCommand;
            set
            {
                _initRequestCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _initRequestCommand;

        public RelayCommand ReleaseRequestCommand
        {
            get => _releaseRequestCommand;
            set
            {
                _releaseRequestCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _releaseRequestCommand;


        public DLMSClient()
        {
            MyDlmsSettings = ServiceLocator.Current.GetInstance<MyDLMSSettings>();
            HdlcFrameMaker = new HdlcFrameMaker(MyDlmsSettings);
            NetFrameMaker = new NetFrameMaker(MyDlmsSettings);
            _socket = ServiceLocator.Current.GetInstance<TcpServerViewModel>().TcpServerHelper;
            PortMaster = ServiceLocator.Current.GetInstance<SerialPortViewModel>().SerialPortMaster;
            InitSerialPortParams(PortMaster);
            InitRequestCommand = new RelayCommand(async () => { await InitRequest(); });
            ReleaseRequestCommand = new RelayCommand(async () => { await DisconnectRequest(true); });
        }

        private readonly SerialPortConfigCaretaker _caretaker = new SerialPortConfigCaretaker();


        public Task<byte[]> SetEnterUpGradeMode()
        {
            return PortMaster.SendAndReceiveReturnDataAsync(HdlcFrameMaker.SetEnterUpGradeMode(256));
        }

        //public Task<bool> Execute21ENegotiate()
        //{
        //    return Task.Run(async () =>
        //    {
        //        BackupPortPara();
        //        Init21ESerialPort();
        //        byte[] array =
        //            await _portMaster.SendAndReceiveReturnDataAsync(_eModeFrameFrameMaker.GetRequestFrameBytes());
        //        if (array.Length != 0 && EModeParser.CheckServerFrameWisEquals2(array))
        //        {
        //            _portMaster.Send(_eModeFrameFrameMaker.GetConfirmFrameBytes());
        //            Thread.Sleep(200);
        //            _portMaster.BaudRate = MyDlmsSettings.RequestBaud; //需要修改波特率 ，再去接收
        //            array = _portMaster.TryToReadReceiveData();
        //            if (array.Length != 0 && EModeParser.CheckServerFrameZisEqualsClient(array))
        //            {
        //                _portMaster.SerialPortLogger.AddInfo("协商成功");
        //                LoadBackupPortPara();
        //                return true;
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        }
        //        else
        //        {
        //            LoadBackupPortPara();
        //            return false;
        //        }
        //    });
        //}
    }
}