using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS.Axdr;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay
{
    public class GetDataResult:IToPduStringInHex,IPduStringInHexConstructor
    {
        public DLMSDataItem Data { get; set; }
        public AxdrUnsigned8 DataAccessResult { get; set; }

        public string ToPduStringInHex()
        {
            if (Data != null)
            {
                return "00" + Data.ToPduBytes().ByteToString("");
            }

            return "01" + DataAccessResult;
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            if (string.IsNullOrEmpty(pduStringInHex))
            {
                return false;
            }
            string a = pduStringInHex.Substring(0, 2);
            if (a == "00")
            {
                pduStringInHex = pduStringInHex.Substring(2);
                Data = new DLMSDataItem();
                DataAccessResult = new AxdrUnsigned8();
                DataAccessResult.Value = "00";
                return Data.PduStringInHexConstructor(ref pduStringInHex);
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