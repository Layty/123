using System;
using System.Xml.Serialization;

namespace 三相智慧能源网关调试软件.DLMS.Axdr
{
    public class AxdrIntegerInteger16 : AxdrIntegerBase
    {
        [XmlIgnore] public override int Length => 2;


        public AxdrIntegerInteger16()
        {
        }

        public AxdrIntegerInteger16(string hexString)
        {
            if (hexString.Length != 4)
            {
                throw new ArgumentException("The length not match type");
            }

            Value = hexString;
        }


        public short GetEntityValue()
        {
            if (string.IsNullOrEmpty(Value))
            {
                throw new InvalidOperationException("Value is null");
            }

            return Convert.ToInt16(Value, 16);
        }
    }
}