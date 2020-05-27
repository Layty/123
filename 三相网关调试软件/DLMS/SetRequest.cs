using System.Collections.Generic;
using 三相智慧能源网关调试软件.DLMS.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS
{
    public class SetRequest : IToPduBytes
    {
        protected Command Command { get; set; } = Command.SetRequest;
        protected SetRequestType SetRequestType { get; set; }
        public Invoke_Id_And_Priority InvokeIdAndPriority { get; set; }
        protected CosemAttributeDescriptor CosemAttributeDescriptor { get; set; }

        public DLMSDataItem AttributeInvocationParameters { get; set; }
        public SetRequest(CosemAttributeDescriptor cosemAttributeDescriptor, DLMSDataItem attributeInvocationParameters,
            SetRequestType setRequestType = SetRequestType.Normal)
        {
            CosemAttributeDescriptor = cosemAttributeDescriptor;
            AttributeInvocationParameters= attributeInvocationParameters;
            SetRequestType = setRequestType;
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
            pduBytes.Add((byte)AttributeInvocationParameters.DataType);
            //TO DO 
            if (AttributeInvocationParameters.DataType == DataType.OctetString) //这里要重写判断机制，根据数据类型
            {
                pduBytes.Add((byte)AttributeInvocationParameters.ValueBytes.Length);
            }
            pduBytes.AddRange(AttributeInvocationParameters.ValueBytes);

            return pduBytes.ToArray();
        }
    }
}