using MyDlmsStandard;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.Association;
using MyDlmsStandard.HDLC;


namespace JobMaster.Services
{
    public class HdlcProtocol : IProtocol
    {
        public SendData SendData { get; set; }
        public Hdlc46FrameBase Hdlc46FrameBase { get; set; }
        public ProtocolInterfaceType InterfaceType => ProtocolInterfaceType.HDLC;
        public AssociationRequest AssociationRequest { get; set; }
        public AssociationResponse AssociationResponse { get; set; }

        public HdlcProtocol(byte ServerAddress, byte ClientAddress)
        {
            Hdlc46FrameBase = new Hdlc46FrameBase(ServerAddress,
                ClientAddress, new DLMSInfo());
        }

        public byte[] BuildFinalSendData(IToPduStringInHex Apdu)
        {
            Hdlc46FrameBase.Apdu = Apdu.ToPduStringInHex().StringToByte();
            SendData = new SendData(Hdlc46FrameBase);
            return SendData.ToPduStringInHex().StringToByte();
        }

        public byte[] TakeReplyApduFromFrame(byte[] frameBytes)
        {
            if (frameBytes == null || frameBytes.Length == 0)
            {
                return null;
            }

            var returnPduBytes = System.Array.Empty<byte>();

            var pstring = frameBytes.ByteToString();
            if (Hdlc46FrameBase.PduStringInHexConstructor(ref pstring))
            {
                returnPduBytes = Hdlc46FrameBase.Apdu;
            }

            return returnPduBytes;
        }
    }
}