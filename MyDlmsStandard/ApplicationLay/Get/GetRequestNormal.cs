using System.Text;
using System.Xml.Serialization;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Axdr;

namespace MyDlmsStandard.ApplicationLay.Get
{
    public class GetRequestNormal : IGetRequest
    {
        [XmlIgnore] public GetRequestType GetRequestType { get; } = GetRequestType.Normal;

        public AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; } = new AxdrIntegerUnsigned8("C1");

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



        public GetRequestNormal(CosemAttributeDescriptorWithSelection attributeDescriptorWithSelection)
        {
            AttributeDescriptorWithSelection = attributeDescriptorWithSelection;
        }

        public string GetRequestToPduStringInHex()
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
                stringBuilder.Append("00");
            }

            if (AccessSelection != null)
            {
                stringBuilder.Append("01");
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