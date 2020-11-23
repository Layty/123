using System.Text;
using System.Xml.Serialization;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Axdr;

namespace MyDlmsStandard.ApplicationLay.Get
{
    public class GetRequestNormal : IGetRequest, IToPduStringInHex
    {
        [XmlIgnore] public GetRequestType GetRequestType { get; } = GetRequestType.Normal;

        public AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; }
            = new AxdrIntegerUnsigned8("C1");

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

//        public byte[] ToPduBytes()
//        {
//            List<byte> pduBytes = new List<byte>();
//            pduBytes.Add((byte) GetRequestType);
//            pduBytes.Add(InvokeIdAndPriority.GetEntityValue());
//            if (AttributeDescriptor != null)
//            {
//                pduBytes.AddRange(AttributeDescriptor.ToPduStringInHex().StringToByte());
//                pduBytes.Add(0x00);
//            }
//
//            if (AccessSelection != null)
//            {
//                pduBytes.Add(0x01);
//                pduBytes.AddRange(AccessSelection.ToPduStringInHex().StringToByte());
//            }
//
//            if (AttributeDescriptorWithSelection != null)
//            {
//                pduBytes.AddRange(AttributeDescriptorWithSelection.ToPduStringInHex().StringToByte());
//            }
//
//            return pduBytes.ToArray();
//        }

        public string ToPduStringInHex()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("01");
            if (InvokeIdAndPriority != null)
            {
                stringBuilder.Append(InvokeIdAndPriority.ToPduStringInHex());
            }

            if (AttributeDescriptor != null)
            {
                stringBuilder.Append(AttributeDescriptor.ToPduStringInHex());
                stringBuilder.Append(0x00);
            }

            if (AccessSelection != null)
            {
                stringBuilder.Append(0x01);
                stringBuilder.Append(AccessSelection.ToPduStringInHex());
            }

            if (AttributeDescriptorWithSelection != null)
            {
                stringBuilder.Append(AttributeDescriptorWithSelection.ToPduStringInHex());
            }

            return stringBuilder.ToString();
        }
    }
}