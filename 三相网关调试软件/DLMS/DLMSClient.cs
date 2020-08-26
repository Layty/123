using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MySerialPortMaster;
using NLog;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.Action;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.Association;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.Get;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.Set;
using 三相智慧能源网关调试软件.DLMS.HDLC;
using 三相智慧能源网关调试软件.DLMS.HDLC.Enums;
using 三相智慧能源网关调试软件.DLMS.HDLC.IEC21EMode;
using 三相智慧能源网关调试软件.DLMS.Wrapper;
using 三相智慧能源网关调试软件.MyControl;
using 三相智慧能源网关调试软件.ViewModel;

namespace 三相智慧能源网关调试软件.DLMS
{
    public class DLMSClient : ViewModelBase
    {
        #region 协议层资源

        private EModeFrameMaker _eModeFrameFrameMaker;
        public HdlcFrameMaker HdlcFrameMaker { get; set; }
        public NetFrameMaker NetFrameMaker { get; set; }

        #endregion


        #region 物理通道资源

        private SerialPortMaster PortMaster { get; set; }
        private TcpServerHelper Socket { get; set; }
        public Socket CurrentSocket { get; set; }

        #endregion
        public Logger Logger = LogManager.GetLogger("XML");
        public bool IsAuthenticationRequired { get; set; }
        public MyDLMSSettings MyDlmsSettings { get; set; }




       


        /// <summary>
        /// 如何选择物理通道进行发送数据
        /// </summary>
        /// <param name="sendBytes"></param>
        /// <returns></returns>
        private async Task<byte[]> HowToSendDataNew(byte[] sendBytes)
        {
            var returnBytes = new byte[] { };
            if (MyDlmsSettings.CommunicationType == CommunicationType.SerialPort)
            {
                returnBytes = await PortMaster.SendAndReceiveReturnDataAsync(sendBytes);
            }
            else if (MyDlmsSettings.CommunicationType == CommunicationType.FrontEndProcess)
            {
                returnBytes = await Socket.SendDataToClientAndWaitReceiveData(CurrentSocket, sendBytes);
            }

            return returnBytes;
        }

        private  byte[] HowToTakeReplyApduData(byte[] parseBytes)
        {
            if (parseBytes.Length == 0)
            {
                return null;
            }

            var returnPduBytes = new byte[] { };
            if (MyDlmsSettings.CommunicationType == CommunicationType.SerialPort)
            {
                var pduAndFcsBytes = parseBytes.Skip(11).ToArray();
                returnPduBytes = pduAndFcsBytes.Take(pduAndFcsBytes.Length - 3).ToArray();
            }
            else if (MyDlmsSettings.CommunicationType == CommunicationType.FrontEndProcess)
            {
                returnPduBytes = parseBytes.Skip(8).ToArray();
            }

            return returnPduBytes;
        }

        public async Task<byte[]> InitRequestNew()
        {
            byte[] bytes = null;
            if (MyDlmsSettings.InterfaceType == InterfaceType.HDLC)
            {
                if (MyDlmsSettings.StartProtocolType == StartProtocolType.IEC21E)
                {
                    var flag21E = await Execute21ENegotiate();
                    if (flag21E)
                    {
                        //HDLC46
                        bytes = await HowToSendDataNew(HdlcFrameMaker.SNRMRequest());
                        ParseUaResponse(bytes);
                        bytes = await HowToSendDataNew(HdlcFrameMaker.AarqRequest());
                        //解析 Response
                    }
                }
                //HDLC46
                else
                {
                    bytes = await HowToSendDataNew(HdlcFrameMaker.SNRMRequest());
                    ParseUaResponse(bytes);
                    bytes = await HowToSendDataNew(HdlcFrameMaker.AarqRequest());
                    bytes = HowToTakeReplyApduData(bytes);
                    if (bytes!=null)
                    {
                        new AssociationResponse().PduBytesToConstructor(bytes);
                    }
                  
                }
            }
            else if (MyDlmsSettings.InterfaceType == InterfaceType.WRAPPER)
            {
                bytes = await HowToSendDataNew(NetFrameMaker.AarqRequest());
            }

            return bytes;
        }

        public async Task<bool> Execute21ENegotiate()
        {
            var EModeExecutor = new EModeExecutor(PortMaster, "");
            var keepAutoDataReceived = false;
            if (PortMaster.IsAutoDataReceived)
            {
                keepAutoDataReceived = true;
                PortMaster.IsAutoDataReceived = false;
            }

            var is21ENegotiateSucceed = await EModeExecutor.Execute21ENegotiate();
            if (keepAutoDataReceived)
            {
                PortMaster.IsAutoDataReceived = true;
            }

            return is21ENegotiateSucceed;
        }


