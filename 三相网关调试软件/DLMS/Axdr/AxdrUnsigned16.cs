using System;
using System.Xml.Serialization;

namespace 三相智慧能源网关调试软件.DLMS.Axdr
{
    public class AxdrUnsigned16
    {
        [XmlAttribute]
        public string Value { get; set; }

        [XmlIgnore]
        public int Length => 2;

        public AxdrUnsigned16()
        {
        }

        public AxdrUnsigned16(string s)
        {
            int length = s.Length;
            if (length <= 4)
            {
                for (int i = 0; i < 4 - length; i++)
                {
                    s = "0" + s;
                }
                Value = s;
                return;
            }
            throw new ArgumentException("The length not match type");
        }

        public string ToPduStringInHex()
        {
            return Value;
        }

        public bool PduStringInHexContructor(ref string pduStringInHex)
        {
            if (pduStringInHex.Equals(null) || pduStringInHex.Length < 4)
            {
                return false;
            }
            Value = pduStringInHex.Substring(0, 4);
            pduStringInHex = pduStringInHex.Substring(4);
            return true;
        }

        public ushort GetEntityValue()
        {
            if (string.IsNullOrEmpty(Value))
            {
                throw new InvalidOperationException("Value is null");
            }
            return Convert.ToUInt16(Value, 16);
        }
    }
}