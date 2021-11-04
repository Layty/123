namespace MyDlmsStandard.HDLC
{
    public class HdlcFrameFormatField
    {
        /// <summary>
        /// 由于使用的是类型3,format type 恒定为0b1010
        /// </summary>
        public const ushort FrameType = 0b1010;

        public bool SplitBit = false;


        public ushort FrameLengthSubField { get; set; } = 0;

        public string ToHexPdu()
        {
            ushort splitBitValue = 0;

            if (SplitBit)
            {
                splitBitValue = 1 << 12;
            }

            var data = (ushort)(FrameType << 12) + splitBitValue + FrameLengthSubField;
            return data.ToString("X4");
        }

        public bool ParsePdu()
        {
            return true;
        }
    }
}