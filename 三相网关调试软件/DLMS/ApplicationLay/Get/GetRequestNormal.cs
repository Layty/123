using System.Collections.Generic;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.Get
{
    public class GetRequestNormal : IToPduBytes
    {
        protected GetRequestType GetRequestType { get; set; } = GetRequestType.Normal;
        public InvokeIdAndPriority InvokeIdAndPriority { get; set; }
        public AttributeDescriptor AttributeDescriptor { get; set; }

        public GetRequestNormal()
        {
            InvokeIdAndPriority = new InvokeIdAndPriority(1, ServiceClass.Confirmed, Priority.High);
        }

        public GetRequestNormal(
            InvokeIdAndPriority invokeIdAndPriority, AttributeDescriptor attributeDescriptor)
        {
            AttributeDescriptor = attributeDescriptor;
            InvokeIdAndPriority = invokeIdAndPriority;
            InvokeIdAndPriority = new InvokeIdAndPriority(1, ServiceClass.Confirmed, Priority.High);
        }
        public GetRequestNormal(AttributeDescriptor attributeDescriptor)
        {
            AttributeDescriptor = attributeDescriptor;
            InvokeIdAndPriority = new InvokeIdAndPriority(1, ServiceClass.Confirmed, Priority.High);
        }

        public byte[] ToPduBytes()
        {
            List<byte> pduBytes = new List<byte>();
            pduBytes.Add((byte) GetRequestType);
            pduBytes.Add(InvokeIdAndPriority.Value);
            if (AttributeDescriptor != null)
            {
                pduBytes.AddRange(AttributeDescriptor.ToPduBytes());
            }

            return pduBytes.ToArray();
        }
    }
}