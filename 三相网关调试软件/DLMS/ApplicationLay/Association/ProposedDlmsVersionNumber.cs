using System.Xml.Serialization;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.Association
{
    public class ProposedDlmsVersionNumber
    {
        [XmlAttribute] public byte Value { get; set; }
    }
}