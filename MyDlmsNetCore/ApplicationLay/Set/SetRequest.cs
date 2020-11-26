using System.Text;
using System.Xml.Serialization;
using MyDlmsNetCore.ApplicationLay.ApplicationLayEnums;
using MyDlmsNetCore.ApplicationLay.Get;

namespace MyDlmsNetCore.ApplicationLay.Set
{
    public class SetRequest : IDlmsCommand, IToPduStringInHex
    {
        [XmlIgnore] public Command Command { get; } = Command.SetRequest;
        public SetRequestNormal SetRequestNormal { get; set; }
        public SetRequestWithFirstDataBlock SetRequestWithFirstDataBlock { get; set; }
        public SetRequestWithDataBlock SetRequestWithDataBlock { get; set; }
        public SetRequestWithList SetRequestWithList { get; set; }
        public SetRequestWithListAndFirstDatablock SetRequestWithListAndFirstDatablock { get; set; }


        public string ToPduStringInHex()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("C1");
            if (SetRequestNormal != null)
            {
                stringBuilder.Append(SetRequestNormal.ToPduStringInHex());
            }

            else if (SetRequestWithFirstDataBlock != null)
            {
                stringBuilder.Append(SetRequestWithFirstDataBlock.ToPduStringInHex());
            }
            else if (SetRequestWithDataBlock != null)
            {
                stringBuilder.Append(SetRequestWithDataBlock.ToPduStringInHex());
            }
            else if (SetRequestWithList != null)
            {
                
                stringBuilder.Append(SetRequestWithList.ToPduStringInHex());
            }
            else if (SetRequestWithListAndFirstDatablock != null)
            {
              
                stringBuilder.Append(SetRequestWithListAndFirstDatablock.ToPduStringInHex());
            }
            return stringBuilder.ToString();
        }
    }
}