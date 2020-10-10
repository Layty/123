using System.Xml.Serialization;
using ClassLibraryDLMS.DLMS.ApplicationLay.ApplicationLayEnums;
using ClassLibraryDLMS.DLMS.Axdr;

namespace ClassLibraryDLMS.DLMS.ApplicationLay.Set
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