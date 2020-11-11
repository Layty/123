using 三相智慧能源网关调试软件.DLMS.Common;

namespace 三相智慧能源网关调试软件.DLMS.Axdr
{
    public class AxdrIntegerOctetString : AxdrStringBase
    {
        public AxdrIntegerOctetString()
        {
        }

        public AxdrIntegerOctetString(string octetString)
        {
            Value = octetString;
        }

        public override string ToPduStringInHex()
        {
            return MyConvert.EncodeVarLength(Length) + Value;
        }

        public override bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            if (MyConvert.VarLengthStringConstructor(ref pduStringInHex, out var _value))
            {
                Value = _value;
                return true;
            }

            return false;
        }
    }
}