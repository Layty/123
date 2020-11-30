using System.Text;
using System.Xml.Serialization;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.Get;

namespace MyDlmsStandard.ApplicationLay.Set
{
    public class SetResponse : IToPduStringInHex, IPduStringInHexConstructor, IDlmsCommand
    {
        [XmlIgnore] public Command Command { get; } = Command.SetResponse;
        public SetResponseNormal SetResponseNormal { get; set; }

        public SetResponseForDataBlock SetResponseForDataBlock { get; set; }

        public SetResponseForLastDataBlock SetResponseForLastDataBlock { get; set; }

        public SetResponseForLastDataBlockWithList SetResponseForLastDataBlockWithList { get; set; }

        public SetResponseWithList SetResponseWithList { get; set; }

        public string ToPduStringInHex()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("C5");
            if (SetResponseNormal != null)
            {
                stringBuilder.Append(SetResponseNormal.ToPduStringInHex());
            }
            else if (SetResponseForDataBlock != null)
                stringBuilder.Append(SetResponseForDataBlock.ToPduStringInHex());
            else if (SetResponseForLastDataBlock != null)
            {
                stringBuilder.Append(SetResponseForLastDataBlock.ToPduStringInHex());
            }
            else if (SetResponseForLastDataBlockWithList != null)
            {
                stringBuilder.Append(SetResponseForLastDataBlockWithList.ToPduStringInHex());
            }
            else if (SetResponseWithList != null)
            {
                stringBuilder.Append(SetResponseWithList.ToPduStringInHex());
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
            if (a == "C5")
            {
                a = pduStringInHex.Substring(2, 2);
                if (a == "01")
                {
                    pduStringInHex = pduStringInHex.Substring(4);
                    SetResponseNormal = new SetResponseNormal();
                    return SetResponseNormal.PduStringInHexConstructor(ref pduStringInHex);
                }

                if (a == "02")
                {
                    pduStringInHex = pduStringInHex.Substring(4);
                    SetResponseForDataBlock = new SetResponseForDataBlock();
                    return SetResponseForDataBlock.PduStringInHexConstructor(ref pduStringInHex);
                }

                if (a == "03")
                {
                    pduStringInHex = pduStringInHex.Substring(4);
                    SetResponseForLastDataBlock = new SetResponseForLastDataBlock();
                    return SetResponseForLastDataBlock.PduStringInHexConstructor(ref pduStringInHex);
                }

                if (a == "04")
                {
                    pduStringInHex = pduStringInHex.Substring(4);
                    SetResponseForLastDataBlockWithList = new SetResponseForLastDataBlockWithList();
                    return SetResponseForLastDataBlockWithList.PduStringInHexConstructor(ref pduStringInHex);
                }

                if (a == "05")
                {
                    pduStringInHex = pduStringInHex.Substring(4);
                    SetResponseWithList = new SetResponseWithList();
                    return SetResponseWithList.PduStringInHexConstructor(ref pduStringInHex);
                }

                return false;
            }

            return false;
        }
    }
}