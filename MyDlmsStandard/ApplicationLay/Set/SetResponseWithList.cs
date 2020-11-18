using MyDlmsStandard.Axdr;
using MyDlmsStandard.Common;

namespace MyDlmsStandard.ApplicationLay.Set
{
    public class SetResponseWithList : IToPduStringInHex, IPduStringInHexConstructor
    {
        public AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; }
        public AxdrIntegerUnsigned8[] Result { get; set; }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            if (string.IsNullOrEmpty(pduStringInHex))
            {
                return false;
            }

            InvokeIdAndPriority = new AxdrIntegerUnsigned8();
            if (!InvokeIdAndPriority.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            int num = MyConvert.DecodeVarLength(ref pduStringInHex);
            Result = new AxdrIntegerUnsigned8[num];
            for (int i = 0; i < num; i++)
            {
                if (string.IsNullOrEmpty(pduStringInHex))
                {
                    return false;
                }

                Result[i] = new AxdrIntegerUnsigned8();
                if (!Result[i].PduStringInHexConstructor(ref pduStringInHex))
                {
                    return false;
                }
            }

            return true;
        }

        public string ToPduStringInHex()
        {
            throw new System.NotImplementedException();
        }
    }
}