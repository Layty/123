using System.Collections.Generic;
using DLMSMaster.ApplicationLay;
using DLMSMaster.ApplicationLay.Enums;
using 三相智慧能源网关调试软件.DLMS.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS
{
    public class GetRequestWithList : IToPduBytes
    {
        protected Command Command { get; set; } = Command.GetRequest;
        protected GetRequestType GetRequestType { get; set; } = GetRequestType.WithList;
        public Invoke_Id_And_Priority InvokeIdAndPriority { get; set; }

        public CosemAttributeDescriptor[] CosemAttributeDescriptors { get; set; }
        public GetRequestWithList()
        {
            InvokeIdAndPriority = new Invoke_Id_And_Priority(1, ServiceClass.Confirmed, Priority.High);
        }

        public byte[] ToPduBytes()
        {
            List<byte> pduBytes = new List<byte>();
            pduBytes.Add((byte)Command);
            pduBytes.Add((byte)GetRequestType);
            pduBytes.Add(InvokeIdAndPriority.InvokeIdAndPriority);
           
           
            return pduBytes.ToArray();
        }
    }
}