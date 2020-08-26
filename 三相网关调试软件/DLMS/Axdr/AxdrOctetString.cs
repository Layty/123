using System.Xml.Serialization;
using 三相智慧能源网关调试软件.DLMS.Common;

namespace 三相智慧能源网关调试软件.DLMS.Axdr
{
    public class AxdrOctetString
    {
        private string value;

        [XmlAttribute]
        public string Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }

        [XmlIgnore]
        public int Length => CalculateLength();

        private int CalculateLength()
        {
            int num = 0;
            if (value != null)
            {
                num += value.Length / 2;
            }
            return num;
        }

        public AxdrOctetString()
        {
        }

        public AxdrOctetString(string s)
        {
            value = s;
        }

        public string ToPduStringInHex()
        {
            int qty = value.Length / 2;
            return MyConvert.EncodeVarLength(qty) + value;
        }

        public bool PduStringInHexContructor(ref string pduStringInHex)
        {
            return MyConvert.VarLengthStringConstructor(ref pduStringInHex, out value);
        }
    }
}