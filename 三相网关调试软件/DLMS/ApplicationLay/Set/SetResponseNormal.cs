using System.Text;
using System.Xml.Serialization;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.Axdr;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.Set
{
    public class SetResponseNormal : IToPduStringInHex, IPduStringInHexConstructor
    {
        public AxdrUnsigned8 InvokeIdAndPriority { get; set; }
        private AxdrUnsigned8 _result { get; set; }
        public DataAccessResult Result { get; set; }

        public string ToPduStringInHex()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(InvokeIdAndPriority.ToPduStringInHex());
            stringBuilder.Append(_result.ToPduStringInHex());
            return stringBuilder.ToString();
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            if (string.IsNullOrEmpty(pduStringInHex))
            {
                return false;
            }

            InvokeIdAndPriority = new AxdrUnsigned8();
            if (!InvokeIdAndPriority.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            _result = new AxdrUnsigned8();
            if (!_result.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }
            else
            {
                Result = (DataAccessResult) _result.GetEntityValue();
            }


            return true;
        }
    }
}