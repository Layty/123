using MyDlmsStandard;
using MyDlmsStandard.ApplicationLay.Association;


namespace JobMaster.Services
{
    public interface IProtocol
    {
        SendData SendData { get; set; }
        ProtocolInterfaceType InterfaceType { get; }

        AssociationRequest AssociationRequest { get; set; }
        AssociationResponse AssociationResponse { get; set; }
        byte[] BuildFinalSendData(IToPduStringInHex Apdu);
        byte[] TakeReplyApduFromFrame(byte[] frameBytes);
    }
}