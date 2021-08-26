using MyDlmsStandard.Axdr;

namespace MyDlmsStandard.Wrapper
{
    public class WrapperHeader : IToPduStringInHex, IPduStringInHexConstructor
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public AxdrIntegerUnsigned16 Version { get; set; }
        /// <summary>
        /// 源地址
        /// </summary>
        public AxdrIntegerUnsigned16 SourceAddress { get; set; }
        /// <summary>
        /// 目的地址
        /// </summary>
        public AxdrIntegerUnsigned16 DestAddress { get; set; }
        /// <summary>
        /// WrapperData帧的字节长度，与WrapperData相关
        /// </summary>
        public AxdrIntegerUnsigned16 Length { get; set; }

        /// <summary>
        /// 组帧
        /// </summary>
        /// <returns></returns>
        public string ToPduStringInHex()
        {
            return Version.ToPduStringInHex() + SourceAddress.ToPduStringInHex() + DestAddress.ToPduStringInHex() +
                   Length.ToPduStringInHex();
        }
        /// <summary>
        /// 解帧
        /// </summary>
        /// <param name="pduStringInHex"></param>
        /// <returns></returns>
        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            if (string.IsNullOrEmpty(pduStringInHex))
            {
                return false;
            }

            if (pduStringInHex.Length <= 8)
            {
                return false;
            }

            Version = new AxdrIntegerUnsigned16();
            if (!Version.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            SourceAddress = new AxdrIntegerUnsigned16();
            if (!SourceAddress.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            DestAddress = new AxdrIntegerUnsigned16();
            if (!DestAddress.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            Length = new AxdrIntegerUnsigned16();
            if (!Length.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            return true;
        }
    }
}