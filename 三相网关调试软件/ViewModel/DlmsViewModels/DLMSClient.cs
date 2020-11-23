using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using CommonServiceLocator;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using MyDlmsStandard;
using MySerialPortMaster;
using NLog;
using 三相智慧能源网关调试软件.Common;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.Action;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.Association;
using MyDlmsStandard.ApplicationLay.Get;
using MyDlmsStandard.ApplicationLay.Release;
using MyDlmsStandard.ApplicationLay.Set;
using MyDlmsStandard.Ber;
using MyDlmsStandard.HDLC;
using MyDlmsStandard.HDLC.Enums;


namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{
    public class Mission
    {
        private DlmsClient dlmsClient { get;set; }

        public void ReadEnergy(int timeSpan)
        {


        }
    }


    public class DlmsClient : ObservableObject
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
        public DlmsSettingsViewModel DlmsSettingsViewModel { get; set; }


        /// <summary>
        /// 如何选择物理通道进行发送数据
        /// </summary>
        /// <param name="sendBytes"></param>
        /// <returns></returns>
        private async Task<byte[]> PhysicalLayerSendData(byte[] sendBytes)
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

        private byte[] HowToTakeReplyApduData(byte[] parseBytes)
        {
            if (parseBytes == null || parseBytes.Length == 0)
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
                    if (!flag21E)
                    {
                        return null;
                    }
                }

                //HDLC46
                bytes = await PhysicalLayerSendData(HdlcFrameMaker.SNRMRequest());
                if (HdlcFrameMaker.ParseUaResponse(bytes))
                {
                    AssociationRequest aarq = new AssociationRequest(DlmsSettingsViewModel.PasswordHex,
                        DlmsSettingsViewModel.MaxReceivePduSize, DlmsSettingsViewModel.DlmsVersion,
                        DlmsSettingsViewModel.SystemTitle, DlmsSettingsViewModel.ProposedConformance);
                    bytes = await PhysicalLayerSendData(HdlcFrameMaker.InvokeApdu(aarq.ToPduBytes()));
                    bytes = HowToTakeReplyApduData(bytes);
                    if (bytes != null)
                    {
                        var ass = new AssociationResponse();
                        if (ass.PduBytesToConstructor(bytes))
                        {
                            XmlCommon(ass);
                        }
                    }
                }
            }
            else if (DlmsSettingsViewModel.InterfaceType == InterfaceType.WRAPPER)
            {
                AssociationRequest aarq = new AssociationRequest(DlmsSettingsViewModel.PasswordHex,
                    DlmsSettingsViewModel.MaxReceivePduSize, DlmsSettingsViewModel.DlmsVersion,
                    DlmsSettingsViewModel.SystemTitle, DlmsSettingsViewModel.ProposedConformance);
                XmlCommon(aarq);
                bytes = await PhysicalLayerSendData(NetFrameMaker.InvokeApdu(aarq.ToPduBytes()));
                if (bytes != null)
                {
                    var ass = new AssociationResponse();
                    if (ass.PduBytesToConstructor(bytes))
                    {
                        XmlCommon(ass);
                    }
                }
            }

            return bytes;
        }

        public async Task<bool> Execute21ENegotiate()
        {
            EModeViewModel = new EModeViewModel(PortMaster);
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


        public async Task<GetResponse> GetRequestAndWaitResponse(CosemAttributeDescriptor cosemAttributeDescriptor,
            GetRequestType getRequestType = GetRequestType.Normal)
        {
            switch (getRequestType)
            {
                case GetRequestType.Normal:
                    getRequest.GetRequestNormal = new GetRequestNormal(cosemAttributeDescriptor);
                    break;
                case GetRequestType.Next: 
                    getRequest.GetRequestNext=new GetRequestNext();
                    break;
                case GetRequestType.WithList:
                  
                   getRequest.GetRequestWithList=new GetRequestWithList();
                    break;
            }

            XmlCommon(getRequest);
            var dataResult = await HandlerSendData(getRequest.ToPduStringInHex());
            return HandleGetResponse(dataResult);
        }

        public async Task<GetResponse> GetRequestAndWaitResponse(
            CosemAttributeDescriptorWithSelection cosemAttributeDescriptorWithSelection)
        {
            getRequest.GetRequestNormal = new GetRequestNormal(cosemAttributeDescriptorWithSelection);
            XmlCommon(getRequest);
            var dataResult = await HandlerSendData(getRequest.ToPduStringInHex());
            return HandleGetResponse(dataResult);
        }

        public async Task<byte[]> GetRequest(CosemAttributeDescriptor cosemAttributeDescriptor)
        {
            getRequest.GetRequestNormal = new GetRequestNormal(cosemAttributeDescriptor);
            return await HandlerSendData(getRequest.ToPduStringInHex());
        }

        private void XmlCommon<T>(T t)
        {
            using (StringWriter stringWriter = new StringWriter())
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", ""); //去掉Namespaces
                //OmitXmlDeclaration = true 省略XML声明
                XmlWriterSettings settings = new XmlWriterSettings
                    {OmitXmlDeclaration = true, Indent = true, Encoding = Encoding.UTF8};
                using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings))
                {
                    xmlSerializer.Serialize(xmlWriter, t, ns);
                    var log = ServiceLocator.Current.GetInstance<XMLLogViewModel>();
                    log.XmlLog = stringWriter + Environment.NewLine + "-----萌萌哒分割线-----\r\n";
                }
            }
        }

        private GetResponse HandleGetResponse(byte[] dataResult)
        {
            GetResponse getResponse = new GetResponse();
            var data = dataResult.ByteToString("");
            if (!getResponse.PduStringInHexConstructor(ref data))
            {
                return null;
            }

            XmlCommon(getResponse);
            if (getResponse.GetResponseNormal.Result.DataAccessResult.Value == "00")
            {
                getResponse.GetResponseNormal.Result.Data.UpdateDisplayFormat();
            }

            return getResponse;
        }


        private SetResponse HandleSetResponse(byte[] dataResult)
        {
            SetResponse setResponse = new SetResponse();
            string d = dataResult.ByteToString("");

            if (!setResponse.PduStringInHexConstructor(ref d))
            {
                return null;
            }

            XmlCommon(setResponse);
            return setResponse;
        }

        public async Task<SetResponse> SetRequestAndWaitResponse(CosemAttributeDescriptor cosemAttributeDescriptor,
            DlmsDataItem value)
        {
            setRequest = new SetRequest {SetRequestNormal = new SetRequestNormal(cosemAttributeDescriptor, value)};
            var dataResult = await HandlerSendData(setRequest.ToPduStringInHex());
            return HandleSetResponse(dataResult);
        }

        public async Task<byte[]> ActionRequest(ActionRequest actionRequestNormal)
        {
            return await HandlerSendData((MyDlmsStandard.Common.Common.StringToByte(actionRequestNormal.ToPduStringInHex())));
        }

        private async Task<byte[]> HandlerSendData(byte[] dataBytes)
        {
            byte[] bytes = { };
            if (DlmsSettingsViewModel.InterfaceType == InterfaceType.HDLC)
            {
                bytes = await PhysicalLayerSendData(HdlcFrameMaker.InvokeApdu(dataBytes));
            }
            else if (DlmsSettingsViewModel.InterfaceType == InterfaceType.WRAPPER)
            {
                bytes = await PhysicalLayerSendData(NetFrameMaker.InvokeApdu(dataBytes));
            }

            bytes = HowToTakeReplyApduData(bytes);
            return bytes;
        }
        private async Task<byte[]> HandlerSendData(string dataHexString)
        {
            byte[] bytes = { };
            if (DlmsSettingsViewModel.InterfaceType == InterfaceType.HDLC)
            {
                bytes = await PhysicalLayerSendData(HdlcFrameMaker.InvokeApdu(dataHexString.StringToByte()));
            }
            else if (DlmsSettingsViewModel.InterfaceType == InterfaceType.WRAPPER)
            {
                bytes = await PhysicalLayerSendData(NetFrameMaker.InvokeApdu(dataHexString.StringToByte()));
            }

            bytes = HowToTakeReplyApduData(bytes);
            return bytes;
        }


        public async Task<byte[]> ReleaseRequest(bool force = true)
        {
            byte[] result = null;
            var re = new ReleaseRequest();
            re.Reason = new BerInteger() {Value = "00"};
            re.UserInformation = new UserInformation()
            {
                InitiateRequest = new InitiateRequest(DlmsSettingsViewModel.MaxReceivePduSize,
                    DlmsSettingsViewModel.DlmsVersion, DlmsSettingsViewModel.ProposedConformance)
            };
            var releaseBytes = Common.Common.StringToByte(re.ToPduStringInHex());
            if (force && (DlmsSettingsViewModel.InterfaceType == InterfaceType.HDLC))
            {
                result = await PhysicalLayerSendData(HdlcFrameMaker.InvokeApdu(releaseBytes));
                result = await PhysicalLayerSendData(HdlcFrameMaker.DisconnectRequest());
                //TODO :ParseUA
            }
            else if (force && DlmsSettingsViewModel.InterfaceType == InterfaceType.WRAPPER)
            {
                result = await PhysicalLayerSendData(NetFrameMaker.InvokeApdu(releaseBytes));
            }
            //TODO Parse ReleaseResponse

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


        public RelayCommand InitRequestCommand { get; set; }

        public RelayCommand ReleaseRequestCommand { get; set; }

        private GetRequest getRequest { get; set; }

        public SetRequest setRequest { get; set; }
        public ActionRequest actionRequest { get; set; }

        public DlmsClient()
        {
            DlmsSettingsViewModel = ServiceLocator.Current.GetInstance<DlmsSettingsViewModel>();
            Socket = ServiceLocator.Current.GetInstance<TcpServerViewModel>().TcpServerHelper;

            PortMaster = ServiceLocator.Current.GetInstance<SerialPortViewModel>().SerialPortMaster;

            HdlcFrameMaker = new HdlcFrameMaker(DlmsSettingsViewModel.ServerAddress,(byte)DlmsSettingsViewModel.ClientAddress,DlmsSettingsViewModel.DlmsInfo);
            NetFrameMaker = new NetFrameMaker(DlmsSettingsViewModel);
            EModeViewModel = new EModeViewModel(PortMaster);

            InitSerialPortParams(PortMaster);


            InitRequestCommand = new RelayCommand(async () => { await InitRequest(); });
            ReleaseRequestCommand = new RelayCommand(async () => { await ReleaseRequest(true); });
            getRequest = new GetRequest();
            setRequest = new SetRequest();
            actionRequest = new ActionRequest();
        }

        public async void ActionEnergy()
        {
           await InitRequest();
//            await GetRequestAndWaitResponse()
        }
        public Task<byte[]> SetEnterUpGradeMode()
        {
            return PortMaster.SendAndReceiveReturnDataAsync(HdlcFrameMaker.SetEnterUpGradeMode(256));
        }
    }
}