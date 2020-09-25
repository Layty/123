using System.Text;
using System.Xml.Serialization;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS.Common;

namespace 三相智慧能源网关调试软件.DLMS.Axdr
{
    public class AxdrVisibleString : IToPduStringInHex, IPduStringInHexConstructor
    {
        [XmlIgnore] public int Length => CalculateLength();
        [XmlAttribute] public string Value { get; set; }

        private int CalculateLength()
        {
            int num = 0;
            if (Value != null)
            {
                num += Value.Length / 2;
            }

            return num;
        }

        public AxdrVisibleString()
        {
        }

        public AxdrVisibleString(string visibleString)
        {
            Value = visibleString;
        }
       
        public string ToPduStringInHex()
        {
            int qty = Value.Length / 2;
            return MyConvert.EncodeVarLength(qty) + MyConvert.ByteArrayToOctetString(Encoding.Default.GetBytes(Value));
        }


        public bool PduStringInHexConstructor(ref string pduStringInHex)
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