using System.Xml.Serialization;

namespace MyDlmsNetCore.ApplicationLay.Association
{
    public class ProposedDlmsVersionNumber
    {
        [XmlAttribute] public byte Value { get; set; }
    }
}