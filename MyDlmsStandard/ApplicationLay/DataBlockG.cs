using System.Text;
using MyDlmsStandard.Axdr;

namespace MyDlmsStandard.ApplicationLay
{
    public class DataBlockG:IToPduStringInHex,IPduStringInHexConstructor
    {
        public AxdrIntegerBoolean LastBlock { get; set; }
        public AxdrIntegerUnsigned32 BlockNumber { get; set; }
        public AxdrIntegerOctetString RawData { get; set; }
        public AxdrIntegerUnsigned8 DataAccessResult { get; set; }


        public string ToPduStringInHex()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(LastBlock.ToPduStringInHex());
            stringBuilder.Append(BlockNumber.ToPduStringInHex());
            if (DataAccessResult != null)
            {
                stringBuilder.Append("01");
                stringBuilder.Append(DataAccessResult.ToPduStringInHex());
            }
            else if (RawData != null)
            {
                stringBuilder.Append("00");
                stringBuilder.Append(RawData.ToPduStringInHex());
            }
            return stringBuilder.ToString();
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            if (string.IsNullOrEmpty(pduStringInHex))
            {
                return false;
            }
            LastBlock = new AxdrIntegerBoolean();
            if (!LastBlock.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }
            BlockNumber = new AxdrIntegerUnsigned32();
            if (!BlockNumber.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }
            string a = pduStringInHex.Substring(0, 2);
            if (a == "00")
            {
                pduStringInHex = pduStringInHex.Substring(2);
                RawData = new AxdrIntegerOctetString();
                return RawData.PduStringInHexConstructor(ref pduStringInHex);
            }
            if (a == "01")
            {
                pduStringInHex = pduStringInHex.Substring(2);
                DataAccessResult = new AxdrIntegerUnsigned8();
                return DataAccessResult.PduStringInHexConstructor(ref pduStringInHex);
            }
            return false;
        }
    }
}