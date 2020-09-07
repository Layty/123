using System.Xml.Serialization;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;
using 三相智慧能源网关调试软件.DLMS.Common;

namespace 三相智慧能源网关调试软件.DLMS.Axdr
{
    public class AxdrOctetString :IToPduBytes,IToPduStringInHex, IPduStringInHexConstructor
    {


        private string _value;

        [XmlAttribute]
        public string Value
        {
            get => _value;
            set => _value = value;
        }



        [XmlIgnore]
        public int Length => CalculateLength();

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

        public AxdrOctetString(string s)
        {
            Value = s;
        }

        public string ToPduStringInHex()
        {
            int qty = Value.Length / 2;
            return MyConvert.EncodeVarLength(qty) + Value;
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            return MyConvert.VarLengthStringConstructor(ref pduStringInHex, out _value);
        }

        public byte[] ToPduBytes()
        {
            return ToPduStringInHex().StringToByte();
        }
    }
}