using System.Xml.Serialization;

namespace MyDlmsStandard.ApplicationLay.Association
{
    public class ProposedDlmsVersionNumber
    {
        [XmlAttribute] public byte Value { get; set; }
    }
}