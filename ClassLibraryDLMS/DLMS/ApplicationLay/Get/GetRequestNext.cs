using System.Collections.Generic;
using System.Xml.Serialization;
using ClassLibraryDLMS.DLMS.ApplicationLay.ApplicationLayEnums;
using ClassLibraryDLMS.DLMS.Axdr;

namespace ClassLibraryDLMS.DLMS.ApplicationLay.Get
{
    public class GetRequestNext : IToPduBytes
    {
        [XmlIgnore] protected GetRequestType GetRequestType { get; set; } = GetRequestType.Next;
        public AxdrUnsigned8 InvokeIdAndPriority { get; set; }
        public AxdrUnsigned32 BlockNumber { get; set; }

        public GetRequestNext()
        {
            InvokeIdAndPriority.Value = "C1";
        }

        public byte[] ToPduBytes()
        {
            List<byte> pduBytes = new List<byte>();
            pduBytes.Add((byte) GetRequestType);
            pduBytes.AddRange(InvokeIdAndPriority.ToPduStringInHex().StringToByte());
            pduBytes.AddRange(BlockNumber.ToPduStringInHex().StringToByte());
            return pduBytes.ToArray();
        }
    }
}