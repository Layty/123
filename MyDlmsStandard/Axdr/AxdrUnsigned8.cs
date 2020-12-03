using System;
using System.Xml.Serialization;

namespace MyDlmsStandard.Axdr
{
    public class AxdrIntegerUnsigned8 : AxdrIntegerBase<byte>
    {
        [XmlIgnore] public override int Length => 1;


        public AxdrIntegerUnsigned8()
        {
        }

        public AxdrIntegerUnsigned8(string hexStringValue)
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

        public AxdrIntegerUnsigned8(byte unsigned8Value)
        {
            Value = unsigned8Value.ToString("X2");
        }

        public override byte GetEntityValue()
        {
            if (string.IsNullOrEmpty(Value))
            {
                throw new InvalidOperationException("Value is null");
            }

            return Convert.ToByte(Value, 16);
        }
    }
}