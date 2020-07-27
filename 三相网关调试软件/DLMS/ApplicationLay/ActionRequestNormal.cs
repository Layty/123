using System.Collections.Generic;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay
{
    public class ActionResponseNormal:IPduBytesToConstructor
    {
        public Invoke_Id_And_Priority InvokeIdAndPriority { get; set; }

        public bool PduBytesToConstructor(byte[] pduBytes)
        {
            throw new System.NotImplementedException();
        }
    }
    public class ActionRequestNormal : IToPduBytes
    {
        protected Command Command { get; set; } = Command.MethodRequest;
        protected ActionRequestType ActionRequestType { get; set; } = ActionRequestType.Normal;
        public Invoke_Id_And_Priority InvokeIdAndPriority { get; set; }
        public CosemMethodDescriptor CosemMethodDescriptor { get; set; }
        public DLMSDataItem MethodInvocationParameters { get; set; }

        public ActionRequestNormal( CosemMethodDescriptor cosemMethodDescriptor,
            DLMSDataItem methodInvocationParameters)
        {
           
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