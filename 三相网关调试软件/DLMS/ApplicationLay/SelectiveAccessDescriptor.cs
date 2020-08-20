using System.Collections.Generic;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay
{
    public class SelectiveAccessDescriptor : IToPduBytes
    {
        public byte AccessSelector { get; set; }
        public DLMSDataItem DlmsDataItem { get; set; }

        public byte[] ToPduBytes()
        {
            List<byte> list = new List<byte>();
            list.Add(AccessSelector);
            list.AddRange(DlmsDataItem.ToPduBytes());
            return list.ToArray();
        }
    }
}