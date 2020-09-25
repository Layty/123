using System.Text;
using 三相智慧能源网关调试软件.DLMS.Axdr;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.Set
{
    public class SetResponseNormal
    {
        public AxdrUnsigned8 InvokeIdAndPriority { get; set; }
        public AxdrUnsigned8 Result { get; set; }
        public string ToPduStringInHex()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(InvokeIdAndPriority.ToPduStringInHex());
            stringBuilder.Append(Result.ToPduStringInHex());
            return stringBuilder.ToString();
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
            Result = new AxdrUnsigned8();
            if (!Result.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }
            return true;
        }

     
    }
}