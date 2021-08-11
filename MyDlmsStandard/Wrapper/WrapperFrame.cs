using System.Text;
using MyDlmsStandard.Axdr;
using MyDlmsStandard.Common;

namespace MyDlmsStandard.Wrapper
{
    public class WrapperFrame : IToPduStringInHex, IPduStringInHexConstructor
    {

        public WrapperHeader WrapperHeader;

        public byte[] WrapperData { get; set; }


        public WrapperFrame()
        {
        }

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
                WrapperHeader.Length=new AxdrIntegerUnsigned16(WrapperData.Length.ToString("X4"));
             
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
            WrapperHeader=new WrapperHeader();
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