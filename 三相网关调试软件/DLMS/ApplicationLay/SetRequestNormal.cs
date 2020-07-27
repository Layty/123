using System.Collections.Generic;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay
{

    public class SetRequestWithDataBlock
    {
        protected Command Command { get; set; } = Command.SetRequest;
        protected SetRequestType SetRequestType { get; set; } = SetRequestType.WithDataBlock;
        public Invoke_Id_And_Priority InvokeIdAndPriority { get; set; }

    }
    public class SetRequestNormal : IToPduBytes
    {
        protected Command Command { get; set; } = Command.SetRequest;
        protected SetRequestType SetRequestType { get; set; } = SetRequestType.Normal;
        public Invoke_Id_And_Priority InvokeIdAndPriority { get; set; }
        protected CosemAttributeDescriptor CosemAttributeDescriptor { get; set; }

        public DLMSDataItem AttributeInvocationParameters { get; set; }
        public SetRequestNormal(CosemAttributeDescriptor cosemAttributeDescriptor, DLMSDataItem attributeInvocationParameters
            )
        {
            CosemAttributeDescriptor = cosemAttributeDescriptor;
            AttributeInvocationParameters= attributeInvocationParameters;
            
            InvokeIdAndPriority = new Invoke_Id_And_Priority(1, ServiceClass.Confirmed, Priority.High);
        }

        public byte[] ToPduBytes()
        {
            List<byte> pduBytes = new List<byte>();
            pduBytes.Add((byte) Command);
            pduBytes.Add((byte) SetRequestType);
            pduBytes.Add(InvokeIdAndPriority.InvokeIdAndPriority);
            if (CosemAttributeDescriptor != null)
            {
                pduBytes.AddRange(CosemAttributeDescriptor.ToPduBytes());
            }
            pduBytes.AddRange(AttributeInvocationParameters.ToPduBytes());
            return pduBytes.ToArray();
        }
    }
}