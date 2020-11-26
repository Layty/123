using System.Text;
using MyDlmsNetCore.ApplicationLay.ApplicationLayEnums;
using MyDlmsNetCore.Axdr;
using MyDlmsNetCore.Common;

namespace MyDlmsNetCore.ApplicationLay.Set
{
    public class SetResponseForLastDataBlockWithList:ISetResponse,IPduStringInHexConstructor
    {
        public SetResponseType SetResponseType { get; } = SetResponseType.LastDataBlockWithList;
        public AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; }
        public AxdrIntegerUnsigned8[] Result { get; set; }
        public AxdrIntegerUnsigned32 BlockNumber { get; set; }
        public string ToPduStringInHex()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("04");
            stringBuilder.Append(InvokeIdAndPriority.ToPduStringInHex());
            int num = Result.Length;
            if (num <= 127)
            {
                stringBuilder.Append(num.ToString("X2"));
            }
            else if (num <= 255)
            {
                stringBuilder.Append("81" + num.ToString("X2"));
            }
            else
            {
                stringBuilder.Append("82" + num.ToString("X4"));
            }
            AxdrIntegerUnsigned8[] array = Result;
            foreach (AxdrIntegerUnsigned8 axdrUnsigned in array)
            {
                stringBuilder.Append(axdrUnsigned.ToPduStringInHex());
            }
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