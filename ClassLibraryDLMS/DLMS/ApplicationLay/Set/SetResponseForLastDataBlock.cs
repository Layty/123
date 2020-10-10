using ClassLibraryDLMS.DLMS.Axdr;

namespace ClassLibraryDLMS.DLMS.ApplicationLay.Set
{
    public class SetResponseForLastDataBlock
    {
        public AxdrUnsigned8 InvokeIdAndPriority { get; set; }
        public AxdrUnsigned8 Result { get; set; }
        public AxdrUnsigned32 BlockNumber { get; set; }

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
            Result = new AxdrUnsigned8();
            if (!Result.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }
            BlockNumber = new AxdrUnsigned32();
            if (!BlockNumber.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }
            return true;
		}

        public char ToPduStringInHex()
        {
            throw new System.NotImplementedException();
        }
    }
}