using MyDlmsStandard.ApplicationLay;

namespace MyDlmsStandard.HDLC
{
    public class IFrame : Hdlc46FrameBase {
        public IFrame(byte destAddress1, byte sourceAddress1, DLMSInfo dlmsInfo) : base(destAddress1, sourceAddress1, dlmsInfo)
        {
        }

        public IFrame(ushort destAddress1, byte sourceAddress1, DLMSInfo dlmsInfo) : base(destAddress1, sourceAddress1, dlmsInfo)
        {
        }

        public IFrame(byte destAddress1, byte sourceAddress1) : base(destAddress1, sourceAddress1)
        {
        }

        public IFrame(ushort destAddress1, byte sourceAddress1) : base(destAddress1, sourceAddress1)
        {
        }
    }
}