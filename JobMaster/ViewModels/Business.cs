using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using System.Threading.Tasks;
using JobMaster.Helpers;
using JobMaster.Services;

namespace JobMaster.ViewModels
{
    public class NettyBusiness
    {
        public IProtocol Protocol { get; set; }
        public ILinkLayer LinkLayer { get; set; }


        public NettyBusiness(IProtocol protocol, ILinkLayer linkLayer)
        {
            LinkLayer = linkLayer;
            Protocol = protocol;
        }


        public async Task AssociationRequestAsyncNetty()
        {
            await LinkLayer.SendAsync(Protocol.BuildFinalSendData(Protocol.AssociationRequest));
        }

        public async Task GetRequestAndWaitResponseNetty(CosemAttributeDescriptor cosemAttributeDescriptor)
        {
            var request = AppProtocolFactory.CreateGetRequestNormal(cosemAttributeDescriptor);
            await LinkLayer.SendAsync(Protocol.BuildFinalSendData(request));
        }

        public async Task GetRequestAndWaitResponseArrayNetty(
            CosemAttributeDescriptorWithSelection cosemAttributeDescriptorWithSelection)
        {
            var request = AppProtocolFactory.CreateGetRequestNormal(cosemAttributeDescriptorWithSelection);
            await LinkLayer.SendAsync(Protocol.BuildFinalSendData(request));
        }

        public async Task SetRequestAndWaitResponseNetty(CosemAttributeDescriptor cosemAttributeDescriptor, DlmsDataItem dataItem)
        {
            var request = AppProtocolFactory.CreateSetRequest(cosemAttributeDescriptor, dataItem);
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