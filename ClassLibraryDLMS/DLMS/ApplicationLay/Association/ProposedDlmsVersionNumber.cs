using System.Xml.Serialization;

namespace ClassLibraryDLMS.DLMS.ApplicationLay.Association
{
    public class ProposedDlmsVersionNumber
    {
        [XmlAttribute] public byte Value { get; set; }
    }
}