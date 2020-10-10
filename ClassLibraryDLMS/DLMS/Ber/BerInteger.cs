using System;
using System.Xml.Serialization;

namespace ClassLibraryDLMS.DLMS.Ber
{
    public class BerInteger : IToPduStringInHex, IPduStringInHexConstructor
    {
        [XmlAttribute] public string Value { get; set; }

        public string ToPduStringInHex()
        {
            if (Value.Length % 2 != 0)
            {
                return "";
            }

            return (Value.Length / 2).ToString("X2") + Value;
        }


        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            int num = Convert.ToInt32(pduStringInHex.Substring(0, 2), 16);
            if ((num + 1) * 2 > pduStringInHex.Length)
            {
                return false;
            }

            Value = pduStringInHex.Substring(2, num * 2);
            pduStringInHex = pduStringInHex.Substring((num + 1) * 2);
            return true;
        }
    }
}