using System;

namespace MyDlmsStandard.Axdr
{
    public class AxdrIntegerInteger8 :  AxdrIntegerBase
    {
       public override int Length => 1;
       

        public AxdrIntegerInteger8()
        {
        }

        public AxdrIntegerInteger8(sbyte value)
        {
            Value = value.ToString("X2");
        }

        public AxdrIntegerInteger8(string hexString)
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



        public sbyte GetEntityValue()
        {
            if (string.IsNullOrEmpty(Value))
            {
                throw new InvalidOperationException("Value is null");
            }

            return Convert.ToSByte(Value, 16);
        }

  

    }
}