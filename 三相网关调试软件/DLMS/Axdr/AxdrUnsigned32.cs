using System;
using System.Xml.Serialization;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;

namespace 三相智慧能源网关调试软件.DLMS.Axdr
{
    public class AxdrUnsigned32 : IToPduBytes,IToPduStringInHex, IPduStringInHexConstructor
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

       

        public uint GetEntityValue()
        {
            if (string.IsNullOrEmpty(Value))
            {
                throw new InvalidOperationException("Value is null");
            }
            return Convert.ToUInt32(Value, 16);
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            if (pduStringInHex.Length < 8)
            {
                return false;
            }
            Value = pduStringInHex.Substring(0, 8);
            pduStringInHex = pduStringInHex.Substring(8);
            return true;
        }

        public byte[] ToPduBytes()
        {
            return ToPduStringInHex().StringToByte();
        }
    }
}