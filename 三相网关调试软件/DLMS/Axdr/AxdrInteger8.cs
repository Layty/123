using System;
using System.Xml.Serialization;

namespace 三相智慧能源网关调试软件.DLMS.Axdr
{
    public class AxdrInteger8
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
        public int Length => 1;

        public AxdrInteger8()
        {
        }

        public AxdrInteger8(string s)
        {
            int length = s.Length;
            if (length <= 2)
            {
                for (int i = 0; i < 2 - length; i++)
                {
                    s = "0" + s;
                }
                value = s;
                return;
            }
            throw new ArgumentException("The length not match type");
        }

        public string ToPduStringInHex()
        {
            return value;
        }

        public bool PduStringInHexContructor(ref string pduStringInHex)
        {
            if (pduStringInHex.Equals(null) || pduStringInHex.Length < 2)
            {
                return false;
            }
            value = pduStringInHex.Substring(0, 2);
            pduStringInHex = pduStringInHex.Substring(2);
            return true;
        }

        public sbyte GetEntityValue()
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException("Value is null");
            }
            return Convert.ToSByte(value, 16);
        }
    }
}