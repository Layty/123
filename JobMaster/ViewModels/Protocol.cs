using MyDlmsStandard;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.Association;
using MyDlmsStandard.Axdr;
using MyDlmsStandard.HDLC;
using MyDlmsStandard.HDLC.Enums;
using MyDlmsStandard.HDLC.IEC21EMode;
using MyDlmsStandard.Wrapper;


namespace JobMaster.ViewModels
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

    public class HdlcProtocol : IProtocol
    {
        public SendData SendData { get; set; }
        public Hdlc46FrameBase Hdlc46FrameBase { get; set; }
        public ProtocolInterfaceType InterfaceType => ProtocolInterfaceType.HDLC;
        public AssociationRequest AssociationRequest { get; set; }
        public AssociationResponse AssociationResponse { get; set; }

        public HdlcProtocol(byte ServerAddress, byte ClientAddress)
        {
            Hdlc46FrameBase = new Hdlc46FrameBase((byte) ServerAddress,
                (byte) ClientAddress, new DLMSInfo());
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


    /// <summary>
    ///协议层
    /// </summary>
    public class Protocol
    {
        public Hdlc46FrameBase Hdlc46FrameBase { get; set; }
        public WrapperFrame WrapperFrame { get; set; }
        public EModeFrame EModeFrame { get; set; }

        public StartProtocolType StartProtocolType { get; set; }
        public SendData SendData { get; set; }
        public DlmsSettingsViewModel DlmsSettingsViewModel { get; set; }


        public SNRMRequest SNRMRequest { get; set; }
        public AssociationRequest AssociationRequest { get; set; }
        public AssociationResponse AssociationResponse { get; set; }

        public Protocol(DlmsSettingsViewModel dlmsSettingsViewModel)
        {
            DlmsSettingsViewModel = dlmsSettingsViewModel;
            StartProtocolType = dlmsSettingsViewModel.StartProtocolType;
            Hdlc46FrameBase = new Hdlc46FrameBase((byte) dlmsSettingsViewModel.ServerAddress,
                (byte) dlmsSettingsViewModel.ClientAddress, dlmsSettingsViewModel.DlmsInfo);
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

            SNRMRequest = new SNRMRequest((byte) DlmsSettingsViewModel.ServerAddress,
                (byte) DlmsSettingsViewModel.ClientAddress, DlmsSettingsViewModel.DlmsInfo);
            AssociationRequest = new AssociationRequest(DlmsSettingsViewModel.PasswordHex,
                DlmsSettingsViewModel.MaxReceivePduSize, DlmsSettingsViewModel.DlmsVersion,
                DlmsSettingsViewModel.SystemTitle, DlmsSettingsViewModel.ProposedConformance);
            AssociationResponse = new AssociationResponse();
        }


        public byte[] BuildFinalSendData(ProtocolInterfaceType interfaceType, IToPduStringInHex Apdu)
        {
            if (interfaceType == ProtocolInterfaceType.HDLC)
            {
                Hdlc46FrameBase.Apdu = Apdu.ToPduStringInHex().StringToByte();
                SendData = new SendData(Hdlc46FrameBase);
            }
            else if (interfaceType == ProtocolInterfaceType.WRAPPER)
            {
                WrapperFrame.WrapperBody.DataBytes = Apdu.ToPduStringInHex().StringToByte();
                SendData = new SendData(WrapperFrame);
            }

            return SendData.ToPduStringInHex().StringToByte();
        }


        public byte[] TakeReplyApduFromFrame(ProtocolInterfaceType interfaceType, byte[] frameBytes)
        {
            if (frameBytes == null || frameBytes.Length == 0)
            {
                return null;
            }

            var returnPduBytes = System.Array.Empty<byte>();
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