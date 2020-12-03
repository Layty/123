using System.Xml.Serialization;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Axdr;

namespace MyDlmsStandard.ApplicationLay
{
    public class DataAccessError
    {
        [XmlAttribute] public string Value { get; set; }
    }

    public class GetDataResult : IToPduStringInHex, IPduStringInHexConstructor
    {
        public DlmsDataItem Data { get; set; }
        [XmlIgnore] public AxdrIntegerUnsigned8 DataAccessResult { get; set; }
        public DataAccessError DataAccessError { get; set; }

        public string ToPduStringInHex()
        {
            if (Data != null)
            {
                return "00" + Data.ToPduStringInHex();
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
                Data = new DlmsDataItem();
                DataAccessResult = new AxdrIntegerUnsigned8 {Value = "00"};
                return Data.PduStringInHexConstructor(ref pduStringInHex);
            }

            if (a == "01")
            {
                pduStringInHex = pduStringInHex.Substring(2);
                DataAccessResult = new AxdrIntegerUnsigned8();
                var b1 = DataAccessResult.PduStringInHexConstructor(ref pduStringInHex);
                if (b1)
                {
                    DataAccessError = new DataAccessError();
                    DataAccessError.Value = ((DataAccessResult) DataAccessResult.GetEntityValue()).ToString();
                }

                return b1;
            }

            return false;
        }
    }
}