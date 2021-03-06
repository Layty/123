using System.Text;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.Get
{
    public class GetResponse : IToPduStringInHex,IPduStringInHexConstructor
    {
        public Command Command { get; set; } = Command.GetResponse;
        public GetResponseNormal GetResponseNormal { get; set; }
        public GetResponseWithDataBlock GetResponseWithDataBlock { get; set; }
        public GetResponseWithList GetResponseWithList { get; set; }

        public string ToPduStringInHex()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("C4");
            if (GetResponseNormal != null)
            {
                stringBuilder.Append("01");
                stringBuilder.Append(GetResponseNormal.ToPduStringInHex());
            }
            else if (GetResponseWithDataBlock != null)
            {
                stringBuilder.Append("02");
                stringBuilder.Append(GetResponseWithDataBlock.ToPduStringInHex());
            }
            else if (GetResponseWithList != null)
            {
                stringBuilder.Append("03");
                stringBuilder.Append(GetResponseWithList.ToPduStringInHex());
            }
            return stringBuilder.ToString();
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        { 
            if (string.IsNullOrEmpty(pduStringInHex))
            {
                return false;
            }
            string a = pduStringInHex.Substring(0, 2);
            if (a == "C4")
            {
                a = pduStringInHex.Substring(2, 2);
                if (a == "01")
                {
                    pduStringInHex = pduStringInHex.Substring(4);
                    GetResponseNormal = new GetResponseNormal();
                    return GetResponseNormal.PduStringInHexConstructor(ref pduStringInHex);
                }
                if (a == "02")
                {
                    pduStringInHex = pduStringInHex.Substring(4);
                    GetResponseWithDataBlock = new GetResponseWithDataBlock();
                    return GetResponseWithDataBlock.PduStringInHexConstructor(ref pduStringInHex);
                }
                if (a == "03")
                {
                    pduStringInHex = pduStringInHex.Substring(4);
                    GetResponseWithList = new GetResponseWithList();
                    return GetResponseWithList.PduStringInHexConstructor(ref pduStringInHex);
                }
                return false;
            }
            return false;
        }
    }
}