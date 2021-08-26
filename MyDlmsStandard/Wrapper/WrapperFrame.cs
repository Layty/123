using System.Text;
using MyDlmsStandard.Axdr;
using MyDlmsStandard.Common;

namespace MyDlmsStandard.Wrapper
{
    /// <summary>
    /// 47协议帧，由WrapperHeader+WrapperData组成
    /// </summary>
    public class WrapperFrame : IToPduStringInHex, IPduStringInHexConstructor
    {

        /// <summary>
        /// 帧头
        /// </summary>
        public WrapperHeader WrapperHeader { get; set; }

        /// <summary>
        /// 帧内容
        /// </summary>
        public byte[] WrapperData { get; set; }


        public WrapperFrame()
        {
        }
        /// <summary>
        /// 翻转源地址和目的地址
        /// </summary>
        public void OverturnDestinationSource()
        {
            var tt = WrapperHeader.DestAddress;
            WrapperHeader.DestAddress = WrapperHeader.SourceAddress;
            WrapperHeader.SourceAddress = tt;
        }


        public string ToPduStringInHex()
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (WrapperHeader != null)
            {
                WrapperHeader.Length = new AxdrIntegerUnsigned16(WrapperData.Length.ToString("X4"));

                stringBuilder.Append(WrapperHeader.ToPduStringInHex());
            }

            if (WrapperData != null)
            {
                stringBuilder.Append(WrapperData.ByteToString());
            }

            return stringBuilder.ToString();
        }


        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            if (string.IsNullOrEmpty(pduStringInHex))
            {
                return false;
            }

            WrapperHeader = new WrapperHeader();
            if (!WrapperHeader.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            if (WrapperHeader.Length.GetEntityValue() != pduStringInHex.StringToByte().Length)
            {
                return false;
            }

            WrapperData = pduStringInHex.StringToByte();

            return true;
        }
    }
}