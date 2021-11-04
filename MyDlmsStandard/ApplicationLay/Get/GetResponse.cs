using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using System.Text;
using System.Xml.Serialization;

namespace MyDlmsStandard.ApplicationLay.Get
{
    public class GetResponse : IPduStringInHexConstructor, IDlmsCommand
    {
        [XmlIgnore] public Command Command => Command.GetResponse;
        public GetResponseNormal GetResponseNormal { get; set; }
        public GetResponseWithDataBlock GetResponseWithDataBlock { get; set; }
        public GetResponseWithList GetResponseWithList { get; set; }


        public string ToPduStringInHex()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("C4");
            if (GetResponseNormal != null)
            {
                stringBuilder.Append(GetResponseNormal.ToPduStringInHex());
            }
            else if (GetResponseWithDataBlock != null)
            {
                stringBuilder.Append(GetResponseWithDataBlock.ToPduStringInHex());
            }
            else if (GetResponseWithList != null)
            {
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

            string command = pduStringInHex.Substring(0, 2);
            if (command != "C4")
            {
                return false;
            }

            string a = pduStringInHex.Substring(2, 2);
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
    }
}