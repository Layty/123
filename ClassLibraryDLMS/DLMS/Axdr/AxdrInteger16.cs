using System;
using System.Xml.Serialization;

namespace ClassLibraryDLMS.DLMS.Axdr
{
    public class AxdrInteger16 : IToPduStringInHex, IPduStringInHexConstructor
    {
        [XmlIgnore] public int Length => 2;
        [XmlAttribute] public string Value { get; set; }
     

        public AxdrInteger16()
        {
        }

        public AxdrInteger16(string hexString)
        {
            if (hexString.Length != 4)
            {
                throw new ArgumentException("The length not match type");
            }

            Value = hexString;
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
    }
}