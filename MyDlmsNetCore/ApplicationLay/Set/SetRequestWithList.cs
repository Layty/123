using System.Text;
using MyDlmsNetCore.ApplicationLay.ApplicationLayEnums;
using MyDlmsNetCore.Axdr;

namespace MyDlmsNetCore.ApplicationLay.Set
{
    public class SetRequestWithList:ISetRequest
    {
        public SetRequestType SetRequestType { get; } = SetRequestType.WithList;
        public AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; }
        public CosemAttributeDescriptorWithSelection[] AttributeDescriptorList { get; set; }
        public DlmsDataItem[] ValueList { get; set; }
        public string ToPduStringInHex()
        {
			StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("04");
            stringBuilder.Append(InvokeIdAndPriority.ToPduStringInHex());
            int num = AttributeDescriptorList.Length;
            if (num <= 127)
            {
                stringBuilder.Append(num.ToString("X2"));
            }
            else if (num <= 255)
            {
                stringBuilder.Append("81" + num.ToString("X2"));
            }
            else
            {
                stringBuilder.Append("82" + num.ToString("X4"));
            }
            CosemAttributeDescriptorWithSelection[] array = AttributeDescriptorList;
            foreach (CosemAttributeDescriptorWithSelection cosemAttributeDescriptorWithSelection in array)
            {
                stringBuilder.Append(cosemAttributeDescriptorWithSelection.ToPduStringInHex());
            }
            num = ValueList.Length;
            if (num <= 127)
            {
                stringBuilder.Append(num.ToString("X2"));
            }
            else if (num <= 255)
            {
                stringBuilder.Append("81" + num.ToString("X2"));
            }
            else
            {
                stringBuilder.Append("82" + num.ToString("X4"));
            }
            DlmsDataItem[] array2 = ValueList;
            foreach (DlmsDataItem dlmsDataItem in array2)
            {
                stringBuilder.Append(dlmsDataItem.ToPduStringInHex());
            }
            return stringBuilder.ToString();
		}
    }
}