using System.Collections.Generic;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.Get
{
    public class GetRequestWithList : IToPduBytes
    {
        protected GetRequestType GetRequestType { get; set; } = GetRequestType.WithList;
        public InvokeIdAndPriority InvokeIdAndPriority { get; set; }

        public AttributeDescriptor[] CosemAttributeDescriptors { get; set; }

        public GetRequestWithList(AttributeDescriptor[] cosemAttributeDescriptors)
        {
            CosemAttributeDescriptors = cosemAttributeDescriptors;
            InvokeIdAndPriority = new InvokeIdAndPriority(1, ServiceClass.Confirmed, Priority.High);
        }

        public byte[] ToPduBytes()
        {
            List<byte> pduBytes = new List<byte>();

            pduBytes.Add((byte) GetRequestType);
            pduBytes.Add(InvokeIdAndPriority.Value);
            if (CosemAttributeDescriptors != null)
            {
                foreach (var cosemAttributeDescriptor in CosemAttributeDescriptors)
                {
                    pduBytes.AddRange(cosemAttributeDescriptor.ToPduBytes());
                }
            }

            return pduBytes.ToArray();
        }
    }
}