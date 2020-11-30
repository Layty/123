using System.Xml.Serialization;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Axdr;

namespace MyDlmsStandard.ApplicationLay.Set
{
    public interface ISetRequest
    {
        [XmlIgnore] SetRequestType SetRequestType { get; }
        AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; }

        string ToPduStringInHex();
    }
}