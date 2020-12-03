using System;

namespace MyDlmsStandard.Axdr
{
    public class AxdrInteger32 : AxdrIntegerBase<int>
    {
        public override int GetEntityValue()
        {
            if (string.IsNullOrEmpty(Value))
            {
                throw new InvalidOperationException("Value is null");
            }

            return Convert.ToInt32(Value, 16);
        }
    }
}