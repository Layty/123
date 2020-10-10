using System.Xml.Serialization;
using 三相智慧能源网关调试软件.DLMS.Common;

namespace 三相智慧能源网关调试软件.DLMS.Axdr
{
    public class AxdrOctetString : IToPduStringInHex, IPduStringInHexConstructor
    {
        [XmlIgnore] public int Length => CalculateLength();

        private string _value;

        [XmlAttribute]
        public string Value
        {
            get => _value;
            set => _value = value;
        }

        private int CalculateLength()
        {
            int num = 0;
            if (Value != null)
            {
                num += Value.Length / 2;
            }

            return num;
        }

        public AxdrOctetString()
        {
        }

        public AxdrOctetString(string octetString)
        {
            Value = octetString;
        }

        public string ToPduStringInHex()
        {
            return MyConvert.EncodeVarLength(Length) + Value;
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            return MyConvert.VarLengthStringConstructor(ref pduStringInHex, out _value);
        }
    }
}