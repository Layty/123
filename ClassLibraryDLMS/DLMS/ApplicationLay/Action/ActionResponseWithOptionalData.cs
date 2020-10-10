using System.Text;
using ClassLibraryDLMS.DLMS.Axdr;

namespace ClassLibraryDLMS.DLMS.ApplicationLay.Action
{
    public class ActionResponseWithOptionalData:IToPduStringInHex,IPduStringInHexConstructor
    {
        public AxdrUnsigned8 Result { get; set; }
        public GetDataResult ReturnParameters { get; set; }
        public string ToPduStringInHex()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Result.ToPduStringInHex());
            if (ReturnParameters != null)
            {
                stringBuilder.Append("01");
                stringBuilder.Append(ReturnParameters.ToPduStringInHex());
            }
            else
            {
                stringBuilder.Append("00");
            }
            return stringBuilder.ToString();
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            if (string.IsNullOrEmpty(pduStringInHex))
            {
                return false;
            }
            Result = new AxdrUnsigned8();
            if (!Result.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }
            string a = pduStringInHex.Substring(0, 2);
            pduStringInHex = pduStringInHex.Substring(2);
            if (a == "00")
            {
                ReturnParameters = null;
                return true;
            }
            ReturnParameters = new GetDataResult();
            return ReturnParameters.PduStringInHexConstructor(ref pduStringInHex);
        }
    }
}