using System.Collections.Generic;
using System.Xml.Serialization;
using ClassLibraryDLMS.DLMS.ApplicationLay.ApplicationLayEnums;
using ClassLibraryDLMS.DLMS.Axdr;

namespace ClassLibraryDLMS.DLMS.ApplicationLay.Get
{
    public class GetRequestNormal : IToPduBytes
    {
        [XmlIgnore] public GetRequestType GetRequestType { get; set; } = GetRequestType.Normal;

        public AxdrUnsigned8 InvokeIdAndPriority { get; set; }
            = new AxdrUnsigned8("C1");

        public CosemAttributeDescriptor AttributeDescriptor { get; set; }
        public SelectiveAccessDescriptor AccessSelection { get; set; }
        public CosemAttributeDescriptorWithSelection AttributeDescriptorWithSelection { get; set; }

        public GetRequestNormal()
        {
        }

        public GetRequestNormal(CosemAttributeDescriptor attributeDescriptor)
        {
            AttributeDescriptor = attributeDescriptor;
        }


        public GetRequestNormal(CosemAttributeDescriptor attributeDescriptor,
            SelectiveAccessDescriptor accessSelection)
        {
            AttributeDescriptor = attributeDescriptor;
            AccessSelection = accessSelection;
        }

        public GetRequestNormal(CosemAttributeDescriptorWithSelection attributeDescriptorWithSelection)
        {
            AttributeDescriptorWithSelection = attributeDescriptorWithSelection;
        }

        public byte[] ToPduBytes()
        {
            List<byte> pduBytes = new List<byte>();
            pduBytes.Add((byte) GetRequestType);
            pduBytes.Add(InvokeIdAndPriority.GetEntityValue());
            if (AttributeDescriptor != null)
            {
                pduBytes.AddRange(AttributeDescriptor.ToPduStringInHex().StringToByte());
                pduBytes.Add(0x00);
            }

            if (AccessSelection != null)
            {
                pduBytes.Add(0x01);
                pduBytes.AddRange(AccessSelection.ToPduStringInHex().StringToByte());
            }

            if (AttributeDescriptorWithSelection != null)
            {
                pduBytes.AddRange(AttributeDescriptorWithSelection.ToPduStringInHex().StringToByte());
            }

            return pduBytes.ToArray();
        }
    }
}