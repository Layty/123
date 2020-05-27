using System.Collections.Generic;
using 三相智慧能源网关调试软件.DLMS.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS
{
    public class ActionRequest : IToPduBytes
    {
        protected Command Command { get; set; } = Command.MethodRequest;
        protected ActionRequestType ActionRequestType { get; set; }
        public Invoke_Id_And_Priority InvokeIdAndPriority { get; set; }
        public CosemMethodDescriptor CosemMethodDescriptor { get; set; }
        public DLMSDataItem MethodInvocationParameters { get; set; }

        public ActionRequest( CosemMethodDescriptor cosemMethodDescriptor,
            DLMSDataItem methodInvocationParameters,ActionRequestType actionRequestType=ActionRequestType.Normal)
        {
            ActionRequestType = actionRequestType;
            CosemMethodDescriptor = cosemMethodDescriptor;
            MethodInvocationParameters = methodInvocationParameters;
            InvokeIdAndPriority = new Invoke_Id_And_Priority(1, ServiceClass.Confirmed, Priority.High);
        }

        public byte[] ToPduBytes()
        {
            List<byte> pduBytes = new List<byte>();
            pduBytes.Add((byte) Command);
            pduBytes.Add((byte) ActionRequestType);
            pduBytes.Add(InvokeIdAndPriority.InvokeIdAndPriority);
            if (CosemMethodDescriptor != null)
            {
                pduBytes.AddRange(CosemMethodDescriptor.ToPduBytes());
            }

            if (MethodInvocationParameters != null)
            {
                pduBytes.AddRange(MethodInvocationParameters.ToPduBytes());
            }

            return pduBytes.ToArray();
        }
    }

   
}