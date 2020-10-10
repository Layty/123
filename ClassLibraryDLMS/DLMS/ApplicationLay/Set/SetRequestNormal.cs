using System.Collections.Generic;
using System.Xml.Serialization;
using ClassLibraryDLMS.DLMS.ApplicationLay.ApplicationLayEnums;

namespace ClassLibraryDLMS.DLMS.ApplicationLay.Set
{
    public class SetRequestNormal : IToPduBytes
    {
        [XmlIgnore] protected SetRequestType SetRequestType { get; set; } = SetRequestType.Normal;
        public InvokeIdAndPriority InvokeIdAndPriority { get; set; }
        protected CosemAttributeDescriptor CosemAttributeDescriptor { get; set; }

        public SelectiveAccessDescriptor AccessSelection { get; set; }

        public DLMSDataItem Value { get; set; }

        public SetRequestNormal()
        {
            
        }
        public SetRequestNormal(CosemAttributeDescriptor cosemAttributeDescriptor,
            SelectiveAccessDescriptor accessSelection
        )
        {
            CosemAttributeDescriptor = cosemAttributeDescriptor;
            AccessSelection = accessSelection;
            InvokeIdAndPriority = new InvokeIdAndPriority(1, ServiceClass.Confirmed, Priority.High);
        }

        public SetRequestNormal(CosemAttributeDescriptor cosemAttributeDescriptor, DLMSDataItem value)
        {
            CosemAttributeDescriptor = cosemAttributeDescriptor;
            Value = value;
            InvokeIdAndPriority = new InvokeIdAndPriority(1, ServiceClass.Confirmed, Priority.High);
        }

        public byte[] ToPduBytes()
        {
            List<byte> pduBytes = new List<byte>();
            pduBytes.Add((byte) SetRequestType);
            pduBytes.Add(InvokeIdAndPriority.Value);
            if (CosemAttributeDescriptor != null)
            {
                pduBytes.AddRange(CosemAttributeDescriptor.ToPduStringInHex().StringToByte());
            }

            if (AccessSelection != null)
            {
                pduBytes.Add(0x01);
                pduBytes.AddRange(AccessSelection.ToPduStringInHex().StringToByte());
            }
            else
            {
                pduBytes.Add(0x00);
            }

            if (Value != null)
            {
                pduBytes.AddRange(Value.ToPduBytes());
            }

            return pduBytes.ToArray();
        }
    }
}