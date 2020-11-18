using System;
using System.Xml.Serialization;

namespace MyDlmsStandard.Axdr
{
    public class AxdrIntegerUnsigned32 : AxdrIntegerBase
    {
        [XmlIgnore] public override int Length => 4;


        public AxdrIntegerUnsigned32()
        {
        }

        public AxdrIntegerUnsigned32(string s)
        {
            if (s.Length != 8)
            {
                throw new ArgumentException("The length not match type");
            }

            Value = s;
        }


        public uint GetEntityValue()
        {
            if (string.IsNullOrEmpty(Value))
            {
                throw new InvalidOperationException("Value is null");
            }

            return Convert.ToUInt32(Value, 16);
        }
    }
}