using System.Xml.Serialization;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.Axdr;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.Set
{
    public class SetRequestWithDataBlock : IToPduBytes
    {
        [XmlIgnore] protected SetRequestType SetRequestType { get; set; } = SetRequestType.WithDataBlock;
        public AxdrUnsigned8 InvokeIdAndPriority { get; set; }
        public DataBlockSA DataBlockSA { get; set; }

        public byte[] ToPduBytes()
        {
            throw new System.NotImplementedException();
        }
    }
}