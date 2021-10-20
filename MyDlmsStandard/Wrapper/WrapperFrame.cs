using System.Text;
using MyDlmsStandard.Axdr;
using MyDlmsStandard.Common;

namespace MyDlmsStandard.Wrapper
{
    /// <summary>
    /// 47协议帧，由WrapperHeader+WrapperBody组成
    /// </summary>
    public class WrapperFrame : IToPduStringInHex, IPduStringInHexConstructor
    {
        /// <summary>
        /// 帧头
        /// </summary>
        public IWrapperHeader WrapperHeader { get; set; }

        /// <summary>
        /// 消息体
        /// </summary>
        public IWrapperBody WrapperBody { get; set; }

        public WrapperFrame(IWrapperHeader wrapperHeader)
        {
            WrapperHeader = wrapperHeader;
            WrapperBody = new WrapperBody {DataBytes = new byte[] { }};
        }

        public WrapperFrame(IWrapperHeader wrapperHeader, IToPduStringInHex pduStringInHex)
        {
            WrapperHeader = wrapperHeader;
            WrapperBody = new WrapperBody
            {
                DataBytes = pduStringInHex.ToPduStringInHex().StringToByte()
            };
        }

        public WrapperFrame()
        {
            WrapperBody = new WrapperBody {DataBytes = new byte[] { }};
        }


        public virtual string ToPduStringInHex()
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (WrapperHeader != null)
            {
                WrapperHeader.Length = new AxdrIntegerUnsigned16(WrapperBody.Length.ToString("X4"));

                stringBuilder.Append(WrapperHeader.ToPduStringInHex());
            }

            if (WrapperBody != null)
            {
                stringBuilder.Append(WrapperBody.DataBytes.ByteToString());
            }

            return stringBuilder.ToString();
        }


        public virtual bool PduStringInHexConstructor(ref string pduStringInHex)
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

            WrapperBody.DataBytes = pduStringInHex.StringToByte();

            return true;
        }

        public static WrapperFrame ParseWrapperFrame(ref string pduStringInHex)
        {
            WrapperFrame wrapper = new WrapperFrame();
            if (wrapper.PduStringInHexConstructor(ref pduStringInHex))
            {
                return wrapper;
            }

            return null;
        }
    }
}