using System.Xml.Serialization;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Axdr;

namespace MyDlmsStandard.ApplicationLay.Get
{
    public class GetResponseNormal : IToPduStringInHex,IPduStringInHexConstructor
    {
        [XmlIgnore] public GetResponseType GetResponseType { get; set; } = GetResponseType.Normal;
        public AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; }

        public GetDataResult Result { get; set; }

        public string ToPduStringInHex()
        {
            return "01"+ InvokeIdAndPriority.ToPduStringInHex() +Result.ToPduStringInHex();
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
            Result = new GetDataResult();
            if (!Result.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }
            return true;
        }
    }
}