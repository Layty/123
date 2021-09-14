using System.Text;
using System.Xml.Serialization;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;

namespace MyDlmsStandard.ApplicationLay.Get
{
    public class GetRequest : IToPduStringInHex, IDlmsCommand
    {
        [XmlIgnore] public  Command Command { get; } = Command.GetRequest;
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
    }
}