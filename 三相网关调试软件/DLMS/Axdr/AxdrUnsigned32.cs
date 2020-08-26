using System;
using System.Xml.Serialization;

namespace 三相智慧能源网关调试软件.DLMS.Axdr
{
    public class AxdrUnsigned32
    {
        [XmlAttribute]
        public string Value { get; set; }

        [XmlIgnore]
        public int Length => 4;

        public AxdrUnsigned32()
        {
        }

        public AxdrUnsigned32(string s)
        {
            if (s.Length != 8)
            {
                throw new ArgumentException("The length not match type");
            }
            Value = s;
        }

        public string ToPduStringInHex()
        {
            return Value;
        }

        public bool PduStringInHexContructor(ref string pduStringInHex)
        {
            if (pduStringInHex.Equals(null) || pduStringInHex.Length < 8)
            {
                return false;
            }
            Value = pduStringInHex.Substring(0, 8);
            pduStringInHex = pduStringInHex.Substring(8);
            return true;
        }

        public uint GetEntityValue()
        {
            if (string.IsNullOrEmpty(Value))
            {
                throw new InvalidOperationException("Value is null");
            }
            return Convert.ToUInt32(Value, 16);
        }
    }
}