using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Axdr;
using System.Xml.Serialization;

namespace MyDlmsStandard.ApplicationLay.Set
{
    public interface ISetRequest
    {
        [XmlIgnore] SetRequestType SetRequestType { get; }
        AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; }

        string ToPduStringInHex();
    }
}