using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MySerialPortMaster;
using NLog;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.Action;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.Association;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.Get;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.Set;
using 三相智慧能源网关调试软件.DLMS.HDLC;
using 三相智慧能源网关调试软件.DLMS.HDLC.Enums;

namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{
    public class DLMSClient : ViewModelBase
    {
        #region 协议层资源
        public EModeViewModel EModeViewModel { get; set; }
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
        public DLMSSettingsViewModel DlmsSettingsViewModel { get; set; }


        /// <summary>
        /// 如何选择物理通道进行发送数据
        /// </summary>
        /// <param name="sendBytes"></param>
        /// <returns></returns>
        private async Task<byte[]> HowToSendDataNew(byte[] sendBytes)
        {
            var returnBytes = new byte[] { };
            if (DlmsSettingsViewModel.CommunicationType == CommunicationType.SerialPort)
            {
                returnBytes = await PortMaster.SendAndReceiveReturnDataAsync(sendBytes);
            }
            else if (DlmsSettingsViewModel.CommunicationType == CommunicationType.FrontEndProcess)
            {
                returnBytes = await Socket.SendDataToClientAndWaitReceiveData(CurrentSocket, sendBytes);
            }

            return returnBytes;
        }

        private  byte[] HowToTakeReplyApduData(byte[] parseBytes)
        {
            if (parseBytes==null|| parseBytes.Length == 0)
            {
                return null;
            }

            var returnPduBytes = new byte[] { };
            if (DlmsSettingsViewModel.CommunicationType == CommunicationType.SerialPort)
            {
                var pduAndFcsBytes = parseBytes.Skip(11).ToArray();
                returnPduBytes = pduAndFcsBytes.Take(pduAndFcsBytes.Length - 3).ToArray();
            }
            else if (DlmsSettingsViewModel.CommunicationType == CommunicationType.FrontEndProcess)
            {
                returnPduBytes = parseBytes.Skip(8).ToArray();
            }

            return returnPduBytes;
        }

        /// <summary>
        /// 初始化，根据不同的通讯协议和物理通道进行初始化请求操作
        /// </summary>
        /// <returns></returns>
        public async Task<byte[]> InitRequest()
        {
            byte[] bytes = null;
            if (DlmsSettingsViewModel.InterfaceType == InterfaceType.HDLC)
            {
                if (DlmsSettingsViewModel.StartProtocolType == StartProtocolType.IEC21E)
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
            else if (DlmsSettingsViewModel.InterfaceType == InterfaceType.WRAPPER)
            {
                bytes = await HowToSendDataNew(NetFrameMaker.AarqRequest());
            }

            return bytes;
        }

        public async Task<bool> Execute21ENegotiate()
        {
             EModeViewModel = new EModeViewModel(PortMaster, "");
            var keepAutoDataReceived = false;
            if (PortMaster.IsAutoDataReceived)
            {
                keepAutoDataReceived = true;
                PortMaster.IsAutoDataReceived = false;
            }

            var is21ENegotiateSucceed = await EModeViewModel.Execute21ENegotiateAsync();
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
                DlmsSettingsViewModel.DlmsInfoFromMeter.ReceiveMaxInfoValue = 128;
                DlmsSettingsViewModel.DlmsInfoFromMeter.TransmitMaxInfoValue = 128;
                DlmsSettingsViewModel.DlmsInfoFromMeter.ReceiveMaxWindowSize = 1;
                DlmsSettingsViewModel.DlmsInfoFromMeter.TransmitMaxWindowSize = 1;
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
            DlmsSettingsViewModel.Connected = ConnectionState.Hdlc;
            return true;
        }

        public async Task<Response> GetRequestAndWaitIncludErrResponse(GetRequest getAttributeBytes)
        {
            var dataResult = await GetRequest(getAttributeBytes);
            Response response = new Response();
            var datare = dataResult.ByteToString("");
            if (response.PduStringInHexConstructor(ref datare))
            {
                if (response.GetResponse != null)
                {
                    response.GetResponse.GetResponseNormal.Result.Data.UpdateDisplayFormat(DlmsSettingsViewModel.OctetStringDisplayFormat,
                        DlmsSettingsViewModel.UInt32ValueDisplayFormat);
                    using (StringWriter stringWriter = new StringWriter())
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(GetResponse));
                        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                        ns.Add("", "");
                        xmlSerializer.Serialize(stringWriter, response.GetResponse, ns);
                        Logger.Trace(dataResult.ByteToString);
                        Logger.Trace(stringWriter);
                        var loggg = ServiceLocator.Current.GetInstance<XMLLogViewModel>();
                        loggg.XmlLog = stringWriter.ToString();
                    }
                }

                if (response.ExceptionResponse != null)
                {
                    using (StringWriter stringWriter = new StringWriter())
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExceptionResponse));
                        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                        ns.Add("", "");
                        xmlSerializer.Serialize(stringWriter, response.ExceptionResponse, ns);
                        Logger.Trace(dataResult.ByteToString);
                        Logger.Trace(stringWriter);
                        var loggg = ServiceLocator.Current.GetInstance<XMLLogViewModel>();
                        loggg.XmlLog = stringWriter.ToString();
                    }
                }
            }
            else
            {
                return null;
            }

            return response;
        }
        public async Task<GetResponse> GetRequestAndWaitResponse(GetRequest getAttributeBytes)
        {
            var dataResult = await GetRequest(getAttributeBytes);
            
            GetResponse getResponse = new GetResponse();
            var data = dataResult.ByteToString("");
            if (getResponse.PduStringInHexConstructor(ref data))
            {
                getResponse.GetResponseNormal.Result.Data.UpdateDisplayFormat(DlmsSettingsViewModel.OctetStringDisplayFormat,
                    DlmsSettingsViewModel.UInt32ValueDisplayFormat);
                using (StringWriter stringWriter = new StringWriter())
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(GetResponse));
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", "");
                    xmlSerializer.Serialize(stringWriter, getResponse, ns);
                    Logger.Trace(dataResult.ByteToString);
                    Logger.Trace(stringWriter);
                    var loggg = ServiceLocator.Current.GetInstance<XMLLogViewModel>();
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
        public async Task<byte[]> ActionRequest(ActionRequest actionRequestNormal)
        {
            return await HandlerSendData((actionRequestNormal.ToPduBytes()));
        }
        private async Task<byte[]> HandlerSendData(byte[] dataBytes)
        {
            byte[] Bytes = { };
            if (DlmsSettingsViewModel.InterfaceType == InterfaceType.HDLC)
            {
                Bytes = await HowToSendDataNew(HdlcFrameMaker.BuildPduRequestBytes(dataBytes));
            }
            else if (DlmsSettingsViewModel.InterfaceType == InterfaceType.WRAPPER)
            {
                Bytes = await HowToSendDataNew(NetFrameMaker.BuildPduRequestBytes(dataBytes));
            }

            Bytes =  HowToTakeReplyApduData(Bytes);
            return Bytes;
        }

       


        public async Task<byte[]> DisconnectRequest(bool force = true)
        {
            byte[] result = null;
            if (force && (DlmsSettingsViewModel.InterfaceType == InterfaceType.HDLC))
            {
                result = await HowToSendDataNew(HdlcFrameMaker.DisconnectRequest());
            }
            else if (force && DlmsSettingsViewModel.InterfaceType == InterfaceType.WRAPPER)
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
            DlmsSettingsViewModel = ServiceLocator.Current.GetInstance<DLMSSettingsViewModel>();
            Socket = ServiceLocator.Current.GetInstance<TcpServerViewModel>().TcpServerHelper;
            PortMaster = ServiceLocator.Current.GetInstance<SerialPortViewModel>().SerialPortMaster;

            HdlcFrameMaker = new HdlcFrameMaker(DlmsSettingsViewModel);
            NetFrameMaker = new NetFrameMaker(DlmsSettingsViewModel);
            EModeViewModel = new EModeViewModel(PortMaster, "");

            InitSerialPortParams(PortMaster);
            
            InitRequestCommand = new RelayCommand(async () => { await InitRequest(); });
            ReleaseRequestCommand = new RelayCommand(async () => { await DisconnectRequest(true); });
        }

        public Task<byte[]> SetEnterUpGradeMode()
        {
            return PortMaster.SendAndReceiveReturnDataAsync(HdlcFrameMaker.SetEnterUpGradeMode(256));
        }

    }
}