using System;
using System.Collections.Generic;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay
{
    public class GetDataResult
    {
        public DLMSDataItem Data { get; set; }
        public ErrorCode DataAccessResult { get; set; }
    }

    public class GetRequestNormal : IToPduBytes
    {
        protected GetRequestType GetRequestType { get; set; } = GetRequestType.Normal;
        public Invoke_Id_And_Priority InvokeIdAndPriority { get; set; }
        public CosemAttributeDescriptor CosemAttributeDescriptor { get; set; }

        public GetRequestNormal(CosemAttributeDescriptor cosemAttributeDescriptor,
            Invoke_Id_And_Priority invokeIdAndPriority)
        {
            CosemAttributeDescriptor = cosemAttributeDescriptor;
            InvokeIdAndPriority = invokeIdAndPriority;
        }

        public byte[] ToPduBytes()
        {
            List<byte> pduBytes = new List<byte>();
           
            pduBytes.Add((byte) GetRequestType);
            pduBytes.Add(InvokeIdAndPriority.InvokeIdAndPriority);
            if (CosemAttributeDescriptor != null)
            {
                pduBytes.AddRange(CosemAttributeDescriptor.ToPduBytes());
            }

            return pduBytes.ToArray();
        }
    }
}