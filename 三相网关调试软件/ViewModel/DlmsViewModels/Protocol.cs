using MyDlmsStandard;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.Action;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.Association;
using MyDlmsStandard.ApplicationLay.Get;
using MyDlmsStandard.ApplicationLay.Release;
using MyDlmsStandard.ApplicationLay.Set;
using MyDlmsStandard.Axdr;
using MyDlmsStandard.Ber;
using MyDlmsStandard.HDLC;
using MyDlmsStandard.HDLC.Enums;
using MyDlmsStandard.HDLC.IEC21EMode;
using MyDlmsStandard.Wrapper;
using MySerialPortMaster;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using 三相智慧能源网关调试软件.Common;

namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{  /// <summary>
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
    /// <summary>
    ///协议层
    /// </summary>
    public class Protocol
    {
        public InterfaceType InterfaceType { get; set; }
        public Hdlc46FrameBase Hdlc46FrameBase { get; set; }
        public WrapperFrame WrapperFrame { get; set; }
        public EModeFrame EModeFrame { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
        public StartProtocolType StartProtocolType { get; set; }
        public SendData sendData { get; set; }
        public DlmsSettingsViewModel DlmsSettingsViewModel { get; set; }
        LinkLayer LinkLayer = new LinkLayer();
        public Protocol(DlmsSettingsViewModel dlmsSettingsViewModel)
        {
            CancellationTokenSource = new CancellationTokenSource();
            DlmsSettingsViewModel = dlmsSettingsViewModel;
               InterfaceType = dlmsSettingsViewModel.InterfaceType;
            StartProtocolType = dlmsSettingsViewModel.StartProtocolType;
            Hdlc46FrameBase = new Hdlc46FrameBase(dlmsSettingsViewModel.ServerAddress, (byte)dlmsSettingsViewModel.ClientAddress, dlmsSettingsViewModel.DlmsInfo);
            var wrapperHeader = new WrapperHeader()
            {
                Version = new AxdrIntegerUnsigned16("1"),
                SourceAddress = new AxdrIntegerUnsigned16(dlmsSettingsViewModel.ClientAddress.ToString("X4")),
                DestAddress = new AxdrIntegerUnsigned16(dlmsSettingsViewModel.ServerAddress.ToString("X4")),
            }; WrapperFrame = new WrapperFrame()
            {
                WrapperHeader = wrapperHeader
            };
            EModeFrame = new EModeFrame(dlmsSettingsViewModel.NegotiateBaud);
        }

        /// <summary>
        /// 处理协议的初始化
        /// </summary>
        /// <returns></returns>
        public async Task<bool> InitAsync()
        {
            bool initResult = false;
            if (InterfaceType == InterfaceType.HDLC)
            {
                if (StartProtocolType == StartProtocolType.IEC21E)
                {
                    var flag21E = await Execute21ENegotiateAsync();
                    if (!flag21E)
                    {
                       // StrongReferenceMessenger.Default.Send("21E协商失败", "Snackbar");
                        return false;
                    }
                }
                else if (StartProtocolType == StartProtocolType.DLMS)
                { //SNRM
                    var snrmRequest = new SNRMRequest((byte)(DlmsSettingsViewModel.ServerAddress),
        (byte)DlmsSettingsViewModel.ClientAddress, DlmsSettingsViewModel.DlmsInfo);
                    byte[] bytes = await LinkLayer.SendAsync(snrmRequest.ToPduStringInHex());
                    if (Hdlc46FrameBase.ParseUaResponse(bytes))
                    {
                        //AARQ
                        Hdlc46FrameBase = new Hdlc46FrameBase((byte)(DlmsSettingsViewModel.ServerAddress),
                            (byte)DlmsSettingsViewModel.ClientAddress, DlmsSettingsViewModel.DlmsInfo);
                        AssociationRequest aarq = new AssociationRequest(DlmsSettingsViewModel.PasswordHex,
                            DlmsSettingsViewModel.MaxReceivePduSize, DlmsSettingsViewModel.DlmsVersion,
                            DlmsSettingsViewModel.SystemTitle, DlmsSettingsViewModel.ProposedConformance);
                        Hdlc46FrameBase.Apdu = aarq.ToPduStringInHex().StringToByte();
                        XmlHelper.XmlCommon(aarq);
                        bytes = await LinkLayer.SendAsync(BuildApduData(aarq));
                        bytes = ParseReplyApduData(bytes);
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
                }
            }
            else if (DlmsSettingsViewModel.InterfaceType == InterfaceType.WRAPPER)
            {
                AssociationRequest aarq = new AssociationRequest(DlmsSettingsViewModel.PasswordHex,
                    DlmsSettingsViewModel.MaxReceivePduSize, DlmsSettingsViewModel.DlmsVersion,
                    DlmsSettingsViewModel.SystemTitle, DlmsSettingsViewModel.ProposedConformance);
                XmlHelper.XmlCommon(aarq);
                byte[] bytes = await LinkLayer.SendAsync(BuildApduData(aarq));
                if (bytes != null && bytes.Length != 0)
                {
                    var result = ParseReplyApduData(bytes).ByteToString();
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

        public async Task<bool> ReleaseRequestAsync(bool force = true)
        {
            byte[] result;

            if (force && (DlmsSettingsViewModel.InterfaceType == InterfaceType.HDLC))
            {
                result = await LinkLayer.SendAsync(BuildApduData(new DisConnectRequest(1, 1)));
                //TODO :ParseUA
                return Hdlc46FrameBase.ParseUaResponse(result);
            }
            else if (force && DlmsSettingsViewModel.InterfaceType == InterfaceType.WRAPPER)
            {
                var re = new ReleaseRequest
                {
                    Reason = new BerInteger() { Value = "00" },
                    UserInformation = new UserInformation()
                    {
                        InitiateRequest = new InitiateRequest(DlmsSettingsViewModel.MaxReceivePduSize,
                            DlmsSettingsViewModel.DlmsVersion, DlmsSettingsViewModel.ProposedConformance)
                    }
                };
                XmlHelper.XmlCommon(re);
                var bytes = await LinkLayer.SendAsync(BuildApduData(re));

            }

            // await PhysicalLayerSendData(releaseBytes);
            // TODO Parse ReleaseResponse  应该使用解析Response中的方法

            return true;
        }


        public async Task<GetResponse> GetRequestAndWaitResponse(CosemAttributeDescriptor cosemAttributeDescriptor,
       GetRequestType getRequestType = GetRequestType.Normal)
        {
            var GetRequest = new GetRequest();

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
            var dataResult = await LinkLayer.SendAsync(BuildApduData(GetRequest));
            var replyApdu = ParseReplyApduData(dataResult);
            return HandleGetResponse(replyApdu);
        }

        public async Task<List<GetResponse>> GetRequestAndWaitResponseArray(
           CosemAttributeDescriptor cosemAttributeDescriptor, GetRequestType getRequestType = GetRequestType.Normal)
        {
          
            List<GetResponse> getResponses = new List<GetResponse>();
        var    GetRequest = new GetRequest();

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
            var dataResult = await LinkLayer.SendAsync(BuildApduData(GetRequest));
            var replyApdu = ParseReplyApduData(dataResult);
            var re = HandleGetResponse(replyApdu);
         
            if (re?.GetResponseNormal != null)
            {
                getResponses.Add(re);
                return getResponses;
            }


            await HowToHandleBlockNumber(getResponses, re);

            return getResponses;
        }
        public async Task<GetResponse> GetRequestAndWaitResponse(
             CosemAttributeDescriptorWithSelection cosemAttributeDescriptorWithSelection,
             GetRequestType getRequestType = GetRequestType.Normal)
        {
            var GetRequest = new GetRequest();
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
            var dataResult = await LinkLayer.SendAsync(BuildApduData(GetRequest));
            return HandleGetResponse(dataResult);
        }


        public async Task<List<GetResponse>> GetRequestAndWaitResponseArray(
        CosemAttributeDescriptorWithSelection cosemAttributeDescriptorWithSelection,
        GetRequestType getRequestType = GetRequestType.Normal)
        {
            var GetRequest = new GetRequest();
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
            var dataResult = await LinkLayer.SendAsync(BuildApduData(GetRequest));
            var replyApdu = ParseReplyApduData(dataResult);
            var re = HandleGetResponse(replyApdu);
            if (re?.GetResponseNormal != null)
            {
                getResponses.Add(re);
                return getResponses;
            }


            await HowToHandleBlockNumber(getResponses, re);

            return getResponses;
        }
        private async Task<List<GetResponse>> HandleGetResponses(byte[] dataResult)
        {

            List<GetResponse> responses = new List<GetResponse>();

            GetResponse getResponse = new GetResponse();
            var data = dataResult.ByteToString();
            if (!getResponse.PduStringInHexConstructor(ref data))
            {
                return null;
            }
            XmlHelper.XmlCommon(getResponse);
            //只有一个
            if (getResponse?.GetResponseNormal != null)
            {
                responses.Add(getResponse);
                return responses;
            }

            if (getResponse?.GetResponseWithDataBlock != null)
            {
                if (getResponse.GetResponseWithDataBlock.DataBlockG.LastBlock.Value == "00")
                {
                    responses.Add(getResponse);
                    var blockNumber = getResponse.GetResponseWithDataBlock.DataBlockG.BlockNumber;
                    var GetRequest = new GetRequest
                    {
                        GetRequestNext = new GetRequestNext() { BlockNumber = blockNumber }
                    };
                    var dataGetRequestNextResultApdu = BuildApduData(GetRequest);
                    var GetResponseWithDataBlock = await LinkLayer.SendAsync(dataGetRequestNextResultApdu);
                    responses = await HandleGetResponses(GetResponseWithDataBlock);
                    if (CancellationTokenSource.Token.IsCancellationRequested)
                    {
                        return responses;
                    }
                }
                else if (getResponse.GetResponseWithDataBlock.DataBlockG.LastBlock.Value == "01")
                {
                    responses.Add(getResponse);
                }
                return responses;
            }
            return responses;
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
                    var  GetRequest = new GetRequest
                    {
                        GetRequestNext = new GetRequestNext() { BlockNumber = blockNumber }
                    };
                    var dataGetRequestNextResult = await LinkLayer.SendAsync(BuildApduData(GetRequest));
                    var replyApdu = ParseReplyApduData(dataGetRequestNextResult);
                    var re = HandleGetResponse(replyApdu);
                    await HowToHandleBlockNumber(list, re);
                }
                else if (response.GetResponseWithDataBlock.DataBlockG.LastBlock.Value == "01")
                {
                    list.Add(response);
                }
            }
        }
        public async Task<SetResponse> SetRequestAndWaitResponse(CosemAttributeDescriptor cosemAttributeDescriptor,
        DlmsDataItem value)
        {
            var setRequest = new SetRequest { SetRequestNormal = new SetRequestNormal(cosemAttributeDescriptor, value) };
            XmlHelper.XmlCommon(setRequest);
            var dataResult = await LinkLayer.SendAsync(BuildApduData(setRequest));
            var replyApdu = ParseReplyApduData(dataResult);
            return HandleSetResponse(replyApdu);
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

        #region Action
        public async Task<byte[]> ActionRequestAndWaitResponseWithByte(CosemMethodDescriptor cosemMethodDescriptor,
           DlmsDataItem dlmsDataItem)
        {
            var actionRequest = new ActionRequest()
            {
                ActionRequestNormal = new ActionRequestNormal(cosemMethodDescriptor,
                    dlmsDataItem)
            };
            XmlHelper.XmlCommon(actionRequest);
            return await LinkLayer.SendAsync(BuildApduData(actionRequest));
        }
        public async Task<Response> ActionRequestAndWaitResponse(CosemMethodDescriptor cosemMethodDescriptor,
         DlmsDataItem dlmsDataItem)
        {
            var actionRequest = new ActionRequest()
            {
                ActionRequestNormal = new ActionRequestNormal(cosemMethodDescriptor,
                    dlmsDataItem)
            };
            XmlHelper.XmlCommon(actionRequest);

            var re = await LinkLayer.SendAsync(BuildApduData(actionRequest));
            var actre = ParseReplyApduData(re);
            ActionResponse actionResponse = new ActionResponse();
            return new Response();
        }
        #endregion

        /// <summary>
        /// 根据协议类型+APDU 返回对应报文
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="Apdu"></param>
        /// <returns></returns>
        public byte[] BuildApduData(IToPduStringInHex Apdu)
        {
            // byte[] bytes = { };
            if (InterfaceType == InterfaceType.HDLC)
            {
                Hdlc46FrameBase.Apdu = Apdu.ToPduStringInHex().StringToByte();
                sendData = new SendData(Hdlc46FrameBase);
            }
            else if (InterfaceType == InterfaceType.WRAPPER)
            {
                WrapperFrame.WrapperBody.DataBytes = Apdu.ToPduStringInHex().StringToByte();
                sendData = new SendData(WrapperFrame);
            }

            return sendData.ToPduStringInHex().StringToByte();

        }
        /// <summary>
        /// 从返回的报文中 解析apdu数据，并返回
        /// </summary>
        /// <param name="replyApduData"></param>
        /// <returns></returns>
        public byte[] ParseReplyApduData(byte[] replyApduData)
        {
            if (replyApduData == null || replyApduData.Length == 0)
            {
                //  OnReportSnackbar("未收到响应帧");
                return null;
            }

            var returnPduBytes = new byte[] { };
            if (InterfaceType == InterfaceType.HDLC)
            {
                var pstring = replyApduData.ByteToString();
                if (Hdlc46FrameBase.PduStringInHexConstructor(ref pstring))
                {
                    returnPduBytes = Hdlc46FrameBase.Apdu;
                }
            }
            else if (InterfaceType == InterfaceType.WRAPPER)
            {
                //47协议来解析
                WrapperFrame wrapperFrame = new WrapperFrame();
                string parseHexString = replyApduData.ByteToString();

                if (wrapperFrame.PduStringInHexConstructor(ref parseHexString))
                {
                    returnPduBytes = wrapperFrame.WrapperBody.DataBytes;
                }
            }

            return returnPduBytes;
        }


        private GetResponse HandleGetResponse(byte[] dataResult)
        {
            GetResponse getResponse = new GetResponse();
            var data = dataResult.ByteToString();
            if (!getResponse.PduStringInHexConstructor(ref data))
            {
              //  OnReportSnackbar("解析响应帧失败");
                return null;
            }

            XmlHelper.XmlCommon(getResponse);

            return getResponse;
        }
        /// <summary>
        /// 执行21e协商
        /// </summary>
        /// <returns></returns>
        public Task<bool> Execute21ENegotiateAsync()
        {
            return Task.Run(async () =>
            {
                var keepAutoDataReceived = false;
                if (LinkLayer.PortMaster.IsAutoDataReceived)
                {
                    keepAutoDataReceived = true;
                    LinkLayer.PortMaster.IsAutoDataReceived = false;
                }
                LinkLayer.BackupPortPara();
                LinkLayer.Init21ESerialPort(300, 9600);
                byte[] array = await LinkLayer.SendAsync((EModeFrame.GetRequestFrameBytes()));
                if (array.Length != 0 && EModeParser.CheckServerFrameWisEquals2(array))
                {
                    await LinkLayer.SendAsync(EModeFrame.GetConfirmFrameBytes());
                    await Task.Delay(200);

                    LinkLayer.PortMaster.BaudRate = 9600; //需要修改波特率 ，再去接收
                    array = LinkLayer.PortMaster.TryToReadReceiveData();
                    if (array.Length != 0 && EModeParser.CheckServerFrameZisEqualsClient(array))
                    {
                        LinkLayer.PortMaster.SerialPortLogger.AddInfo("协商成功");
                        LinkLayer.LoadBackupPortPara();
                        if (keepAutoDataReceived)
                        {
                            LinkLayer.PortMaster.IsAutoDataReceived = true;
                        }
                        return true;
                    }

                    return false;
                }
                else
                {
                    if (keepAutoDataReceived)
                    {
                        LinkLayer.PortMaster.IsAutoDataReceived = true;
                    }
                    LinkLayer.LoadBackupPortPara();
                    return false;
                }


            });

        }
    }
}

