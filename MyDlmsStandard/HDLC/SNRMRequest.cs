using System.Collections.Generic;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;

namespace MyDlmsStandard.HDLC
{
    public class SNRMRequest : Hdlc46FrameBase
    {
        public Command Command { get; set; } = Command.Snrm;
        public bool SnrmContainInfoFlag { get; set; } = true;
        public DLMSInfo DlmsInfo { get; set; }

        public SNRMRequest(byte destAddress1, byte sourceAddress1, DLMSInfo dlmsInfo) : base(destAddress1,
            sourceAddress1)
        {
            DlmsInfo = dlmsInfo;
        }

        public SNRMRequest(ushort destAddress1, byte sourceAddress1, DLMSInfo dlmsInfo) : base(destAddress1,
            sourceAddress1)
        {
            DlmsInfo = dlmsInfo;
        }

        public override byte[] ToPduBytes()
        {
            InitFrameSequenceNumber();
            List<byte> snrmFrame = new List<byte>();
            PackagingDestinationAndSourceAddress(snrmFrame);
            //  LastCommand = Command.Snrm;
            snrmFrame.Add((byte)Command);
            byte[] snrmInfo = { };
            byte hcs = 0;
            if (SnrmContainInfoFlag)
            {
                hcs = 2;
                snrmInfo = DlmsInfo.GetSnrmInfo();
            }

            //不包含头尾2个0x7E
            //FrameFormatField=2，,1/2/4个字节的目的地址字节数，  command=1个字节， hcs字节=2，snrminfo=12, 2个字节FCS,
            int count = 2 + DestAddress1.Size + 1 + 1 + hcs + snrmInfo.Length + Fcs.Length;
            snrmFrame.InsertRange(0, GetFrameFormatField(count));

            if (SnrmContainInfoFlag)
            {
                PackingHcs(snrmFrame);
                snrmFrame.AddRange(DlmsInfo.GetSnrmInfo());
            }

            PackingFcs_And_FrameStartEnd(snrmFrame);

            return snrmFrame.ToArray();
        }
    }
}