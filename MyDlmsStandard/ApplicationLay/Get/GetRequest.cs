using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using System.Text;
using System.Xml.Serialization;

namespace MyDlmsStandard.ApplicationLay.Get
{
    public class GetRequest : IToPduStringInHex, IDlmsCommand, IPduStringInHexConstructor
    {
        [XmlIgnore] public Command Command { get; } = Command.GetRequest;
        public GetRequestNormal GetRequestNormal { get; set; }
        public GetRequestNext GetRequestNext { get; set; }
        public GetRequestWithList GetRequestWithList { get; set; }



        public string ToPduStringInHex()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("C0");

            if (GetRequestNormal != null)
            {
                stringBuilder.Append(GetRequestNormal.GetRequestToPduStringInHex());
            }
            else if (GetRequestNext != null)
            {
                stringBuilder.Append(GetRequestNext.GetRequestToPduStringInHex());
            }
            else if (GetRequestWithList != null)
            {
                stringBuilder.Append(GetRequestWithList.GetRequestToPduStringInHex());
            }

            return stringBuilder.ToString();
        }
        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            if (string.IsNullOrEmpty(pduStringInHex))
            {
                return false;
            }
            string command = pduStringInHex.Substring(0, 2);
            if (command != "C0")
            {
                return false;
            }
            string a = pduStringInHex.Substring(2, 2);
            if (a == "01")
            {
                pduStringInHex = pduStringInHex.Substring(2);
                GetRequestNormal = new GetRequestNormal();
                return GetRequestNormal.PduStringInHexConstructor(ref pduStringInHex);
            }
            //if (a == "02")
            //{
            //    pduStringInHex = pduStringInHex.Substring(4);
            //    GetRequestNext = new GetRequestNext();
            //    return GetRequestNext.PduStringInHexConstructor(ref pduStringInHex);
            //}

            //if (a == "03")
            //{
            //    pduStringInHex = pduStringInHex.Substring(4);
            //    GetRequestWithList = new GetRequestWithList();
            //    return GetResponseWithList.PduStringInHexConstructor(ref pduStringInHex);
            //}
            return true;
        }

    }
}