using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.Action;
using MyDlmsStandard.ApplicationLay.Get;
using MyDlmsStandard.ApplicationLay.Release;
using MyDlmsStandard.ApplicationLay.Set;
using MyDlmsStandard.Axdr;
using MyDlmsStandard.Ber;


namespace JobMaster.Helpers
{
    /// <summary>
    /// dlms 应用层帧 工厂
    /// </summary>
    public static class AppProtocolFactory
    {

        public static GetRequest CreateGetRequestNormal(CosemAttributeDescriptor cosemAttributeDescriptor)
        {
            var GetRequest = new GetRequest();
            GetRequest.GetRequestNormal = new GetRequestNormal(cosemAttributeDescriptor);
            return GetRequest;
        }
        public static GetRequest CreateGetRequestNormal(CosemAttributeDescriptorWithSelection cosemAttributeDescriptorWithSelection)
        {
            var GetRequest = new GetRequest();
            GetRequest.GetRequestNormal = new GetRequestNormal(cosemAttributeDescriptorWithSelection);
            return GetRequest;
        }

        public static GetRequest CreateGetRequestNext(AxdrIntegerUnsigned32 blockNumber)
        {
            var GetRequest = new GetRequest
            {
                GetRequestNext = new GetRequestNext() { BlockNumber = blockNumber }
            };

            return GetRequest;
        }
        public static SetRequest CreateSetRequest(CosemAttributeDescriptor cosemAttributeDescriptor,
            DlmsDataItem value)
        {
            SetRequest setRequest = new();
            setRequest.SetRequestNormal = new SetRequestNormal(cosemAttributeDescriptor, value);
            return setRequest;
        }
        public static ActionRequest CreateActionRequest(CosemMethodDescriptor cosemMethodDescriptor,
          DlmsDataItem dlmsDataItem)
        {
            var actionRequest = new ActionRequest()
            {
                ActionRequestNormal = new ActionRequestNormal(cosemMethodDescriptor,
                    dlmsDataItem)
            };
            return actionRequest;
        }

        public static GetResponse CreateGetResponse(byte[] dataResult)
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
        public static SetResponse CreateSetResponse(byte[] dataResult)
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
        public static ActionResponse CreateActionResponse(byte[] bytes)
        {
            ActionResponse actionResponse = new ActionResponse();
            // TODO: 
            return new ActionResponse();
        }

        public static ReleaseRequest CreateReleaseRequest()
        {
            var releaseRequest = new ReleaseRequest
            {
                Reason = new BerInteger() { Value = "00" }
                //UserInformation = new UserInformation()
                //{
                //    InitiateRequest = new InitiateRequest(DlmsSettingsViewModel.MaxReceivePduSize,
                //            DlmsSettingsViewModel.DlmsVersion, DlmsSettingsViewModel.ProposedConformance)
                //}
            };
            return releaseRequest;
        }

        public static ReleaseResponse CreateReleaseResponse(byte[] dataResult)
        {
            var releaseResponse = new ReleaseResponse();
            var data = dataResult.ByteToString();
            if (!releaseResponse.PduStringInHexConstructor(ref data))
            {
                return null;
            }

            return releaseResponse;
        }
    }
}