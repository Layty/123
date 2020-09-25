using System;
using System.Xml.Serialization;

namespace 三相智慧能源网关调试软件.DLMS.Axdr
{
    public class AxdrUnsigned16 : IToPduStringInHex, IPduStringInHexConstructor
    {
        [XmlIgnore] public int Length => 2;
        [XmlAttribute] public string Value { get; set; }


        public AxdrUnsigned16()
        {
        }

        public AxdrUnsigned16(string hexStringValue)
        {
            int length = hexStringValue.Length;
            if (length <= 4)
            {
                for (int i = 0; i < 4 - length; i++)
                {
                    hexStringValue = "0" + hexStringValue;
                }

                Value = hexStringValue;
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
    }
}