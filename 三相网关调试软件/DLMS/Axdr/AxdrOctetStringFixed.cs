using System;
using System.Xml.Serialization;

namespace 三相智慧能源网关调试软件.DLMS.Axdr
{
    /// <summary>
    /// 不包含长度的OctetString
    /// </summary>
    public class AxdrOctetStringFixed
    {
        private int size = 1;

        [XmlAttribute]
        public string Value { get; set; }

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

        public AxdrOctetStringFixed()
        {
        }

        public AxdrOctetStringFixed(int fixLength)
        {
            size = fixLength;
        }

        public AxdrOctetStringFixed(string s, int fixLength)
        {
            if (s.Length != fixLength * 2)
            {
                throw new ArgumentException("The length not match value");
            }
            size = fixLength;
            Value = s;
        }

        public string ToPduStringInHex()
        {
            size = Value.Length / 2;
            return Value;
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            if (pduStringInHex.Length < size * 2)
            {
                return false;
            }
            Value = pduStringInHex.Substring(0, size * 2);
            pduStringInHex = pduStringInHex.Substring(size * 2);
            return true;
        }
    }
}