using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using System.Collections.Generic;

namespace MyDlmsStandard.HDLC
{
    public class DisConnectRequest : Hdlc46FrameBase
    {
        public Command Command { get; set; } = Command.DisconnectRequest;

        public DisConnectRequest(byte destAddress1, byte sourceAddress1) : base(destAddress1, sourceAddress1)
        {
        }

        public DisConnectRequest(ushort destAddress1, byte sourceAddress1) : base(destAddress1, sourceAddress1)
        {
        }
        public override byte[] ToPduBytes()
        {
            List<byte> disConnect = new List<byte>();
            PackagingDestinationAndSourceAddress(disConnect);
            disConnect.Add((byte)Command);
            byte count = (byte)(disConnect.Count + 2 + 2); //FCS=2,FrameFormatField=2 ,目的地址=1/2/4，源地址=1
            disConnect.InsertRange(0, GetFrameFormatField(count));
            PackingFcs_And_FrameStartEnd(disConnect);
            InitFrameSequenceNumber();
            return disConnect.ToArray();
        }
    }
}