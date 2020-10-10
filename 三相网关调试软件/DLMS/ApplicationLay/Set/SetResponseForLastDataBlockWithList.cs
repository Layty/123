using 三相智慧能源网关调试软件.DLMS.Axdr;
using 三相智慧能源网关调试软件.DLMS.Common;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.Set
{
    public class SetResponseForLastDataBlockWithList:IPduStringInHexConstructor
    {
        public AxdrUnsigned8 InvokeIdAndPriority { get; set; }
        public AxdrUnsigned8[] Result { get; set; }
        public AxdrUnsigned32 BlockNumber { get; set; }
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
            InvokeIdAndPriority = new AxdrUnsigned8();
            if (!InvokeIdAndPriority.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }
            int num = MyConvert.DecodeVarLength(ref pduStringInHex);
            Result = new AxdrUnsigned8[num];
            for (int i = 0; i < num; i++)
            {
                if (string.IsNullOrEmpty(pduStringInHex))
                {
                    return false;
                }
                Result[i] = new AxdrUnsigned8();
                if (!Result[i].PduStringInHexConstructor(ref pduStringInHex))
                {
                    return false;
                }
            }
            BlockNumber = new AxdrUnsigned32();
            if (!BlockNumber.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }
            return true;
		}
    }
}