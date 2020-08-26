using System;
using System.Xml.Serialization;

namespace 三相智慧能源网关调试软件.DLMS.Axdr
{
    public class AxdrInteger16
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

        public AxdrInteger16()
        {
        }

        public AxdrInteger16(string s)
        {
            if (s.Length != 4)
            {
                throw new ArgumentException("The length not match type");
            }
            value = s;
        }

        public string ToPduStringInHex()
        {
            return value;
        }

        public bool PduStringInHexContructor(ref string pduStringInHex)
        {
            if (pduStringInHex.Equals(null) || pduStringInHex.Length < 4)
            {
                return false;
            }
            value = pduStringInHex.Substring(0, 4);
            pduStringInHex = pduStringInHex.Substring(4);
            return true;
        }

        public short GetEntityValue()
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException("Value is null");
            }
            return Convert.ToInt16(value, 16);
        }
    }
}