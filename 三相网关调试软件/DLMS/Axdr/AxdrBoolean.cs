using System;

namespace 三相智慧能源网关调试软件.DLMS.Axdr
{
    public class AxdrIntegerBoolean : AxdrIntegerBase
    {
        public override int Length => 1;

        public AxdrIntegerBoolean()
        {
        }

        public AxdrIntegerBoolean(string hexStringValue)
        {
            if (hexStringValue.Length != 2)
            {
                throw new ArgumentException("The length not match type");
            }

            Value = hexStringValue;
        }

        public AxdrIntegerBoolean(byte boolByte)
        {
            Value = boolByte.ToString("X2").PadLeft(2, '0');
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
            if (Value == "FF")
            {
                return false; //真假待定
            }

            if (Value == "01")
            {
                return true;
            }

            throw new InvalidOperationException("Value is not a Boolean value");
        }
    }
}