using System.Collections.Generic;
using System.Xml.Serialization;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.Get
{
    public class GetRequest : IToPduBytes
    {
       [XmlIgnore] protected Command Command { get; set; } = Command.GetRequest;
        public GetRequestNormal GetRequestNormal { get; set; }
        public GetRequestNext GetRequestNext { get; set; }
        public GetRequestWithList GetRequestWithList { get; set; }

        public byte[] ToPduBytes()
        {
            List<byte> list = new List<byte>();
            list.Add((byte) Command);
            if (GetRequestNormal != null)
            {
                list.AddRange(GetRequestNormal.ToPduBytes());
            }
            else if (GetRequestNext != null)
            {
                list.AddRange(GetRequestNext.ToPduBytes());
            }
            else if (GetRequestWithList != null)
            {
                list.AddRange(GetRequestWithList.ToPduBytes());
            }

            return list.ToArray();
        }
    }
}