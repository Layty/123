using System.Text;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.Set
{
    public class SetResponse:IToPduStringInHex,IPduStringInHexConstructor
    {
        public SetResponseNormal SetResponseNormal { get; set; }

        public SetResponseDataBlock SetResponseDataBlock { get; set; }

        public SetResponseLastDataBlock SetResponseLastDataBlock { get; set; }

        public SetResponseLastDataBlockWithList SetResponseLastDataBlockWithList { get; set; }

        public SetResponseWithList SetResponseWithList { get; set; }

        public string ToPduStringInHex()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("C5");
            if (SetResponseNormal != null)
            {
                stringBuilder.Append("01");
                stringBuilder.Append(SetResponseNormal.ToPduStringInHex());
            }
            else if (SetResponseDataBlock != null)
            {
                stringBuilder.Append("02");
                stringBuilder.Append(SetResponseDataBlock.ToPduStringInHex());
            }
            else if (SetResponseLastDataBlock != null)
            {
                stringBuilder.Append("03");
                stringBuilder.Append(SetResponseLastDataBlock.ToPduStringInHex());
            }
            else if (SetResponseLastDataBlockWithList != null)
            {
                stringBuilder.Append("04");
                stringBuilder.Append(SetResponseLastDataBlockWithList.ToPduStringInHex());
            }
            else if (SetResponseWithList != null)
            {
                stringBuilder.Append("05");
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
                    SetResponseDataBlock = new SetResponseDataBlock();
                    return SetResponseDataBlock.PduStringInHexConstructor(ref pduStringInHex);
                }
                if (a == "03")
                {
                    pduStringInHex = pduStringInHex.Substring(4);
                    SetResponseLastDataBlock = new SetResponseLastDataBlock();
                    return SetResponseLastDataBlock.PduStringInHexContructor(ref pduStringInHex);
                }
                if (a == "04")
                {
                    pduStringInHex = pduStringInHex.Substring(4);
                    SetResponseLastDataBlockWithList = new SetResponseLastDataBlockWithList();
                    return SetResponseLastDataBlockWithList.PduStringInHexConstructor(ref pduStringInHex);
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