using System;
using System.Xml.Serialization;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;

namespace 三相智慧能源网关调试软件.DLMS.Axdr
{
    public class AxdrInteger16:IToPduBytes, IToPduStringInHex, IPduStringInHexConstructor
    {
        [XmlAttribute]
        public string Value { get; set; }

        public AxdrInteger16()
        {
        }

        public AxdrInteger16(string s)
        {
            if (s.Length != 4)
            {
                throw new ArgumentException("The length not match type");
            }
            Value = s;
        }

        public string ToPduStringInHex()
        {
            return Value;
        }


        public short GetEntityValue()
        {
            if (string.IsNullOrEmpty(Value))
            {
                throw new InvalidOperationException("Value is null");
            }
            return Convert.ToInt16(Value, 16);
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            if (pduStringInHex.Length < 4)
            {
                return false;
            }
            Value = pduStringInHex.Substring(0, 4);
            pduStringInHex = pduStringInHex.Substring(4);
            return true;
        }

        public byte[] ToPduBytes()
        {
            return ToPduStringInHex().StringToByte();
        }
    }
}