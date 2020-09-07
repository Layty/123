using System.Collections.Generic;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay
{
    public class AccessParameters:IToPduBytes
    {
        public DLMSDataItem Data { get; set; }
        public byte[] ToPduBytes()
        {
           var list= new List<byte>();
//           list.Add((byte)Data.DataType);
           list.AddRange(Data.ToPduBytes());
           return list.ToArray();
        }
    }
    public class SelectiveAccessDescriptor : IToPduBytes
    {
        public byte AccessSelector { get; set; }
        public AccessParameters AccessParameters { get; set; }

        public SelectiveAccessDescriptor(byte accessSelector , DLMSDataItem dlmsDataItem)
        {
            AccessSelector = accessSelector;
            AccessParameters=new AccessParameters(){Data = dlmsDataItem };
        }
        public byte[] ToPduBytes()
        {
            List<byte> list = new List<byte>();
            list.Add(AccessSelector);
            list.AddRange(AccessParameters.ToPduBytes());
            return list.ToArray();
        }
    }
}