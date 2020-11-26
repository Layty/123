using System;
using System.Xml.Serialization;

namespace MyDlmsNetCore.Axdr
{
    public class AxdrIntegerUnsigned8 : AxdrIntegerBase
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


        public byte GetEntityValue()
        {
            if (string.IsNullOrEmpty(Value))
            {
                throw new InvalidOperationException("Value is null");
            }

            return Convert.ToByte(Value, 16);
        }
    }
}