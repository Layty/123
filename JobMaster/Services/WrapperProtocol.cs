using JobMaster.ViewModels;
using MyDlmsStandard;
using MyDlmsStandard.ApplicationLay.Association;
using MyDlmsStandard.Axdr;
using MyDlmsStandard.Wrapper;


namespace JobMaster.Services
{
    public class WrapperProtocol : IProtocol
    {
        public ProtocolInterfaceType InterfaceType { get; } = ProtocolInterfaceType.WRAPPER;
        public SendData SendData { get; set; }
        public WrapperFrame WrapperFrame { get; }
        public AssociationRequest AssociationRequest { get; set; }
        public AssociationResponse AssociationResponse { get; set; }

        public WrapperProtocol(DlmsSettingsViewModel dlmsSettingsViewModel)
        {
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

            AssociationRequest = AssoctioinFactory.CreateAssociationRequest(dlmsSettingsViewModel);
            AssociationResponse = new AssociationResponse();
        }

        public byte[] BuildFinalSendData(IToPduStringInHex Apdu)
        {
            WrapperFrame.WrapperBody.DataBytes = Apdu.ToPduStringInHex().StringToByte();
            SendData = new SendData(WrapperFrame);
            return SendData.ToPduStringInHex().StringToByte();
        }

        public byte[] TakeReplyApduFromFrame(byte[] frameBytes)
        {
            if (frameBytes == null || frameBytes.Length == 0)
            {
                return null;
            }

            var returnPduBytes = System.Array.Empty<byte>();

            string parseHexString = frameBytes.ByteToString();

            if (WrapperFrame.PduStringInHexConstructor(ref parseHexString))
            {
                returnPduBytes = WrapperFrame.WrapperBody.DataBytes;
            }

            return returnPduBytes;
        }
    }
}