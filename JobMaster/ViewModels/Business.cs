using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using System.Threading.Tasks;
using JobMaster.Helpers;

namespace JobMaster.ViewModels
{
    public class NettyBusiness
    {
        public IProtocol Protocol { get; set; }
        public ILinkLayer LinkLayer { get; set; }


        public NettyBusiness(IProtocol protocol,ILinkLayer linkLayer)
        {
            LinkLayer = linkLayer;         
            Protocol = protocol;
        }
        

        public async Task AssociationRequestAsyncNetty()
        {
            await LinkLayer.SendAsync(Protocol.BuildFinalSendData(Protocol.AssociationRequest));
        }

        public async Task GetRequestAndWaitResponseNetty(CosemAttributeDescriptor cosemAttributeDescriptor,
            GetRequestType getRequestType = GetRequestType.Normal)
        {
            var request = AppProtocolFactory.CreateGetRequestNormal(cosemAttributeDescriptor);
            await LinkLayer.SendAsync(Protocol.BuildFinalSendData(request));
        }

        public async Task GetRequestAndWaitResponseArrayNetty(
            CosemAttributeDescriptorWithSelection cosemAttributeDescriptorWithSelection,
            GetRequestType getRequestType = GetRequestType.Normal)
        {
            var request = AppProtocolFactory.CreateGetRequestNormal(cosemAttributeDescriptorWithSelection);
            await LinkLayer.SendAsync(Protocol.BuildFinalSendData(request));
        }

        public async Task ReleaseRequestAsyncNetty(bool force = true)
        {
            var re = AppProtocolFactory.CreateReleaseRequest();
            XmlHelper.XmlCommon(re);
            await LinkLayer.SendAsync(Protocol.BuildFinalSendData(re));
        }
    }

  
}