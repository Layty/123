using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Axdr;
using System.Xml.Serialization;

namespace MyDlmsStandard.ApplicationLay.Set
{
    public interface ISetResponse
    {
        [XmlIgnore] SetResponseType SetResponseType { get; }
        AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; }
        bool PduStringInHexConstructor(ref string pduStringInHex);
    }
}