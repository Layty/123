using System.Text;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Axdr;
using MyDlmsStandard.Common;

namespace MyDlmsStandard.ApplicationLay.Set
{
    public class SetResponseWithList :ISetResponse
    {
        public SetResponseType SetResponseType { get; } = SetResponseType.WithList;
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
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("05");
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
            return stringBuilder.ToString();
        }
    }
}