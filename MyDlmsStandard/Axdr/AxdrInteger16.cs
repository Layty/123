using System;
using System.Xml.Serialization;

namespace MyDlmsStandard.Axdr
{
    public class AxdrIntegerInteger16 : AxdrIntegerBase<short>
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
        public AxdrIntegerInteger16(short shortValue)
        {
            Value = shortValue.ToString("X4");
        }

        public override short GetEntityValue()
        {
            if (string.IsNullOrEmpty(Value))
            {
                throw new InvalidOperationException("Value is null");
            }

            return Convert.ToInt16(Value, 16);
        }
    }
}