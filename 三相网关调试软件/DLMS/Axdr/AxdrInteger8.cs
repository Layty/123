using System;
using System.Xml.Serialization;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;

namespace 三相智慧能源网关调试软件.DLMS.Axdr
{
    public class AxdrInteger8 :  IToPduStringInHex, IPduStringInHexConstructor
    {
        [XmlIgnore] public int Length => 1;
        [XmlAttribute] public string Value { get; set; }

        public AxdrInteger8()
        {
        }

        public AxdrInteger8(sbyte value)
        {
            Value = value.ToString("X2");
        }

        public AxdrInteger8(string hexString)
        {
            int length = hexString.Length;
            if (length <= 2)
            {
                for (int i = 0; i < 2 - length; i++)
                {
                    hexString = "0" + hexString;
                }

                Value = hexString;
                return;
            }

            throw new ArgumentException("The length not match type");
        }

        public string ToPduStringInHex()
        {
            return Value;
        }


        public sbyte GetEntityValue()
        {
            if (string.IsNullOrEmpty(Value))
            {
                throw new InvalidOperationException("Value is null");
            }

            return Convert.ToSByte(Value, 16);
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            if (pduStringInHex.Length < 2)
            {
                return false;
            }

            Value = pduStringInHex.Substring(0, 2);
            pduStringInHex = pduStringInHex.Substring(2);
            return true;
        }

    }
}