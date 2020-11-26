using System.Text;
using MyDlmsNetCore.ApplicationLay.ApplicationLayEnums;
using MyDlmsNetCore.Axdr;

namespace MyDlmsNetCore.ApplicationLay.Set
{
    public class SetRequestWithFirstDataBlock : ISetRequest
    {
        public SetRequestType SetRequestType { get; } = SetRequestType.FirstDataBlock;

        public AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; }
        public CosemAttributeDescriptorWithSelection CosemAttributeDescriptorWithSelection { get; set; }
        public DataBlockSA DataBlockSA { get; set; }


        public string ToPduStringInHex()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("02");
            stringBuilder.Append(InvokeIdAndPriority.ToPduStringInHex());
            stringBuilder.Append(CosemAttributeDescriptorWithSelection.ToPduStringInHex());
            stringBuilder.Append(DataBlockSA.ToPduStringInHex());
            return stringBuilder.ToString();
        }
    }
}