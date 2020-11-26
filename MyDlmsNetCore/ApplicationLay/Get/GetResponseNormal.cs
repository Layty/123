using System.Xml.Serialization;
using MyDlmsNetCore.ApplicationLay.ApplicationLayEnums;
using MyDlmsNetCore.Axdr;

namespace MyDlmsNetCore.ApplicationLay.Get
{
    public class GetResponseNormal : IToPduStringInHex, IPduStringInHexConstructor, IGetResponse
    {
        [XmlIgnore] public GetResponseType GetResponseType { get; } = GetResponseType.Normal;
        public AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; }

        public GetDataResult Result { get; set; }

        public string ToPduStringInHex()
        {
            return "01" + InvokeIdAndPriority.ToPduStringInHex() + Result.ToPduStringInHex();
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