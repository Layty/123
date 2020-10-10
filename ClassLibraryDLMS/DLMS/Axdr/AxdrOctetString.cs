using System.Xml.Serialization;
using ClassLibraryDLMS.DLMS.Common;

namespace ClassLibraryDLMS.DLMS.Axdr
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