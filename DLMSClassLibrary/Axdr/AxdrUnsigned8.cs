using System;
using System.Xml.Serialization;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;

namespace 三相智慧能源网关调试软件.DLMS.Axdr
{
    public class AxdrUnsigned8 : IToPduBytes,IToPduStringInHex, IPduStringInHexConstructor
    {
        [XmlAttribute]
        public string Value { get; set; }

        [XmlIgnore]
        public int Length => 1;

        public AxdrUnsigned8()
        {
        }

        public AxdrUnsigned8(string s)
        {
            int length = s.Length;
            if (length <= 2)
            {
                for (int i = 0; i < 2 - length; i++)
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

        public byte[] ToPduBytes()
        {
            return ToPduStringInHex().StringToByte();
        }
    }
}