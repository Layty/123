using MyDlmsStandard.Axdr;
using MyDlmsStandard.Common;

namespace MyDlmsStandard.ApplicationLay.Set
{
    public class SetResponseForLastDataBlockWithList:IPduStringInHexConstructor
    {
        public AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; }
        public AxdrIntegerUnsigned8[] Result { get; set; }
        public AxdrIntegerUnsigned32 BlockNumber { get; set; }
        public char ToPduStringInHex()
        {
            throw new System.NotImplementedException();
        }

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
            BlockNumber = new AxdrIntegerUnsigned32();
            if (!BlockNumber.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }
            return true;
		}
    }
}