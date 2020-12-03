using System;

namespace MyDlmsStandard.Axdr
{
    /// <summary>
    /// bool类型继承自Unsigned8,重写GetEntityValue方法 返回bool类型
    /// </summary>
    public class AxdrIntegerBoolean : AxdrIntegerUnsigned8
    {
        public AxdrIntegerBoolean()
        {
        }
        public AxdrIntegerBoolean(string hexStringValue) : base(hexStringValue)
        {
        }
        public AxdrIntegerBoolean(byte boolByte) : base(boolByte)
        {
        }

        public new bool GetEntityValue()
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