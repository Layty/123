using System.Collections.Generic;
using DLMSMaster.ApplicationLay.Enums;

namespace DLMSMaster.ApplicationLay.GetRequest
{
    public class GetRequestNormal : IToPduBytes
    {
        protected Command Command { get; set; } = Command.GetRequest;
        protected GetRequestType GetRequestType { get; set; }
        public Invoke_Id_And_Priority InvokeIdAndPriority { get; set; }
        public CosemAttributeDescriptor CosemAttributeDescriptor { get; set; }

        public GetRequestNormal(CosemAttributeDescriptor cosemAttributeDescriptor,
            GetRequestType getRequestType = GetRequestType.Normal)
        {
            GetRequestType = getRequestType;
            CosemAttributeDescriptor = cosemAttributeDescriptor;
            InvokeIdAndPriority = new Invoke_Id_And_Priority(1, ServiceClass.Confirmed, Priority.High);
        }

        public byte[] ToPduBytes()
        {
            List<byte> pduBytes = new List<byte>();
            pduBytes.Add((byte) Command);
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