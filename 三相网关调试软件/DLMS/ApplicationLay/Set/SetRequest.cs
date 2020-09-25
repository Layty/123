using System.Collections.Generic;
using System.Xml.Serialization;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.Set
{
    public class SetRequest : IToPduBytes
    {
        [XmlIgnore] public Command Command { get; set; } = Command.SetRequest;
        public SetRequestNormal SetRequestNormal { get; set; }
        public SetRequestWithFirstDataBlock SetRequestWithFirstDataBlock { get; set; }
        public SetRequestWithDataBlock SetRequestWithDataBlock { get; set; }

        public SetRequestWithList SetRequestWithList { get; set; }

        public SetRequestWithListAndFirstDataBlock SetRequestWithListAndFirstDataBlock { get; set; }


        public byte[] ToPduBytes()
        {
            List<byte> list = new List<byte>();
            list.Add((byte) Command);
            if (SetRequestNormal != null)
            {
                list.AddRange(SetRequestNormal.ToPduBytes());
            }
            else if (SetRequestWithFirstDataBlock != null)
            {
                list.AddRange(SetRequestWithFirstDataBlock.ToPduBytes());
            }
            else if (SetRequestWithDataBlock != null)
            {
                list.AddRange(SetRequestWithDataBlock.ToPduBytes());
            }
            else if (SetRequestWithList != null)
            {
                list.AddRange(SetRequestWithList.ToPduBytes());
            }
            else if (SetRequestWithListAndFirstDataBlock != null)
            {
                list.AddRange(SetRequestWithListAndFirstDataBlock.ToPduBytes());
            }

            return list.ToArray();
        }
    }
}