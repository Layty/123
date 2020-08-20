using System.Collections.Generic;
using System.Xml.Serialization;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.Set
{
    public class SetRequestNormal : IToPduBytes,IPduBytesToConstructor
    {
        [XmlIgnore] protected SetRequestType SetRequestType { get; set; } = SetRequestType.Normal;
        public InvokeIdAndPriority InvokeIdAndPriority { get; set; }
        protected AttributeDescriptor AttributeDescriptor { get; set; }

        public DLMSDataItem AttributeInvocationParameters { get; set; }

        public SetRequestNormal(AttributeDescriptor attributeDescriptor, DLMSDataItem attributeInvocationParameters
        )
        {
            AttributeDescriptor = attributeDescriptor;
            AttributeInvocationParameters = attributeInvocationParameters;

            InvokeIdAndPriority = new InvokeIdAndPriority(1, ServiceClass.Confirmed, Priority.High);
        }

        public byte[] ToPduBytes()
        {
            List<byte> pduBytes = new List<byte>();
            pduBytes.Add((byte) SetRequestType);
            pduBytes.Add(InvokeIdAndPriority.Value);
            if (AttributeDescriptor != null)
            {
                pduBytes.AddRange(AttributeDescriptor.ToPduBytes());
            }

            pduBytes.AddRange(AttributeInvocationParameters.ToPduBytes());
            return pduBytes.ToArray();
        }

        public bool PduBytesToConstructor(byte[] pduBytes)
        {
            throw new System.NotImplementedException();
        }
    }
}