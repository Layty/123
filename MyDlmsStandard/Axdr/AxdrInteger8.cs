using System;

namespace MyDlmsStandard.Axdr
{
    public class AxdrInteger8 :  AxdrIntegerBase<sbyte>
    {
       public override int Length => 1;

       public AxdrInteger8()
        {
        }

        public AxdrInteger8(string hexString)
        {
            int length = hexString.Length;
            if (length <= 2)
            {
                for (int i = 0; i < 2 - length; i++)
                {
                    hexString = "0" + hexString;
                }

                Value = hexString;
                return;
            }

            throw new ArgumentException("The length not match type");
        }

        public AxdrInteger8(sbyte value)
        {
            Value = value.ToString("X2");
        }

        public override sbyte GetEntityValue()
        {
            if (string.IsNullOrEmpty(Value))
            {
                throw new InvalidOperationException("Value is null");
            }

            return Convert.ToSByte(Value, 16);
        }

  

    }
}