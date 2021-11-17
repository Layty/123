using MyDlmsStandard;
using MyDlmsStandard.ApplicationLay.Association;
using MyDlmsStandard.Axdr;
using MyDlmsStandard.HDLC;
using MyDlmsStandard.HDLC.Enums;
using MyDlmsStandard.HDLC.IEC21EMode;
using MyDlmsStandard.Wrapper;
using System.Threading;
using System.Threading.Tasks;
using 三相智慧能源网关调试软件.Common;

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

    /// <summary>
    ///协议层
    /// </summary>
    public class Protocol
    {
        public Hdlc46FrameBase Hdlc46FrameBase { get; set; }
        public WrapperFrame WrapperFrame { get; set; }
        public EModeFrame EModeFrame { get; set; }

        public StartProtocolType StartProtocolType { get; set; }
        public SendData sendData { get; set; }
        public DlmsSettingsViewModel DlmsSettingsViewModel { get; set; }

       
        public SNRMRequest SNRMRequest { get; set; }
        public AssociationRequest AssociationRequest { get; set; }
        public AssociationResponse AssociationResponse { get; set; }

        public Protocol(DlmsSettingsViewModel dlmsSettingsViewModel)
        {
            DlmsSettingsViewModel = dlmsSettingsViewModel;
            StartProtocolType = dlmsSettingsViewModel.StartProtocolType;
            Hdlc46FrameBase = new Hdlc46FrameBase((byte)dlmsSettingsViewModel.ServerAddress,
                (byte)dlmsSettingsViewModel.ClientAddress, dlmsSettingsViewModel.DlmsInfo);
            var wrapperHeader = new WrapperHeader()
            {
                Version = new AxdrIntegerUnsigned16("1"),
                SourceAddress = new AxdrIntegerUnsigned16(dlmsSettingsViewModel.ClientAddress.ToString("X4")),
                DestAddress = new AxdrIntegerUnsigned16(dlmsSettingsViewModel.ServerAddress.ToString("X4")),
            };
            WrapperFrame = new WrapperFrame()
            {
                WrapperHeader = wrapperHeader
            };
            EModeFrame = new EModeFrame(dlmsSettingsViewModel.NegotiateBaud);

            SNRMRequest = new SNRMRequest((byte)DlmsSettingsViewModel.ServerAddress,
                (byte)DlmsSettingsViewModel.ClientAddress, DlmsSettingsViewModel.DlmsInfo);
            AssociationRequest = new AssociationRequest(DlmsSettingsViewModel.PasswordHex,
                DlmsSettingsViewModel.MaxReceivePduSize, DlmsSettingsViewModel.DlmsVersion,
                DlmsSettingsViewModel.SystemTitle, DlmsSettingsViewModel.ProposedConformance);
            AssociationResponse = new AssociationResponse();
        }



        //        public async Task<GetResponse> GetRequestAndWaitResponse(CosemAttributeDescriptor cosemAttributeDescriptor,
        //            GetRequestType getRequestType = GetRequestType.Normal)
        //        {
        //            var GetRequest = new GetRequest();
        //
        //            switch (getRequestType)
        //            {
        //                case GetRequestType.Normal:
        //                    GetRequest.GetRequestNormal = new GetRequestNormal(cosemAttributeDescriptor);
        //                    break;
        //                case GetRequestType.Next:
        //                    GetRequest.GetRequestNext = new GetRequestNext();
        //                    break;
        //                case GetRequestType.WithList:
        //                    GetRequest.GetRequestWithList = new GetRequestWithList();
        //                    break;
        //            }
        //
        //            XmlHelper.XmlCommon(GetRequest);
        //            var dataResult = await LinkLayer.SendAsync(BuildFinalSendData(GetRequest));
        //            var replyApdu = ParseReplyApduData(dataResult);
        //            return HandleGetResponse(replyApdu);
        //        }

        //        public async Task<List<GetResponse>> GetRequestAndWaitResponseArray(
        //            CosemAttributeDescriptor cosemAttributeDescriptor, GetRequestType getRequestType = GetRequestType.Normal)
        //        {
        //            List<GetResponse> getResponses = new List<GetResponse>();
        //            var GetRequest = new GetRequest();
        //
        //            switch (getRequestType)
        //            {
        //                case GetRequestType.Normal:
        //                    GetRequest.GetRequestNormal = new GetRequestNormal(cosemAttributeDescriptor);
        //                    break;
        //                case GetRequestType.Next:
        //                    GetRequest.GetRequestNext = new GetRequestNext();
        //                    break;
        //                case GetRequestType.WithList:
        //                    GetRequest.GetRequestWithList = new GetRequestWithList();
        //                    break;
        //            }
        //
        //            XmlHelper.XmlCommon(GetRequest);
        //            var dataResult = await LinkLayer.SendAsync(BuildFinalSendData(GetRequest));
        //            var replyApdu = ParseReplyApduData(dataResult);
        //            var re = HandleGetResponse(replyApdu);
        //
        //            if (re?.GetResponseNormal != null)
        //            {
        //                getResponses.Add(re);
        //                return getResponses;
        //            }
        //
        //
        //            await HowToHandleBlockNumber(getResponses, re);
        //
        //            return getResponses;
        //        }




        //        public async Task<List<GetResponse>> GetRequestAndWaitResponseArray(
        //            CosemAttributeDescriptorWithSelection cosemAttributeDescriptorWithSelection,
        //            GetRequestType getRequestType = GetRequestType.Normal)
        //        {
        //            var GetRequest = new GetRequest();
        //            List<GetResponse> getResponses = new List<GetResponse>();
        //            switch (getRequestType)
        //            {
        //                case GetRequestType.Normal:
        //                    GetRequest.GetRequestNormal = new GetRequestNormal(cosemAttributeDescriptorWithSelection);
        //                    break;
        //                case GetRequestType.Next:
        //                    GetRequest.GetRequestNext = new GetRequestNext();
        //                    break;
        //                case GetRequestType.WithList:
        //                    GetRequest.GetRequestWithList = new GetRequestWithList();
        //                    break;
        //            }
        //
        //            XmlHelper.XmlCommon(GetRequest);
        //            var dataResult = await LinkLayer.SendAsync(BuildFinalSendData(GetRequest));
        //            var replyApdu = ParseReplyApduData(dataResult);
        //            var re = HandleGetResponse(replyApdu);
        //            if (re?.GetResponseNormal != null)
        //            {
        //                getResponses.Add(re);
        //                return getResponses;
        //            }
        //
        //
        //            await HowToHandleBlockNumber(getResponses, re);
        //
        //            return getResponses;
        //        }

        //private async Task<List<GetResponse>> HandleGetResponses(byte[] dataResult)
        //{
        //    List<GetResponse> responses = new List<GetResponse>();

        //    GetResponse getResponse = new GetResponse();
        //    var data = dataResult.ByteToString();
        //    if (!getResponse.PduStringInHexConstructor(ref data))
        //    {
        //        return null;
        //    }

        //    XmlHelper.XmlCommon(getResponse);
        //    //只有一个
        //    if (getResponse?.GetResponseNormal != null)
        //    {
        //        responses.Add(getResponse);
        //        return responses;
        //    }

        //    if (getResponse?.GetResponseWithDataBlock != null)
        //    {
        //        if (getResponse.GetResponseWithDataBlock.DataBlockG.LastBlock.Value == "00")
        //        {
        //            responses.Add(getResponse);
        //            var blockNumber = getResponse.GetResponseWithDataBlock.DataBlockG.BlockNumber;
        //            var GetRequest = new GetRequest
        //            {
        //                GetRequestNext = new GetRequestNext() {BlockNumber = blockNumber}
        //            };
        //            var dataGetRequestNextResultApdu = BuildFinalSendData(GetRequest);
        //            var GetResponseWithDataBlock = await LinkLayer.SendAsync(dataGetRequestNextResultApdu);
        //            responses = await HandleGetResponses(GetResponseWithDataBlock);
        //            if (CancellationTokenSource.Token.IsCancellationRequested)
        //            {
        //                return responses;
        //            }
        //        }
        //        else if (getResponse.GetResponseWithDataBlock.DataBlockG.LastBlock.Value == "01")
        //        {
        //            responses.Add(getResponse);
        //        }

        //        return responses;
        //    }

        //    return responses;
        //}




        public byte[] BuildFinalSendData(ProtocolInterfaceType interfaceType, IToPduStringInHex Apdu)
        {
            if (interfaceType == ProtocolInterfaceType.HDLC)
            {
                Hdlc46FrameBase.Apdu = Apdu.ToPduStringInHex().StringToByte();
                sendData = new SendData(Hdlc46FrameBase);
            }
            else if (interfaceType == ProtocolInterfaceType.WRAPPER)
            {
                WrapperFrame.WrapperBody.DataBytes = Apdu.ToPduStringInHex().StringToByte();
                sendData = new SendData(WrapperFrame);
            }

            return sendData.ToPduStringInHex().StringToByte();
        }


        public byte[] TakeReplyApduFromFrame(ProtocolInterfaceType interfaceType, byte[] frameBytes)
        {
            if (frameBytes == null || frameBytes.Length == 0)
            {
                return null;
            }

            var returnPduBytes = new byte[] { };
            if (interfaceType == ProtocolInterfaceType.HDLC)
            {
                var pstring = frameBytes.ByteToString();
                if (Hdlc46FrameBase.PduStringInHexConstructor(ref pstring))
                {
                    returnPduBytes = Hdlc46FrameBase.Apdu;
                }
            }
            else if (interfaceType == ProtocolInterfaceType.WRAPPER)
            {
                //47协议来解析
                WrapperFrame wrapperFrame = new WrapperFrame();
                string parseHexString = frameBytes.ByteToString();

                if (wrapperFrame.PduStringInHexConstructor(ref parseHexString))
                {
                    returnPduBytes = wrapperFrame.WrapperBody.DataBytes;
                }
            }

            return returnPduBytes;
        }




        /// <summary>
        ///// 执行21e协商
        ///// </summary>
        ///// <returns></returns>
        //public Task<bool> Execute21ENegotiateAsync()
        //{
        //    return Task.Run(async () =>
        //    {
        //        var keepAutoDataReceived = false;
        //        if (LinkLayer.PortMaster.IsAutoDataReceived)
        //        {
        //            keepAutoDataReceived = true;
        //            LinkLayer.PortMaster.IsAutoDataReceived = false;
        //        }

        //        LinkLayer.BackupPortPara();
        //        LinkLayer.Init21ESerialPort(300, 9600);
        //        byte[] array = await LinkLayer.SendAsync((EModeFrame.GetRequestFrameBytes()));
        //        if (array.Length != 0 && EModeParser.CheckServerFrameWisEquals2(array))
        //        {
        //            await LinkLayer.SendAsync(EModeFrame.GetConfirmFrameBytes());
        //            await Task.Delay(200);

        //            LinkLayer.PortMaster.BaudRate = 9600; //需要修改波特率 ，再去接收
        //            array = LinkLayer.PortMaster.TryToReadReceiveData();
        //            if (array.Length != 0 && EModeParser.CheckServerFrameZisEqualsClient(array))
        //            {
        //                LinkLayer.PortMaster.SerialPortLogger.AddInfo("协商成功");
        //                LinkLayer.LoadBackupPortPara();
        //                if (keepAutoDataReceived)
        //                {
        //                    LinkLayer.PortMaster.IsAutoDataReceived = true;
        //                }

        //                return true;
        //            }

        //            return false;
        //        }
        //        else
        //        {
        //            if (keepAutoDataReceived)
        //            {
        //                LinkLayer.PortMaster.IsAutoDataReceived = true;
        //            }

        //            LinkLayer.LoadBackupPortPara();
        //            return false;
        //        }
        //    });
        //}

        public byte[] SetEnterUpGradeMode()
        {
            return Hdlc46FrameBase.SetEnterUpGradeMode(256);
        }
    }

    public static class AssoctioinFactory
    {

        public static AssociationRequest CreateAssociationRequest(DlmsSettingsViewModel DlmsSettingsViewModel)
        {
            var AssociationRequest = new AssociationRequest(DlmsSettingsViewModel.PasswordHex,
                  DlmsSettingsViewModel.MaxReceivePduSize, DlmsSettingsViewModel.DlmsVersion,
                  DlmsSettingsViewModel.SystemTitle, DlmsSettingsViewModel.ProposedConformance);
            return AssociationRequest;
        }
    }
}