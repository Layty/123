using MyDlmsStandard.Axdr;

namespace MyDlmsStandard.ApplicationLay.Set
{
    public class SetResponseForDataBlock:IPduStringInHexConstructor
    {
        public AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; }
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
            BlockNumber = new AxdrIntegerUnsigned32();
            if (!BlockNumber.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }
            return true;
		}
    }
}