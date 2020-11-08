using System.Collections.Generic;
using System.Xml.Serialization;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.Action
{
    public class ActionRequestNormal :  IToPduBytes
    {
        [XmlIgnore] protected ActionRequestType ActionRequestType { get; set; } = ActionRequestType.Normal;
        public InvokeIdAndPriority InvokeIdAndPriority { get; set; }
    
        public CosemMethodDescriptor CosemMethodDescriptor { get; set; }

        public DlmsDataItem MethodInvocationParameters { get; set; }

        public ActionRequestNormal(CosemMethodDescriptor cosemMethodDescriptor,
            DlmsDataItem methodInvocationParameters)
        {
            CosemMethodDescriptor = cosemMethodDescriptor;
            MethodInvocationParameters = methodInvocationParameters;
            InvokeIdAndPriority = new InvokeIdAndPriority(1, ServiceClass.Confirmed, Priority.High);
        }

        public ActionRequestNormal(CosemMethodDescriptor cosemMethodDescriptor)
        {
            CosemMethodDescriptor = cosemMethodDescriptor;
            InvokeIdAndPriority = new InvokeIdAndPriority(1, ServiceClass.Confirmed, Priority.High);
        }

        public byte[] ToPduBytes()
        {
            List<byte> pduBytes = new List<byte>();
            
            pduBytes.Add((byte) ActionRequestType);
            pduBytes.Add(InvokeIdAndPriority.Value);
            if (CosemMethodDescriptor != null)
            {
                pduBytes.AddRange(CosemMethodDescriptor.ToPduStringInHex().StringToByte());
            }

            if (MethodInvocationParameters != null)
            {
                pduBytes.Add(0x01);
                pduBytes.AddRange( (MethodInvocationParameters.ToPduBytes()));
            }
            else
            {
                pduBytes.Add(0x00);
            }

            return pduBytes.ToArray();
        }

      
    }
}