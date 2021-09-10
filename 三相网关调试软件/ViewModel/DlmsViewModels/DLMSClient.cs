﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Text;
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
using MySerialPortMaster;
using 三相智慧能源网关调试软件.Common;


namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{
    public class DlmsClient : ObservableObject
    {
        #region 协议层资源

        public EModeViewModel EModeViewModel { get; set; }
        public HdlcFrameMaker HdlcFrameMaker { get; set; }
        public NetFrameMaker NetFrameMaker { get; set; }

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
        public RelayCommand CancelCommand { get; set; }

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
                if (DlmsSettingsViewModel.CommunicationType == CommunicationType.SerialPort)
                {
                    returnBytes = await PortMaster.SendAndReceiveReturnDataAsync(sendBytes);
                }
                else if (DlmsSettingsViewModel.CommunicationType == CommunicationType.FrontEndProcess)
                {
                    returnBytes = await Socket.SendDataToClientAndWaitReceiveData(CurrentSocket, sendBytes);
                }
            }
            catch (Exception e)
            {
                OnReportSnackbar(e.Message);
            }


            return returnBytes;
        }

        private byte[] HowToTakeReplyApduData(byte[] parseBytes)
        {
            if (parseBytes == null || parseBytes.Length == 0)
            {
                OnReportSnackbar("未收到响应帧");
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
        public async Task<bool> InitRequest()
        {
            HdlcFrameMaker = new HdlcFrameMaker(DlmsSettingsViewModel.ServerAddress,
                (byte) DlmsSettingsViewModel.ClientAddress, DlmsSettingsViewModel.DlmsInfo);
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

                //HDLC46
                byte[] bytes = await PhysicalLayerSendData(HdlcFrameMaker.SNRMRequest());
                if (HdlcFrameMaker.ParseUaResponse(bytes))
                {
                    AssociationRequest aarq = new AssociationRequest(DlmsSettingsViewModel.PasswordHex,
                        DlmsSettingsViewModel.MaxReceivePduSize, DlmsSettingsViewModel.DlmsVersion,
                        DlmsSettingsViewModel.SystemTitle, DlmsSettingsViewModel.ProposedConformance);
                    bytes = await PhysicalLayerSendData(HdlcFrameMaker.InvokeApduHex(aarq.ToPduStringInHex()));
                    bytes = HowToTakeReplyApduData(bytes);
                    if (bytes != null)
                    {
                        var result = MyDlmsStandard.Common.Common.ByteToString(bytes);
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
                byte[] bytes =
                    await PhysicalLayerSendData(NetFrameMaker.InvokeApdu(aarq.ToPduStringInHex().StringToByte()));
                bytes = HowToTakeReplyApduData(bytes);
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
            CosemAttributeDescriptor cosemAttributeDescriptor, 
            GetRequestType getRequestType = GetRequestType.Normal)
        {
            List<GetResponse> getResponses = new List<GetResponse>();
            _stringBuilder = new StringBuilder();
            getRequest = new GetRequest();

            switch (getRequestType)
            {
                case GetRequestType.Normal:
                    getRequest.GetRequestNormal = new GetRequestNormal(cosemAttributeDescriptor);
                    break;
                case GetRequestType.Next:
                    getRequest.GetRequestNext = new GetRequestNext();
                    break;
                case GetRequestType.WithList:
                    getRequest.GetRequestWithList = new GetRequestWithList();
                    break;
            }

            XmlHelper.XmlCommon(getRequest);
            var dataResult = await HandlerSendData(getRequest.ToPduStringInHex());
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
            getRequest = new GetRequest();

            switch (getRequestType)
            {
                case GetRequestType.Normal:
                    getRequest.GetRequestNormal = new GetRequestNormal(cosemAttributeDescriptor);
                    break;
                case GetRequestType.Next:
                    getRequest.GetRequestNext = new GetRequestNext();
                    break;
                case GetRequestType.WithList:
                    getRequest.GetRequestWithList = new GetRequestWithList();
                    break;
            }

            XmlHelper.XmlCommon(getRequest);
            var dataResult = await HandlerSendData(getRequest.ToPduStringInHex());
            return HandleGetResponse(dataResult);
        }

        public async Task<GetResponse> GetRequestAndWaitResponse(
            CosemAttributeDescriptorWithSelection cosemAttributeDescriptorWithSelection,
            GetRequestType getRequestType = GetRequestType.Normal)
        {
            switch (getRequestType)
            {
                case GetRequestType.Normal:
                    getRequest.GetRequestNormal = new GetRequestNormal(cosemAttributeDescriptorWithSelection);
                    break;
                case GetRequestType.Next:
                    getRequest.GetRequestNext = new GetRequestNext();
                    break;
                case GetRequestType.WithList:
                    getRequest.GetRequestWithList = new GetRequestWithList();
                    break;
            }

            XmlHelper.XmlCommon(getRequest);
            var dataResult = await HandlerSendData(getRequest.ToPduStringInHex());
            return HandleGetResponse(dataResult);
        }

        public async Task<List<GetResponse>> GetRequestAndWaitResponseArray(
            CosemAttributeDescriptorWithSelection cosemAttributeDescriptorWithSelection,
            GetRequestType getRequestType = GetRequestType.Normal)
        {
            List<GetResponse> getResponses = new List<GetResponse>();
            _stringBuilder = new StringBuilder();
            switch (getRequestType)
            {
                case GetRequestType.Normal:
                    getRequest.GetRequestNormal = new GetRequestNormal(cosemAttributeDescriptorWithSelection);
                    break;
                case GetRequestType.Next:
                    getRequest.GetRequestNext = new GetRequestNext();
                    break;
                case GetRequestType.WithList:
                    getRequest.GetRequestWithList = new GetRequestWithList();
                    break;
            }

            XmlHelper.XmlCommon(getRequest);
            var dataResult = await HandlerSendData(getRequest.ToPduStringInHex());

            var re = HandleGetResponse(dataResult);
            if (re?.GetResponseNormal != null)
            {
                getResponses.Add(re);
                return getResponses;
            }


            await HowToHandleBlockNumber(getResponses, re);

            return getResponses;
        }

        private StringBuilder _stringBuilder = new StringBuilder();

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
                    _stringBuilder.Append(response.GetResponseWithDataBlock.DataBlockG.RawData.Value);
                    list.Add(response);
                    var blockNumber = response.GetResponseWithDataBlock.DataBlockG.BlockNumber;
                    getRequest = new GetRequest
                    {
                        GetRequestNext = new GetRequestNext() {BlockNumber = blockNumber}
                    };
                    var dataGetRequestNextResult = await HandlerSendData(getRequest.ToPduStringInHex());
                    var re = HandleGetResponse(dataGetRequestNextResult);
                    await HowToHandleBlockNumber(list, re);
                }
                else if (response.GetResponseWithDataBlock.DataBlockG.LastBlock.Value == "01")
                {
                    _stringBuilder.Append(response.GetResponseWithDataBlock.DataBlockG.RawData.Value);
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
            string d = dataResult.ByteToString("");

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
            var dataResult = await HandlerSendData(setRequest.ToPduStringInHex());
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
            return await HandlerSendData(
                (MyDlmsStandard.Common.Common.StringToByte(actionRequest.ToPduStringInHex())));
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


        public async Task<bool> ReleaseRequest(bool force = true)
        {
            byte[] result = null;
            var re = new ReleaseRequest
            {
                Reason = new BerInteger() {Value = "00"},
                UserInformation = new UserInformation()
                {
                    InitiateRequest = new InitiateRequest(DlmsSettingsViewModel.MaxReceivePduSize,
                        DlmsSettingsViewModel.DlmsVersion, DlmsSettingsViewModel.ProposedConformance)
                }
            };
            var releaseBytes = re.ToPduStringInHex().StringToByte();
            if (force && (DlmsSettingsViewModel.InterfaceType == InterfaceType.HDLC))
            {
                releaseBytes = HdlcFrameMaker.DisconnectRequest();
                result = await PhysicalLayerSendData(HdlcFrameMaker.DisconnectRequest());
                //TODO :ParseUA
                return HdlcFrameMaker.ParseUaResponse(result);
            }
            else if (force && DlmsSettingsViewModel.InterfaceType == InterfaceType.WRAPPER)
            {
                result = await PhysicalLayerSendData(NetFrameMaker.InvokeApdu(releaseBytes));
            }

            await PhysicalLayerSendData(releaseBytes);
            //TODO Parse ReleaseResponse

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

        private GetRequest getRequest { get; set; }


        public DlmsClient()
        {
            DlmsSettingsViewModel = ServiceLocator.Current.GetInstance<DlmsSettingsViewModel>();
            Socket = ServiceLocator.Current.GetInstance<TcpServerViewModel>().TcpServerHelper;

            PortMaster = ServiceLocator.Current.GetInstance<SerialPortViewModel>().SerialPortMaster;

            HdlcFrameMaker = new HdlcFrameMaker(DlmsSettingsViewModel.ServerAddress,
                (byte) DlmsSettingsViewModel.ClientAddress, DlmsSettingsViewModel.DlmsInfo);
            NetFrameMaker = new NetFrameMaker(DlmsSettingsViewModel);
            EModeViewModel = new EModeViewModel(PortMaster);

            InitSerialPortParams(PortMaster);


            InitRequestCommand = new RelayCommand(async () => { await InitRequest(); });
            ReleaseRequestCommand = new RelayCommand(async () => { await ReleaseRequest(true); });
            getRequest = new GetRequest();
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
            return PortMaster.SendAndReceiveReturnDataAsync(HdlcFrameMaker.SetEnterUpGradeMode(256));
        }
    }
}