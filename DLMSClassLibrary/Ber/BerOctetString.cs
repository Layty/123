using System;
using System.Xml.Serialization;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;

namespace 三相智慧能源网关调试软件.DLMS.Ber
{
    public class BerOctetString : IToPduBytes, IToPduStringInHex, IPduStringInHexConstructor
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
            string text = pduStringInHex.Substring(0, 2);
            int num = Convert.ToInt32(text, 16);
            if (num * 2 + 2 > pduStringInHex.Length)
            {
                return false;
            }

            pduStringInHex = pduStringInHex.Substring(2);
            Value = pduStringInHex.Substring(0, num * 2);
            pduStringInHex = pduStringInHex.Substring(num * 2);
            return true;
        }

        public byte[] ToPduBytes()
        {
            return ToPduStringInHex().StringToByte();
        }
    }
}