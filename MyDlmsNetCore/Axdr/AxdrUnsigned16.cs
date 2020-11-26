using System;

namespace MyDlmsNetCore.Axdr
{
    public class AxdrIntegerUnsigned16 : AxdrIntegerBase
    {
        public override int Length => 2;

        public AxdrIntegerUnsigned16()
        {
        }

        public AxdrIntegerUnsigned16(string hexStringValue)
        {
            int length = hexStringValue.Length;
            if (length <= 4)
            {
                for (int i = 0; i < 4 - length; i++)
                {
                    hexStringValue = "0" + hexStringValue;
                }

                Value = hexStringValue;
                return;
            }

            throw new ArgumentException("The length not match type");
        }



        public ushort GetEntityValue()
        {
            if (string.IsNullOrEmpty(Value))
            {
                throw new InvalidOperationException("Value is null");
            }

            return Convert.ToUInt16(Value, 16);
        }
    }
}