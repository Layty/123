using System.Collections.Generic;
using System.Xml.Serialization;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.Axdr;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.Get
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