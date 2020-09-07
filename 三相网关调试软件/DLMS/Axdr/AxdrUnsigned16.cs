using System;
using System.Xml.Serialization;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;

namespace 三相智慧能源网关调试软件.DLMS.Axdr
{
    public class AxdrUnsigned16 :IToPduBytes, IToPduStringInHex, IPduStringInHexConstructor
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
        public ushort GetEntityValue()
        {
            if (string.IsNullOrEmpty(Value))
            {
                throw new InvalidOperationException("Value is null");
            }
            return Convert.ToUInt16(Value, 16);
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