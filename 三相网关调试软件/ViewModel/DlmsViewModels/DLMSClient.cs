using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using CommonServiceLocator;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using MyDlmsStandard;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.Association;
using MyDlmsStandard.ApplicationLay.Get;
using MyDlmsStandard.ApplicationLay.Set;
using MyDlmsStandard.ApplicationLay.Action;
using MyDlmsStandard.ApplicationLay.Release;
using MyDlmsStandard.Ber;
using MyDlmsStandard.HDLC;
using MyDlmsStandard.HDLC.Enums;
using MyDlmsStandard.Wrapper;
using MySerialPortMaster;
using 三相智慧能源网关调试软件.Common;
using MyDlmsStandard.Axdr;

namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{
    /// <summary>
    /// 数据层
    /// </summary>
    public class SendData : IToPduStringInHex
    {
        public SendData(IToPduStringInHex handlerData)
        {
            HandlerHexData = handlerData;
        }

        private IToPduStringInHex HandlerHexData;

        public string ToPduStringInHex()
        {
            return HandlerHexData.ToPduStringInHex();
        }
    }

    public class DlmsClient : ObservableObject
    {
        #region 协议层资源

        public EModeViewModel EModeViewModel { get; set; }

        public Hdlc46FrameBase Hdlc46FrameBase { get; set; }
        public WrapperFrame WrapperFrame { get; set; }

        #endregion


        #region 物理通道资源

        /// <summary>
        /// 串口资源
        /// </summary>
        private SerialPortMaster PortMaster { get; set; }

        /// <summary>
        /// 网络资源
        /// </summary>
        public TcpServerHelper Socket { get; set; }

        /// <summary>
        /// 标识当前的Socket链接
        /// </summary>
        public Socket CurrentSocket { get; set; }

        #endregion

        protected void OnReportSnackbar(string message)
        {
            StrongReferenceMessenger.Default.Send(message, "Snackbar");
        }

        public bool IsAuthenticationRequired { get; set; }

        /// <summary>
        /// DlmsSettings配置相关
        /// </summary>
        public DlmsSettingsViewModel DlmsSettingsViewModel { get; set; }

        public CancellationTokenSource CancellationTokenSource { get; set; }

        /// <summary>
        /// 用于取消长时间通讯
        /// </summary>
        public RelayCommand CancelCommand { get; set; }

        #region 发送数据 //协议层=>物理层

        

        private async Task<byte[]> PhysicalLayerSendData(IToPduStringInHex sendHexString)
        {
            return await PhysicalLayerSendData(sendHexString.ToPduStringInHex());
        }

        private async Task<byte[]> PhysicalLayerSendData(string sendHexString)
        {
            return await PhysicalLayerSendData(sendHexString.StringToByte());
        }

        /// <summary>
        /// 如何选择物理通道进行发送数据
        /// </summary>
        /// <param name="sendBytes"></param>
        /// <returns></returns>
        private async Task<byte[]> PhysicalLayerSendData(byte[] sendBytes)
        {
            var returnBytes = new byte[] { };
            try
            {
                if (DlmsSettingsViewModel.CommunicationType == ChanelType.SerialPort)
                {
                    returnBytes = await PortMaster.SendAndReceiveReturnDataAsync(sendBytes);
                }
                else if (DlmsSettingsViewModel.CommunicationType == ChanelType.FrontEndProcess)
                {
                    returnBytes = await Socket.SendDataToClientAndWaitReceiveDataAsync(CurrentSocket, sendBytes);
                }
            }
            catch (Exception e)
            {
                OnReportSnackbar(e.Message);
            }


            return returnBytes;
        }

        #endregion


        /// <summary>
        /// 物理层得到数据，再给到协议层进行解析
        /// </summary>
        /// <param name="parseBytes"></param>
        /// <returns></returns>
        private byte[] HowToTakeReplyApduData(byte[] parseBytes)
        {
            if (parseBytes == null || parseBytes.Length == 0)
            {
                OnReportSnackbar("未收到响应帧");
                return null;
            }

            var returnPduBytes = new byte[] { };
            if (DlmsSettingsViewModel.CommunicationType == ChanelType.SerialPort)
            {
                var pstring = parseBytes.ByteToString();
                if (Hdlc46FrameBase.PduStringInHexConstructor(ref pstring))
                {
                    returnPduBytes = Hdlc46FrameBase.Apdu;
                }
            }
            else if (DlmsSettingsViewModel.CommunicationType == ChanelType.FrontEndProcess)
            {
                var wrapperFrame = Wrapper47FrameFactory.CreateWrapperFrame(parseBytes);
                if (wrapperFrame != null)
                {
                    returnPduBytes = wrapperFrame.WrapperBody.DataBytes;
                }
            }

            return returnPduBytes;
        }

        /// <summary>
        /// 初始化，根据不同的通讯协议和物理通道进行初始化请求操作
        /// </summary>
        /// <returns></returns>
        public async Task<bool> InitRequest()
        {
            bool initResult = false;
            if (DlmsSettingsViewModel.InterfaceType == InterfaceType.HDLC)
            {
                //21E协商
                if (DlmsSettingsViewModel.StartProtocolType == StartProtocolType.IEC21E)
                {
                    var flag21E = await Execute21ENegotiate();
                    if (!flag21E)
                    {
                        StrongReferenceMessenger.Default.Send("21E协商失败", "Snackbar");
                        return false;
                    }
                }
                else if (DlmsSettingsViewModel.StartProtocolType == StartProtocolType.DLMS)
                {
                    //SNRM
                    var snrmRequest = new SNRMRequest((byte) (DlmsSettingsViewModel.ServerAddress),
                        (byte) DlmsSettingsViewModel.ClientAddress, DlmsSettingsViewModel.DlmsInfo);
                    //HDLC46
                    byte[] bytes = await PhysicalLayerSendData(snrmRequest);
                    if (Hdlc46FrameBase.ParseUaResponse(bytes))
                    {
                        //AARQ
                        Hdlc46FrameBase = new Hdlc46FrameBase((byte) (DlmsSettingsViewModel.ServerAddress),
                            (byte) DlmsSettingsViewModel.ClientAddress, DlmsSettingsViewModel.DlmsInfo);
                        AssociationRequest aarq = new AssociationRequest(DlmsSettingsViewModel.PasswordHex,
                            DlmsSettingsViewModel.MaxReceivePduSize, DlmsSettingsViewModel.DlmsVersion,
                            DlmsSettingsViewModel.SystemTitle, DlmsSettingsViewModel.ProposedConformance);
                        Hdlc46FrameBase.Apdu = aarq.ToPduStringInHex().StringToByte();
                        XmlHelper.XmlCommon(aarq);
                        bytes = await ProtocolHandlerApduDataThenReturnApdu(aarq);

                        if (bytes != null)
                        {
                            var result = bytes.ByteToString();
                            var ass = new AssociationResponse();
                            if (ass.PduStringInHexConstructor(ref result))
                            {
                                initResult = true;
                                XmlHelper.XmlCommon(ass);
                            }
                        }
                    }
                    else
                    {
                        StrongReferenceMessenger.Default.Send("HDLC失败", "Snackbar");
                        return false;
                    }
                }
            }
            else if (DlmsSettingsViewModel.InterfaceType == InterfaceType.WRAPPER)
            {
                AssociationRequest aarq = new AssociationRequest(DlmsSettingsViewModel.PasswordHex,
                    DlmsSettingsViewModel.MaxReceivePduSize, DlmsSettingsViewModel.DlmsVersion,
                    DlmsSettingsViewModel.SystemTitle, DlmsSettingsViewModel.ProposedConformance);
                XmlHelper.XmlCommon(aarq);
                var wrapperHeader = new WrapperHeader()
                {
                    Version = new AxdrIntegerUnsigned16("1"),
                    SourceAddress = new AxdrIntegerUnsigned16(DlmsSettingsViewModel.ClientAddress.ToString("X4")),
                    DestAddress = new AxdrIntegerUnsigned16(DlmsSettingsViewModel.ServerAddress.ToString("X4")),
                };
                WrapperFrame = new WrapperFrame(wrapperHeader, aarq);
                byte[] bytes = await ProtocolHandlerApduDataThenReturnApdu(WrapperFrame);
                if (bytes != null && bytes.Length != 0)
                {
                    var result = MyDlmsStandard.Common.Common.ByteToString(bytes);
                    var associationResponse = new AssociationResponse();
                    if (associationResponse.PduStringInHexConstructor(ref result))
                    {
                        initResult = true;
                        XmlHelper.XmlCommon(associationResponse);
                    }
                }
            }

            return initResult;
        }

        /// <summary>
        /// 执行21E协商
        /// </summary>
        /// <returns></returns>
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


        public async Task<List<GetResponse>> GetRequestAndWaitResponseArray(
            CosemAttributeDescriptor cosemAttributeDescriptor, GetRequestType getRequestType = GetRequestType.Normal)
        {
            List<GetResponse> getResponses = new List<GetResponse>();
            GetRequest = new GetRequest();

            switch (getRequestType)
            {
                case GetRequestType.Normal:
                    GetRequest.GetRequestNormal = new GetRequestNormal(cosemAttributeDescriptor);
                    break;
                case GetRequestType.Next:
                    GetRequest.GetRequestNext = new GetRequestNext();
                    break;
                case GetRequestType.WithList:
                    GetRequest.GetRequestWithList = new GetRequestWithList();
                    break;
            }

            XmlHelper.XmlCommon(GetRequest);
            var dataResult = await ProtocolHandlerApduDataThenReturnApdu(GetRequest);
            var re = HandleGetResponse(dataResult);
            if (re?.GetResponseNormal != null)
            {
                getResponses.Add(re);
                return getResponses;
            }


            await HowToHandleBlockNumber(getResponses, re);

            return getResponses;
        }

        public async Task<GetResponse> GetRequestAndWaitResponse(CosemAttributeDescriptor cosemAttributeDescriptor,
            GetRequestType getRequestType = GetRequestType.Normal)
        {
            GetRequest = new GetRequest();

            switch (getRequestType)
            {
                case GetRequestType.Normal:
                    GetRequest.GetRequestNormal = new GetRequestNormal(cosemAttributeDescriptor);
                    break;
                case GetRequestType.Next:
                    GetRequest.GetRequestNext = new GetRequestNext();
                    break;
                case GetRequestType.WithList:
                    GetRequest.GetRequestWithList = new GetRequestWithList();
                    break;
            }

            XmlHelper.XmlCommon(GetRequest);
            var dataResult = await ProtocolHandlerApduDataThenReturnApdu(GetRequest);
            return HandleGetResponse(dataResult);
        }

        public async Task<GetResponse> GetRequestAndWaitResponse(
            CosemAttributeDescriptorWithSelection cosemAttributeDescriptorWithSelection,
            GetRequestType getRequestType = GetRequestType.Normal)
        {
            switch (getRequestType)
            {
                case GetRequestType.Normal:
                    GetRequest.GetRequestNormal = new GetRequestNormal(cosemAttributeDescriptorWithSelection);
                    break;
                case GetRequestType.Next:
                    GetRequest.GetRequestNext = new GetRequestNext();
                    break;
                case GetRequestType.WithList:
                    GetRequest.GetRequestWithList = new GetRequestWithList();
                    break;
            }

            XmlHelper.XmlCommon(GetRequest);
            var dataResult = await ProtocolHandlerApduDataThenReturnApdu(GetRequest);
            return HandleGetResponse(dataResult);
        }

        public async Task<List<GetResponse>> GetRequestAndWaitResponseArray(
            CosemAttributeDescriptorWithSelection cosemAttributeDescriptorWithSelection,
            GetRequestType getRequestType = GetRequestType.Normal)
        {
            List<GetResponse> getResponses = new List<GetResponse>();
            switch (getRequestType)
            {
                case GetRequestType.Normal:
                    GetRequest.GetRequestNormal = new GetRequestNormal(cosemAttributeDescriptorWithSelection);
                    break;
                case GetRequestType.Next:
                    GetRequest.GetRequestNext = new GetRequestNext();
                    break;
                case GetRequestType.WithList:
                    GetRequest.GetRequestWithList = new GetRequestWithList();
                    break;
            }

            XmlHelper.XmlCommon(GetRequest);
            var dataResult = await ProtocolHandlerSendData(GetRequest.ToPduStringInHex());

            var re = HandleGetResponse(dataResult);
            if (re?.GetResponseNormal != null)
            {
                getResponses.Add(re);
                return getResponses;
            }


            await HowToHandleBlockNumber(getResponses, re);

            return getResponses;
        }


        private async Task HowToHandleBlockNumber(List<GetResponse> list, GetResponse response)
        {
            if (CancellationTokenSource.Token.IsCancellationRequested)
            {
                return;
            }

            if (response?.GetResponseWithDataBlock != null)
            {
                if (response.GetResponseWithDataBlock.DataBlockG.LastBlock.Value == "00")
                {
                    list.Add(response);
                    var blockNumber = response.GetResponseWithDataBlock.DataBlockG.BlockNumber;
                    GetRequest = new GetRequest
                    {
                        GetRequestNext = new GetRequestNext() {BlockNumber = blockNumber}
                    };
                    var dataGetRequestNextResult = await ProtocolHandlerApduDataThenReturnApdu(GetRequest);
                    var re = HandleGetResponse(dataGetRequestNextResult);
                    await HowToHandleBlockNumber(list, re);
                }
                else if (response.GetResponseWithDataBlock.DataBlockG.LastBlock.Value == "01")
                {
                    list.Add(response);
                }
            }
        }


        private GetResponse HandleGetResponse(byte[] dataResult)
        {
            GetResponse getResponse = new GetResponse();
            var data = dataResult.ByteToString();
            if (!getResponse.PduStringInHexConstructor(ref data))
            {
                OnReportSnackbar("解析响应帧失败");
                return null;
            }

            XmlHelper.XmlCommon(getResponse);

            return getResponse;
        }


        private SetResponse HandleSetResponse(byte[] dataResult)
        {
            SetResponse setResponse = new SetResponse();
            string d = dataResult.ByteToString();

            if (!setResponse.PduStringInHexConstructor(ref d))
            {
                return null;
            }

            XmlHelper.XmlCommon(setResponse);
            return setResponse;
        }

        public async Task<SetResponse> SetRequestAndWaitResponse(CosemAttributeDescriptor cosemAttributeDescriptor,
            DlmsDataItem value)
        {
            var setRequest = new SetRequest {SetRequestNormal = new SetRequestNormal(cosemAttributeDescriptor, value)};
            XmlHelper.XmlCommon(setRequest);
            var dataResult = await ProtocolHandlerApduDataThenReturnApdu(setRequest);
            return HandleSetResponse(dataResult);
        }


        public async Task<byte[]> ActionRequestAndWaitResponse(CosemMethodDescriptor cosemMethodDescriptor,
            DlmsDataItem dlmsDataItem)
        {
            var actionRequest = new ActionRequest()
            {
                ActionRequestNormal = new ActionRequestNormal(cosemMethodDescriptor,
                    dlmsDataItem)
            };
            XmlHelper.XmlCommon(actionRequest);
            return await ProtocolHandlerSendData(actionRequest.ToPduStringInHex());
        }


        SendData sendData { get; set; }

        /// <summary>
        /// 根据是HDLC46协议还是以太网47协议进行数据封装，进而再进一步交给物理层处理数据
        /// </summary>
        /// <param name="dataBytes"></param>
        /// <returns></returns>
        private async Task<byte[]> ProtocolHandlerSendData(byte[] dataBytes)
        {
            byte[] bytes = { };
            if (DlmsSettingsViewModel.InterfaceType == InterfaceType.HDLC)
            {
                Hdlc46FrameBase.Apdu = dataBytes;
                sendData = new SendData(Hdlc46FrameBase);
            }
            else if (DlmsSettingsViewModel.InterfaceType == InterfaceType.WRAPPER)
            {
                WrapperFrame.WrapperBody.DataBytes = dataBytes;
                sendData = new SendData(WrapperFrame);
            }

            bytes = await PhysicalLayerSendData(sendData);

            bytes = HowToTakeReplyApduData(bytes);
            return bytes;
        }

        private async Task<byte[]> ProtocolHandlerSendData(string dataHexString)
        {
            return await ProtocolHandlerSendData(dataHexString.StringToByte());
        }

        private async Task<byte[]> ProtocolHandlerApduDataThenReturnApdu(IToPduStringInHex pduStringInHex)
        {
            if (pduStringInHex is DisConnectRequest dis)
            {
            }

            return await ProtocolHandlerSendData(pduStringInHex.ToPduStringInHex());
        }


        public async Task<bool> ReleaseRequest(bool force = true)
        {
            byte[] result;

            if (force && (DlmsSettingsViewModel.InterfaceType == InterfaceType.HDLC))
            {
                Hdlc46FrameBase = new DisConnectRequest(1, 1);

                result = await PhysicalLayerSendData(Hdlc46FrameBase);
                //TODO :ParseUA
                return Hdlc46FrameBase.ParseUaResponse(result);
            }
            else if (force && DlmsSettingsViewModel.InterfaceType == InterfaceType.WRAPPER)
            {
                var re = new ReleaseRequest
                {
                    Reason = new BerInteger() {Value = "00"},
                    UserInformation = new UserInformation()
                    {
                        InitiateRequest = new InitiateRequest(DlmsSettingsViewModel.MaxReceivePduSize,
                            DlmsSettingsViewModel.DlmsVersion, DlmsSettingsViewModel.ProposedConformance)
                    }
                };
                XmlHelper.XmlCommon(re);
                var releaseBytes = re.ToPduStringInHex().StringToByte();
                WrapperFrame.WrapperBody.DataBytes = releaseBytes;
                result = await ProtocolHandlerApduDataThenReturnApdu(WrapperFrame);
                ReleaseResponse releaseResponse = new ReleaseResponse();
                var release = result.ByteToString();
                if (releaseResponse.PduStringInHexConstructor(ref release))
                {
                    XmlHelper.XmlCommon(releaseResponse);
                }

            }

            // await PhysicalLayerSendData(releaseBytes);
            // TODO Parse ReleaseResponse  应该使用解析Response中的方法

            return true;
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

        private GetRequest GetRequest { get; set; }
        private AssociationRequest AssociationRequest { get; set; }

        public void InitDlmsClient()
        {
            InitSerialPortParams(PortMaster);

            AssociationRequest = new AssociationRequest(DlmsSettingsViewModel.PasswordHex,
                DlmsSettingsViewModel.MaxReceivePduSize, DlmsSettingsViewModel.DlmsVersion,
                DlmsSettingsViewModel.SystemTitle, DlmsSettingsViewModel.ProposedConformance);
        }

        public DlmsClient()
        {
            DlmsSettingsViewModel = ServiceLocator.Current.GetInstance<DlmsSettingsViewModel>();
            Socket = ServiceLocator.Current.GetInstance<TcpServerViewModel>().TcpServerHelper;

            PortMaster = ServiceLocator.Current.GetInstance<SerialPortViewModel>().SerialPortMaster;

            Hdlc46FrameBase = new Hdlc46FrameBase(DlmsSettingsViewModel.ServerAddress,
                (byte) DlmsSettingsViewModel.ClientAddress, DlmsSettingsViewModel.DlmsInfo);

            EModeViewModel = new EModeViewModel(PortMaster);

            Protocol protocol = new Protocol(DlmsSettingsViewModel);

            InitDlmsClient();


            InitRequestCommand = new RelayCommand(async () => { await InitRequest(); });
            ReleaseRequestCommand = new RelayCommand(async () => { await ReleaseRequest(); });
            GetRequest = new GetRequest();
            CancellationTokenSource = new CancellationTokenSource();

            CancelCommand = new RelayCommand(async () =>
            {
                CancellationTokenSource.Cancel();
                await Task.Delay(2000);
                CancellationTokenSource = new CancellationTokenSource();
            });
        }

        /// <summary>
        /// 进入基表的升级模式，写256
        /// </summary>
        /// <returns></returns>
        public Task<byte[]> SetEnterUpGradeMode()
        {
            //TODO 

            return PortMaster.SendAndReceiveReturnDataAsync(Hdlc46FrameBase.SetEnterUpGradeMode(256));
        }
    }
}