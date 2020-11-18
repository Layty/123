using System.Xml.Serialization;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Axdr;

namespace MyDlmsStandard.ApplicationLay.Set
{
    public class SetRequestWithDataBlock : IToPduBytes
    {
        [XmlIgnore] protected SetRequestType SetRequestType { get; set; } = SetRequestType.WithDataBlock;
        public AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; }
        public DataBlockSA DataBlockSA { get; set; }

        public byte[] ToPduBytes()
        {
            throw new System.NotImplementedException();
        }
    }
}