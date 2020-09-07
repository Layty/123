using System;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;

namespace 三相智慧能源网关调试软件.DLMS.Axdr
{
    public class AxdrBoolean:IToPduBytes,IToPduStringInHex,IPduStringInHexConstructor
    {
        public string Value { get; set; }

        public AxdrBoolean()
        {
            
        }
        public AxdrBoolean(string s)
        {
            if (s.Length != 2)
            {
                throw new ArgumentException("The length not match type");
            }
            Value = s;
        }
        public AxdrBoolean(byte boolByte)
        {
            Value = boolByte.ToString("X2").PadLeft(2,'0');
        }
        public bool GetEntityValue()
        {
            if (string.IsNullOrEmpty(Value))
            {
                throw new InvalidOperationException("Value is null");
            }
            if (Value == "00")
            {
                return false;
            }
            //TODO 真假待定
            if (Value=="FF")
            {
                return false;//真假待定
            }
            if (Value == "01")
            {
                return true;
            }
            throw new InvalidOperationException("Value is not a Boolean value");
        }

        public string ToPduStringInHex()
        {
            return Value;
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