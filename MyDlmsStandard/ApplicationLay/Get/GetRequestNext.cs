using System.Xml.Serialization;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Axdr;

namespace MyDlmsStandard.ApplicationLay.Get
{
    public class GetRequestNext : IGetRequest
    {
        [XmlIgnore] public GetRequestType GetRequestType { get; } = GetRequestType.Next;
        public AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; } = new AxdrIntegerUnsigned8("C1");
        public AxdrIntegerUnsigned32 BlockNumber { get; set; }

        public GetRequestNext()
        {
        }

        public string GetRequestToPduStringInHex()
        {
            return "02" + InvokeIdAndPriority.ToPduStringInHex() + BlockNumber.ToPduStringInHex();
        }
    }
}