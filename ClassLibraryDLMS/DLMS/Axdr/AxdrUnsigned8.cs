using System;
using System.Xml.Serialization;

namespace ClassLibraryDLMS.DLMS.Axdr
{
    public class AxdrUnsigned8 : IToPduStringInHex, IPduStringInHexConstructor
    {
        [XmlIgnore] public int Length => 1;
        [XmlAttribute] public string Value { get; set; }


        public AxdrUnsigned8()
        {
        }

        public AxdrUnsigned8(string hexStringValue)
        {
            int length = hexStringValue.Length;
            if (length <= 2)
            {
                for (int i = 0; i < 2 - length; i++)
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


        public byte GetEntityValue()
        {
            if (string.IsNullOrEmpty(Value))
            {
                throw new InvalidOperationException("Value is null");
            }

            return Convert.ToByte(Value, 16);
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