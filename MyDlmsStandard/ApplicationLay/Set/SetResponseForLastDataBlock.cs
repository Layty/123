using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Axdr;
using System.Text;

namespace MyDlmsStandard.ApplicationLay.Set
{
    public class SetResponseForLastDataBlock : ISetResponse
    {
        public SetResponseType SetResponseType { get; } = SetResponseType.LastDataBlock;
        public AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; }
        public AxdrIntegerUnsigned8 Result { get; set; }
        public AxdrIntegerUnsigned32 BlockNumber { get; set; }

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
            Result = new AxdrIntegerUnsigned8();
            if (!Result.PduStringInHexConstructor(ref pduStringInHex))
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

        public string ToPduStringInHex()
        {

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("03");
            stringBuilder.Append(InvokeIdAndPriority.ToPduStringInHex());
            stringBuilder.Append(Result.ToPduStringInHex());
            stringBuilder.Append(BlockNumber.ToPduStringInHex());
            return stringBuilder.ToString();
        }
    }
}