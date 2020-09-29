using System.Xml.Serialization;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums
{
    public enum AccessMode
    {
        [XmlEnum("0")]
        NoAccess,
        [XmlEnum("1")]
        Read,
        [XmlEnum("2")]
        Write,
        [XmlEnum("3")]
        ReadWrite,
        [XmlEnum("4")]
        AuthenticatedRead,
        [XmlEnum("5")]
        AuthenticatedWrite,
        [XmlEnum("6")]
        AuthenticatedReadWrite
    }
}