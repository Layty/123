using System.Text;
using MyDlmsNetCore.ApplicationLay.ApplicationLayEnums;
using MyDlmsNetCore.Axdr;

namespace MyDlmsNetCore.ApplicationLay.Set
{
    public class SetResponseForDataBlock:IPduStringInHexConstructor,ISetResponse
    {
        public SetResponseType SetResponseType { get; } = SetResponseType.DataBlock;
        public AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; }
        public AxdrIntegerUnsigned32 BlockNumber { get; set; }

        public string ToPduStringInHex()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("02");
            stringBuilder.Append(InvokeIdAndPriority.ToPduStringInHex());
            stringBuilder.Append(BlockNumber.ToPduStringInHex());
            return stringBuilder.ToString();
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