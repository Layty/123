using System;

namespace MyDlmsNetCore.Axdr
{
    /// <summary>
    /// 包含长度的OctetString
    /// </summary>
    public class AxdrOctetStringFixed : AxdrStringBase
    {
        private int size = 1;

        public AxdrOctetStringFixed()
        {
        }

        public AxdrOctetStringFixed(int fixLength)
        {
            size = fixLength;
        }

        public AxdrOctetStringFixed(string s, int fixLength)
        {
            if (s.Length != fixLength * 2)
            {
                throw new ArgumentException("The length not match value");
            }

            size = fixLength;
            Value = s;
        }

        public override string ToPduStringInHex()
        {
            size = Value.Length / 2;
            return Value;
        }

        public override bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            if (pduStringInHex.Length < size * 2)
            {
                return false;
            }

            Value = pduStringInHex.Substring(0, size * 2);
            pduStringInHex = pduStringInHex.Substring(size * 2);
            return true;
        }
    }
}