        public bool ParseUaResponse(byte[] replyData)
        {
            if (replyData.Length == 0)
            {
                MyDlmsSettings.DlmsInfoFromMeter.ReceiveMaxInfoValue = 128;
                MyDlmsSettings.DlmsInfoFromMeter.TransmitMaxInfoValue = 128;
                MyDlmsSettings.DlmsInfoFromMeter.ReceiveMaxWindowSize = 1;
                MyDlmsSettings.DlmsInfoFromMeter.TransmitMaxWindowSize = 1;
                return false;
            }

            if (replyData.First() != 0x7E)
            {
                return false;
            }

            if (replyData.Last() != 0x7E)
            {
                return false;
            }

            var buff = replyData.Skip(8).ToArray();
            var buff1 = (buff.Take(buff.Length - 3).ToArray());
            // Gurux.DLMS.GXDLMS.ParseSnrmUaResponse(data, Settings);
            MyDlmsSettings.Connected = ConnectionState.Hdlc;
            return true;
        }


       

        public async Task<GetResponse> GetRequestAndWaitResponse(byte[] getAttributeBytes)
        {
            var dataResult = await GetRequest(getAttributeBytes);

            GetResponse getResponse = new GetResponse();
            if (getResponse.PduBytesToConstructor(dataResult))
            {
                getResponse.GetResponseNormal.Result.Data.UpdateDisplayFormat(MyDlmsSettings.OctetStringDisplayFormat,
                    MyDlmsSettings.UInt32ValueDisplayFormat);
                using (StringWriter stringWriter = new StringWriter())
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(GetResponse));
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", "");
                    xmlSerializer.Serialize(stringWriter, getResponse, ns);
                    Logger.Trace(dataResult.ByteToString);
                    Logger.Trace(stringWriter);
                    var loggg = ServiceLocator.Current.GetInstance<DMLSXMLLog>();
                    loggg.XmlLog = stringWriter.ToString();
                }
            }

            return getResponse;
        }
        public async Task<GetResponse> GetRequestAndWaitResponse(GetRequest getAttributeBytes)
        {
            var dataResult = await GetRequest(getAttributeBytes);

            GetResponse getResponse = new GetResponse();
            if (getResponse.PduBytesToConstructor(dataResult))
            {
                getResponse.GetResponseNormal.Result.Data.UpdateDisplayFormat(MyDlmsSettings.OctetStringDisplayFormat,
                    MyDlmsSettings.UInt32ValueDisplayFormat);
                using (StringWriter stringWriter = new StringWriter())
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(GetResponse));
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", "");
                    xmlSerializer.Serialize(stringWriter, getResponse, ns);
                    Logger.Trace(dataResult.ByteToString);
                    Logger.Trace(stringWriter);
                    var loggg = ServiceLocator.Current.GetInstance<DMLSXMLLog>();
                    loggg.XmlLog = stringWriter.ToString();
                }
            }
            else
            {
                return null;
            }

            return getResponse;
        }


        public async Task<byte[]> GetRequest(byte[] getAttributeBytes)
        {
            return await HandlerSendData(getAttributeBytes);
        }

        public async Task<byte[]> GetRequest(GetRequest getRequest)
        {
            return await HandlerSendData(getRequest.ToPduBytes());
        }

       

        public async Task<byte[]> SetRequest(byte[] setAttributeBytes)
        {
            return await HandlerSendData(setAttributeBytes);
        }
        public async Task<byte[]> SetRequest(SetRequest setRequest)
        {
            return await HandlerSendData(setRequest.ToPduBytes());
        }
        public async Task<byte[]> ActionRequest(byte[] actionRequestBytes)
        {
            return await HandlerSendData(actionRequestBytes);
        }
        public async Task<byte[]> ActionRequest(ActionRequestNormal actionRequestNormal)
        {
            return await HandlerSendData(actionRequestNormal.ToPduBytes());
        }
        private async Task<byte[]> HandlerSendData(byte[] dataBytes)
        {
            byte[] Bytes = { };
            if (MyDlmsSettings.InterfaceType == InterfaceType.HDLC)
            {
                Bytes = await HowToSendDataNew(HdlcFrameMaker.BuildPduRequestBytes(dataBytes));
            }
            else if (MyDlmsSettings.InterfaceType == InterfaceType.WRAPPER)
            {
                Bytes = await HowToSendDataNew(NetFrameMaker.BuildPduRequestBytes(dataBytes));
            }

            Bytes =  HowToTakeReplyApduData(Bytes);
            return Bytes;
        }

       


        public async Task<byte[]> DisconnectRequest(bool force = true)
        {
            byte[] result = null;
            if (force && (MyDlmsSettings.InterfaceType == InterfaceType.HDLC))
            {
                result = await HowToSendDataNew(HdlcFrameMaker.DisconnectRequest());
            }
            else if (force && MyDlmsSettings.InterfaceType == InterfaceType.WRAPPER)
            {
                result = await HowToSendDataNew(NetFrameMaker.ReleaseRequest());
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
            Socket = ServiceLocator.Current.GetInstance<TcpServerViewModel>().TcpServerHelper;
            PortMaster = ServiceLocator.Current.GetInstance<SerialPortViewModel>().SerialPortMaster;
            InitSerialPortParams(PortMaster);
            InitRequestCommand = new RelayCommand(async () => { await InitRequestNew(); });
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