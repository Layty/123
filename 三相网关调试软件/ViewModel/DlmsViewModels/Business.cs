using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Messaging;
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
using 三相智慧能源网关调试软件.Common;

namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{
    /// <summary>
    /// 业务层
    /// </summary>
    public class Business
    {
        public DlmsSettingsViewModel DlmsSettingsViewModel { get; set; }
        Protocol Protocol { get; set; } = new Protocol(null);
        LinkLayer LinkLayer { get; set; } = new LinkLayer();
        public CancellationTokenSource CancellationTokenSource { get; set; }
        public Business(DlmsSettingsViewModel dlmsSettingsViewModel)
        {
            DlmsSettingsViewModel = dlmsSettingsViewModel;
               CancellationTokenSource = new CancellationTokenSource();
        }
        public async Task<bool> InitRequestAsync()
        {
            bool initResult = false;
            if (Protocol.InterfaceType==InterfaceType.HDLC)
            {
                // 21E协商
                if (Protocol.StartProtocolType == StartProtocolType.IEC21E)
                {
                    var flag21E = await Execute21ENegotiateAsync();
                    if (!flag21E)
                    {
                        StrongReferenceMessenger.Default.Send("21E协商失败", "Snackbar");
                        return false;
                    }
                }
                else if (Protocol.StartProtocolType == StartProtocolType.DLMS)
                {
                    //SNRM
                    var snrmRequest = new SNRMRequest((byte)(DlmsSettingsViewModel.ServerAddress),
                        (byte)DlmsSettingsViewModel.ClientAddress, DlmsSettingsViewModel.DlmsInfo);
                    //HDLC46
                    byte[] bytes = await LinkLayer.SendAsync(Protocol.BuildApduData (snrmRequest));
                    if (Protocol.Hdlc46FrameBase.ParseUaResponse(bytes))
                    {
                        //AARQ
                        Protocol.Hdlc46FrameBase = new Hdlc46FrameBase((byte)(DlmsSettingsViewModel.ServerAddress),
                            (byte)DlmsSettingsViewModel.ClientAddress, DlmsSettingsViewModel.DlmsInfo);
                        AssociationRequest aarq = new AssociationRequest(DlmsSettingsViewModel.PasswordHex,
                            DlmsSettingsViewModel.MaxReceivePduSize, DlmsSettingsViewModel.DlmsVersion,
                            DlmsSettingsViewModel.SystemTitle, DlmsSettingsViewModel.ProposedConformance);
                        Protocol.Hdlc46FrameBase.Apdu = aarq.ToPduStringInHex().StringToByte();
                        XmlHelper.XmlCommon(aarq);
                        bytes =  await LinkLayer.SendAsync(Protocol.BuildApduData(aarq));

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
                else if (DlmsSettingsViewModel.InterfaceType == InterfaceType.WRAPPER)
                {
                    AssociationRequest aarq = new AssociationRequest(DlmsSettingsViewModel.PasswordHex,
                        DlmsSettingsViewModel.MaxReceivePduSize, DlmsSettingsViewModel.DlmsVersion,
                        DlmsSettingsViewModel.SystemTitle, DlmsSettingsViewModel.ProposedConformance);
                    XmlHelper.XmlCommon(aarq);                                    
                    byte[] bytes = await LinkLayer.SendAsync(Protocol.BuildApduData(aarq));
                    if (bytes != null && bytes.Length != 0)
                    {
                      var result =  Protocol.ParseReplyApduData(bytes).ByteToString();
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

            return true;
        }
        public async Task<bool> ReleaseRequestAsync(bool force = true)
        {
            byte[] result;

            if (force && (DlmsSettingsViewModel.InterfaceType == InterfaceType.HDLC))
            {
                result = await LinkLayer.SendAsync(Protocol.BuildApduData(new DisConnectRequest(1,1)));
                //TODO :ParseUA
                return Protocol.Hdlc46FrameBase.ParseUaResponse(result);
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
                var bytes = await LinkLayer.SendAsync(Protocol.BuildApduData(re));
              
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
            var dataResult = await LinkLayer.SendAsync(Protocol.BuildApduData(GetRequest));
            var replyApdu = Protocol.ParseReplyApduData(dataResult);
            return HandleGetResponse(replyApdu);
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
            var dataResult = await LinkLayer.SendAsync(Protocol.BuildApduData(GetRequest));
            var replyApdu = Protocol.ParseReplyApduData(dataResult);
            var rs = HandleGetResponses(replyApdu);
            return getResponses;
        }

        public async Task<SetResponse> SetRequestAndWaitResponse(CosemAttributeDescriptor cosemAttributeDescriptor,
           DlmsDataItem value)
        {
            var setRequest = new SetRequest { SetRequestNormal = new SetRequestNormal(cosemAttributeDescriptor, value) };
            XmlHelper.XmlCommon(setRequest);
            var dataResult = await LinkLayer.SendAsync(Protocol.BuildApduData(setRequest));
            var replyApdu = Protocol.ParseReplyApduData(dataResult);
            return HandleSetResponse(replyApdu);
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

            var re = await LinkLayer.SendAsync(Protocol.BuildApduData(actionRequest));
            var actre = Protocol.ParseReplyApduData(re);
            ActionResponse actionResponse = new ActionResponse();
            return new Response();
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
                    var dataGetRequestNextResultApdu = Protocol.BuildApduData(GetRequest);
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
        private GetResponse HandleGetResponse(byte[] dataResult)
        {
            GetResponse getResponse = new GetResponse();
            var data = dataResult.ByteToString();
            if (!getResponse.PduStringInHexConstructor(ref data))
            {
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
        /// <summary>
        /// 执行21e协商
        /// </summary>
        /// <returns></returns>
        public Task<bool> Execute21ENegotiateAsync()
        {
            return Task.Run(async() =>
            {
                var keepAutoDataReceived = false;
                if (LinkLayer.PortMaster.IsAutoDataReceived)
                {
                    keepAutoDataReceived = true;
                    LinkLayer. PortMaster.IsAutoDataReceived = false;
                }
                LinkLayer.BackupPortPara();
                LinkLayer.Init21ESerialPort(300, 9600);
                byte[] array = await LinkLayer.SendAsync((Protocol.EModeFrame.GetRequestFrameBytes()));
                if (array.Length != 0 && EModeParser.CheckServerFrameWisEquals2(array))
                {
                   await  LinkLayer.SendAsync(Protocol.EModeFrame.GetConfirmFrameBytes());
                   await Task.Delay(200);
                 
                   LinkLayer. PortMaster.BaudRate = 9600; //需要修改波特率 ，再去接收
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

