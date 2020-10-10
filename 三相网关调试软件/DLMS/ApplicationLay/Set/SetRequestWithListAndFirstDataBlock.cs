using System.Text;
using 三相智慧能源网关调试软件.DLMS.Axdr;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.Set
{
    public class SetRequestWithListAndFirstDatablock 
    {
        public AxdrUnsigned8 InvokeIdAndPriority { get; set; }
        public CosemAttributeDescriptorWithSelection[] AttributeDescriptorList { get; set; }
        public DataBlockSA DataBlock { get; set; }
        public string ToPduStringInHex()
        {
			StringBuilder stringBuilder = new StringBuilder();
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
            stringBuilder.Append(DataBlock.ToPduStringInHex());
            return stringBuilder.ToString();
		}
    }
}