using System.Collections.Generic;
using System.Xml.Serialization;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.Set
{
    public class SetRequest : IToPduBytes, IPduBytesToConstructor
    {
        [XmlIgnore] public Command Command { get; set; } = Command.SetRequest;
        public SetRequestNormal SetRequestNormal { get; set; }
        public SetRequestWithFirstDataBlock SetRequestWithFirstDataBlock { get; set; }
        public SetRequestWithDataBlock SetRequestWithDataBlock { get; set; }
        public SetResponseDataBlock SetResponseDataBlock { get; set; }

        public byte[] ToPduBytes()
        {
            List<byte> list = new List<byte>();
            list.Add((byte) Command);
            if (SetRequestNormal != null)
            {
                list.AddRange(SetRequestNormal.ToPduBytes());
            }
            else if (SetRequestWithDataBlock != null)
            {
                list.AddRange(SetRequestWithDataBlock.ToPduBytes());
            }
            else if (SetResponseDataBlock != null)
            {
                list.AddRange(SetResponseDataBlock.ToPduBytes());
            }

            return list.ToArray();
        }

      


        public bool PduBytesToConstructor(byte[] pduBytes)
        {
            throw new System.NotImplementedException();
        }
    }
}