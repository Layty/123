using System;
using System.Xml.Serialization;

namespace MyDlmsStandard.Axdr
{
    public class AxdrIntegerUnsigned32 : AxdrIntegerBase<uint>
    {
        [XmlIgnore] public override int Length => 4;


        public AxdrIntegerUnsigned32()
        {
        }
        public AxdrIntegerUnsigned32(uint uintValue)
        {
            Value = uintValue.ToString("X8");
        }
        public AxdrIntegerUnsigned32(string hexString)
        {
            if (hexString.Length != 8)
            {
                throw new ArgumentException("The length not match type");
            }

            Value = hexString;
        }


        public override uint GetEntityValue()
        {
            if (string.IsNullOrEmpty(Value))
            {
                throw new InvalidOperationException("Value is null");
            }

            return Convert.ToUInt32(Value, 16);
        }
    }
}