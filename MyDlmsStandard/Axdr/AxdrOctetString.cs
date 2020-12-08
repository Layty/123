using MyDlmsStandard.Common;

namespace MyDlmsStandard.Axdr
{
    public class AxdrOctetString : AxdrStringBase
    {
        public AxdrOctetString()
        {
        }

        public AxdrOctetString(string octetString)
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