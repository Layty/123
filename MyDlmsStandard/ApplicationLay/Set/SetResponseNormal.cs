using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Axdr;
using System.Text;
using System.Xml.Serialization;

namespace MyDlmsStandard.ApplicationLay.Set
{
    public class SetResponseNormal : ISetResponse
    {
        [XmlIgnore] public SetResponseType SetResponseType { get; set; }//需要标注XmlIgnore，不然XML序列化时报错
        public AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; }
        private AxdrIntegerUnsigned8 _result;
        public DataAccessResult Result { get; set; }

        public string ToPduStringInHex()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("01");
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

            InvokeIdAndPriority = new AxdrIntegerUnsigned8();
            if (!InvokeIdAndPriority.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            _result = new AxdrIntegerUnsigned8();
            if (!_result.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            Result = (DataAccessResult)_result.GetEntityValue();


            return true;
        }
    }
}