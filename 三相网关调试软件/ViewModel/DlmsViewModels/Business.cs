using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.Get;
using MyDlmsStandard.ApplicationLay.Set;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyDlmsStandard;
using MyDlmsStandard.HDLC;
using MyDlmsStandard.HDLC.Enums;
using 三相智慧能源网关调试软件.Common;
using MyDlmsStandard.ApplicationLay.Action;
using MyDlmsStandard.HDLC.IEC21EMode;
using 三相智慧能源网关调试软件.Factory;

namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{
    /// <summary>
    /// 业务层,理论上只关心DLMS应用层协议
    /// </summary>
    public class Business
    {
        public DlmsSettingsViewModel DlmsSettingsViewModel { get; set; }
        public Protocol Protocol { get; set; }
        public LinkLayer LinkLayer { get; set; }

        public PhysicalChanelType CommunicationType { get; set; }
        public StartProtocolType StartProtocolType { get; set; }
        public ProtocolInterfaceType InterfaceType { get; set; }

        /// <summary>
        /// 用于取消长时间的操作，例如请求曲线块时处理NextBlock
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; set; }

        public Business(DlmsSettingsViewModel dlmsSettingsViewModel,SerialPortViewModel serialPortViewModel,TcpServerViewModel tcpServerViewModel)
        {
            DlmsSettingsViewModel = dlmsSettingsViewModel;
            Protocol = new Protocol(dlmsSettingsViewModel);
            InterfaceType = dlmsSettingsViewModel.ProtocolInterfaceType;
            StartProtocolType = dlmsSettingsViewModel.StartProtocolType;
            LinkLayer = new LinkLayer(serialPortViewModel.SerialPortMaster,tcpServerViewModel);
            LinkLayer.CommunicationType = dlmsSettingsViewModel.PhysicalChanelType;
            CancellationTokenSource = new CancellationTokenSource();
        }
        public Business()
        {

        }

        /// <summary>
        /// 业务层的初始化 ，内部调用协议层的SNRM
        /// </summary>
        /// <returns></returns>
        public async Task<bool> InitRequestAsync()
        {
            bool initResult = false;
            if (InterfaceType == ProtocolInterfaceType.HDLC)
            {
                if (StartProtocolType == StartProtocolType.IEC21E)
                {
                    var flag21E = await Execute21ENegotiateAsync();
                    if (!flag21E)
                    {
                        //StrongReferenceMessenger.Default.Send("21E协商失败", "Snackbar");
                        return false;
                    }
                }
                else if (StartProtocolType == StartProtocolType.DLMS)
                {
                    //Snrm
                
                    byte[] bytes = await LinkLayer.SendAsync(Protocol.SNRMRequest.ToPduStringInHex());
                    if (!Protocol.SNRMRequest.ParseUaResponse(bytes)) return false;

                    //AARQ
                    XmlHelper.XmlCommon(Protocol.AssociationRequest);
                    bytes = await LinkLayer.SendAsync(Protocol.BuildFinalSendData(ProtocolInterfaceType.HDLC,
                        Protocol.AssociationRequest));
                    var result = Protocol.TakeReplyApduFromFrame(ProtocolInterfaceType.HDLC, bytes).ByteToString();
                    if (Protocol.AssociationResponse.PduStringInHexConstructor(ref result))
                    {
                        initResult = true;
                        XmlHelper.XmlCommon(Protocol.AssociationResponse);
                    }
                }
            }
            else if (InterfaceType == ProtocolInterfaceType.WRAPPER)
            {
                XmlHelper.XmlCommon(Protocol.AssociationRequest);
                byte[] bytes =
                    await LinkLayer.SendAsync(Protocol.BuildFinalSendData(InterfaceType,
                        Protocol.AssociationRequest));
                var result = Protocol.TakeReplyApduFromFrame(InterfaceType, bytes).ByteToString();
                if (Protocol.AssociationResponse.PduStringInHexConstructor(ref result))
                {
                    initResult = true;
                    XmlHelper.XmlCommon(Protocol.AssociationResponse);
                }
            }

            return initResult;
        }

        public async Task<bool> ReleaseRequestAsync(bool force = true)
        {
            byte[] result;

            if (force && (InterfaceType == ProtocolInterfaceType.HDLC))
            {
                result = await LinkLayer.SendAsync(Protocol.BuildFinalSendData(InterfaceType,
                    new DisConnectRequest(1, 1)));
                //TODO :ParseUA
                return Protocol.Hdlc46FrameBase.ParseUaResponse(result);
            }
            else if (force && InterfaceType == ProtocolInterfaceType.WRAPPER)
            {
                var re = AppProtocolFactory.CreateReleaseRequest();
                XmlHelper.XmlCommon(re);
                var bytes = await LinkLayer.SendAsync(Protocol.BuildFinalSendData(InterfaceType, re));
            }

            // await PhysicalLayerSendData(releaseBytes);
            // TODO Parse ReleaseResponse  应该使用解析Response中的方法

            return true;
        }

        public async Task<GetResponse> GetRequestAndWaitResponse(CosemAttributeDescriptor cosemAttributeDescriptor,
            GetRequestType getRequestType = GetRequestType.Normal)
        {
            var request = AppProtocolFactory.CreateGetRequestNormal(cosemAttributeDescriptor);
            var dataResult = await LinkLayer.SendAsync(Protocol.BuildFinalSendData(InterfaceType, request));
            var replyApdu = Protocol.TakeReplyApduFromFrame(InterfaceType, dataResult);
            return AppProtocolFactory.CreateGetResponse(replyApdu);
        }

        public async Task<List<GetResponse>> GetRequestAndWaitResponseArray(
            CosemAttributeDescriptor cosemAttributeDescriptor, GetRequestType getRequestType = GetRequestType.Normal)
        {
            List<GetResponse> getResponses = new List<GetResponse>();
            var request = AppProtocolFactory.CreateGetRequestNormal(cosemAttributeDescriptor);
            var dataResult = await LinkLayer.SendAsync(Protocol.BuildFinalSendData(InterfaceType, request));
            var replyApdu = Protocol.TakeReplyApduFromFrame(InterfaceType, dataResult);
            var re = AppProtocolFactory.CreateGetResponse(replyApdu);

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
                    var getRequestNext = AppProtocolFactory.CreateGetRequestNext(blockNumber);
                    var dataGetRequestNextResult =
                        await LinkLayer.SendAsync(Protocol.BuildFinalSendData(InterfaceType, getRequestNext));
                    var replyApdu = Protocol.TakeReplyApduFromFrame(InterfaceType, dataGetRequestNextResult);
                    var re = AppProtocolFactory.CreateGetResponse(replyApdu);
                    await HowToHandleBlockNumber(list, re);
                }
                else if (response.GetResponseWithDataBlock.DataBlockG.LastBlock.Value == "01")
                {
                    list.Add(response);
                }
            }
        }

        public async Task<GetResponse> GetRequestAndWaitResponse(
            CosemAttributeDescriptorWithSelection cosemAttributeDescriptorWithSelection,
            GetRequestType getRequestType = GetRequestType.Normal)
        {
            var request = AppProtocolFactory.CreateGetRequestNormal(cosemAttributeDescriptorWithSelection);
            var dataResult = await LinkLayer.SendAsync(Protocol.BuildFinalSendData(InterfaceType, request));
            var replyApdu = Protocol.TakeReplyApduFromFrame(InterfaceType, dataResult);
            return AppProtocolFactory.CreateGetResponse(replyApdu);

        }

        public async Task<List<GetResponse>> GetRequestAndWaitResponseArray(
            CosemAttributeDescriptorWithSelection cosemAttributeDescriptorWithSelection,
            GetRequestType getRequestType = GetRequestType.Normal)
        {
            var request = AppProtocolFactory.CreateGetRequestNormal(cosemAttributeDescriptorWithSelection);
            var dataResult = await LinkLayer.SendAsync(Protocol.BuildFinalSendData(InterfaceType, request));
            var replyApdu = Protocol.TakeReplyApduFromFrame(InterfaceType, dataResult);
            var re = AppProtocolFactory.CreateGetResponse(replyApdu);

            List<GetResponse> getResponses = new List<GetResponse>();
            if (re?.GetResponseNormal != null)
            {
                getResponses.Add(re);
                return getResponses;
            }

            await HowToHandleBlockNumber(getResponses, re);

            return getResponses;

        }

        public async Task<SetResponse> SetRequestAndWaitResponse(CosemAttributeDescriptor cosemAttributeDescriptor,
            DlmsDataItem value)
        {
            var setRequest = AppProtocolFactory.CreateSetRequest(cosemAttributeDescriptor, value);
            XmlHelper.XmlCommon(setRequest);
            var dataResult = await LinkLayer.SendAsync(Protocol.BuildFinalSendData(InterfaceType, setRequest));
            var replyApdu = Protocol.TakeReplyApduFromFrame(InterfaceType, dataResult);
            return AppProtocolFactory.CreateSetResponse(replyApdu);
        }

        public async Task<byte[]> ActionRequestAndWaitResponseWithByte(CosemMethodDescriptor cosemMethodDescriptor,
            DlmsDataItem dlmsDataItem)
        {
            var actionRequest = AppProtocolFactory.CreateActionRequest(cosemMethodDescriptor, dlmsDataItem);
            return await LinkLayer.SendAsync(Protocol.BuildFinalSendData(InterfaceType, actionRequest));
        }

        public async Task<ActionResponse> ActionRequestAndWaitResponse(CosemMethodDescriptor cosemMethodDescriptor,
            DlmsDataItem dlmsDataItem)
        {
            var actionRequest = AppProtocolFactory.CreateActionRequest(cosemMethodDescriptor, dlmsDataItem);
            var re = await LinkLayer.SendAsync(Protocol.BuildFinalSendData(InterfaceType, actionRequest));
            var actre = Protocol.TakeReplyApduFromFrame(InterfaceType, re);
            return AppProtocolFactory.CreateActionResponse(actre);
        }


        public async Task Cancel()
        {
            CancellationTokenSource.Cancel();
            await Task.Delay(2000);
            CancellationTokenSource = new CancellationTokenSource();
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
                LinkLayer.Init21ESerialPort(300);
                byte[] array = await LinkLayer.SendAsync((Protocol.EModeFrame.GetRequestFrameBytes()));
                if (array.Length != 0 && EModeParser.CheckServerFrameWisEquals2(array))
                {
                    await LinkLayer.SendAsync(Protocol.EModeFrame.GetConfirmFrameBytes());
                    await Task.Delay(200);
                    LinkLayer.LoadBackupPortPara();//即之前如申请9600，调用有则为9600等参数进行接受
                   // LinkLayer.PortMaster.BaudRate = 9600; //需要修改波特率 ，再去接收
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
        public async Task<byte[]> SetEnterUpGradeMode()
        {
            return await LinkLayer.PortMaster.SendAndReceiveReturnDataAsync(Protocol.SetEnterUpGradeMode());
        }
    }
}