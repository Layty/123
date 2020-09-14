using System.Linq;
using System.Text;
using 三相智慧能源网关调试软件.DLMS.Axdr;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay
{
    public class DataBlockG:IToPduStringInHex,IPduStringInHexConstructor
    {
        public AxdrBoolean LastBlock { get; set; }
        public AxdrUnsigned32 BlockNumber { get; set; }
        public AxdrOctetString RawData { get; set; }
        public AxdrUnsigned8 DataAccessResult { get; set; }


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
            LastBlock = new AxdrBoolean();
            if (!LastBlock.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }
            BlockNumber = new AxdrUnsigned32();
            if (!BlockNumber.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }
            string a = pduStringInHex.Substring(0, 2);
            if (a == "00")
            {
                pduStringInHex = pduStringInHex.Substring(2);
                RawData = new AxdrOctetString();
                return RawData.PduStringInHexConstructor(ref pduStringInHex);
            }
            if (a == "01")
            {
                pduStringInHex = pduStringInHex.Substring(2);
                DataAccessResult = new AxdrUnsigned8();
                return DataAccessResult.PduStringInHexConstructor(ref pduStringInHex);
            }
            return false;
        }
    }
}