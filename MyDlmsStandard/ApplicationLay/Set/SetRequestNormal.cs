using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Axdr;
using System.Text;
using System.Xml.Serialization;

namespace MyDlmsStandard.ApplicationLay.Set
{
    public class SetRequestNormal : ISetRequest
    {
        [XmlIgnore] public SetRequestType SetRequestType { get; } = SetRequestType.Normal;
        public AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; } = new AxdrIntegerUnsigned8("C1");
        protected CosemAttributeDescriptor CosemAttributeDescriptor { get; set; }

        public SelectiveAccessDescriptor AccessSelection { get; set; }

        public CosemAttributeDescriptorWithSelection CosemAttributeDescriptorWithSelection { get; set; }

        public DlmsDataItem Value { get; set; }

        public SetRequestNormal()
        {

        }
        public SetRequestNormal(CosemAttributeDescriptor cosemAttributeDescriptor,
            SelectiveAccessDescriptor accessSelection
        )
        {
            CosemAttributeDescriptor = cosemAttributeDescriptor;
            AccessSelection = accessSelection;
            CosemAttributeDescriptorWithSelection = new CosemAttributeDescriptorWithSelection(CosemAttributeDescriptor, AccessSelection);
        }

        public SetRequestNormal(CosemAttributeDescriptor cosemAttributeDescriptor, DlmsDataItem value)
        {
            CosemAttributeDescriptor = cosemAttributeDescriptor;
            Value = value;
            CosemAttributeDescriptorWithSelection = new CosemAttributeDescriptorWithSelection(CosemAttributeDescriptor, AccessSelection);


        }

        public string ToPduStringInHex()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("01");
            stringBuilder.Append(InvokeIdAndPriority.ToPduStringInHex());
            stringBuilder.Append(CosemAttributeDescriptorWithSelection.ToPduStringInHex());
            stringBuilder.Append(Value.ToPduStringInHex());
            return stringBuilder.ToString();
        }
    }
}