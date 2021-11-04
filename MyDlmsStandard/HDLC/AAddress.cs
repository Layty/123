namespace MyDlmsStandard.HDLC
{
    /// <summary>
    /// HDLC地址，4字节，分高字节和低字节，长度可变，
    /// 源地址固定一个字节，目的地址1~4个字节
    /// 客户机端地址应固定为单一字节表示。扩展寻址的使用把客户机地址的范围限制在128。
    /// HDLC扩展寻址机制应适用于上述两种地址域。这种地址扩展规定了可变长度的地址域。
    /// </summary>
    public struct AAddress
    {
        public ushort Upper;

        public ushort Lower;

        public int Size;

        public AAddress(ushort upper, ushort lower, int size)
        {
            Upper = upper;
            Lower = lower;
            Size = size;
        }
        public byte[] ToPdu()
        {
            byte[] array = new byte[Size];
            switch (Size)
            {
                case 1:
                    array[0] = (byte)((Upper << 1) | 1);
                    break;
                case 2:
                    array[0] = (byte)(Upper << 1);
                    array[1] = (byte)((Lower << 1) | 1);
                    break;
                case 4:
                    array[0] = (byte)(Upper >> 7 << 1);
                    array[1] = (byte)((Upper & 0x7F) << 1);
                    array[2] = (byte)(Lower >> 7 << 1);
                    array[3] = (byte)(((Lower & 0x7F) << 1) | 1);
                    break;
                default:
                    array = new byte[0];
                    break;
            }

            return array;
        }

        public static AAddress FromPdu(byte[] pdu, ref int index)
        {
            AAddress result = default(AAddress);
            result.Size = 1;
            int num = index;
            while ((pdu[num++] & 1) == 0)
            {
                result.Size++;
            }

            switch (result.Size)
            {
                case 1:
                    result.Upper = (ushort)(pdu[index] >> 1);
                    break;
                case 2:
                    result.Upper = (ushort)(pdu[index] >> 1);
                    result.Lower = (ushort)(pdu[index + 1] >> 1);
                    break;
                case 4:
                    result.Upper = (ushort)((pdu[index] >> 1 << 7) | (pdu[index + 1] >> 1));
                    result.Lower = (ushort)((pdu[index + 2] >> 1 << 7) | (pdu[index + 3] >> 1));
                    break;
            }

            index += result.Size;
            return result;
        }
    }
}