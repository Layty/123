using System.Text;
using System.Xml.Serialization;
using ClassLibraryDLMS.DLMS.Common;

namespace ClassLibraryDLMS.DLMS.Axdr
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