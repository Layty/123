using System.Collections.Generic;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay
{
    public class GetRequestWithList : IToPduBytes
    {
        protected GetRequestType GetRequestType { get; set; } = GetRequestType.WithList;
        public Invoke_Id_And_Priority InvokeIdAndPriority { get; set; }

        public CosemAttributeDescriptor[] CosemAttributeDescriptors { get; set; }

        public GetRequestWithList(CosemAttributeDescriptor[] cosemAttributeDescriptors)
        {
            CosemAttributeDescriptors = cosemAttributeDescriptors;
            InvokeIdAndPriority = new Invoke_Id_And_Priority(1, ServiceClass.Confirmed, Priority.High);
        }

        public byte[] ToPduBytes()
        {
            List<byte> pduBytes = new List<byte>();

            pduBytes.Add((byte) GetRequestType);
            pduBytes.Add(InvokeIdAndPriority.InvokeIdAndPriority);
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