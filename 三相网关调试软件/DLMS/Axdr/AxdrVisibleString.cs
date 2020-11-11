using System.Text;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS.Common;

namespace 三相智慧能源网关调试软件.DLMS.Axdr
{
    public class AxdrVisibleString : AxdrStringBase
    {
        public AxdrVisibleString()
        {
        }

        public AxdrVisibleString(string visibleString)
        {
            Value = visibleString;
        }

        public override string ToPduStringInHex()
        {
            int qty = Value.Length / 2;
            return MyConvert.EncodeVarLength(qty) + MyConvert.ByteArrayToOctetString(Encoding.Default.GetBytes(Value));
        }


        public override bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            int num = MyConvert.DecodeVarLength(ref pduStringInHex);
            if (num < 0)
            {
                return false;
            }

            if (pduStringInHex.Length < num * 2)
            {
                return false;
            }

            Value = Encoding.Default.GetString(pduStringInHex.Substring(0, num * 2).StringToByte());
            pduStringInHex = pduStringInHex.Substring((num) * 2);
            return true;
        }
    }
}