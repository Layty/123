using System.Text;
using System.Xml.Serialization;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.Action
{
    public class ActionRequest : IToPduStringInHex
    {
        [XmlIgnore] public Command Command { get; set; } = Command.MethodRequest;
        public ActionRequestNormal ActionRequestNormal { get; set; }
        public ActionRequestNextBlock ActionRequestNextBlock { get; set; }
        public ActionRequestWithList ActionRequestWithList { get; set; }
        public ActionRequestWithFirstBlock ActionRequestWithFirstBlock { get; set; }
        public ActionRequestWithListAndFirstBlock ActionRequestWithListAndFirstBlock { get; set; }
        public ActionRequestWithBlock ActionRequestWithBlock { get; set; }

        public string ToPduStringInHex()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("C3");
            if (ActionRequestNormal != null)
            {
                stringBuilder.Append("01");
                stringBuilder.Append(ActionRequestNormal.ToPduStringInHex());
            }
            else if (ActionRequestNextBlock != null)
            {
                stringBuilder.Append("02");
                stringBuilder.Append(ActionRequestNextBlock.ToPduStringInHex());
            }
            else if (ActionRequestWithList != null)
            {
                stringBuilder.Append("03");
                stringBuilder.Append(ActionRequestWithList.ToPduStringInHex());
            }
            else if (ActionRequestWithFirstBlock != null)
            {
                stringBuilder.Append("04");
                stringBuilder.Append(ActionRequestWithFirstBlock.ToPduStringInHex());
            }
            else if (ActionRequestWithListAndFirstBlock != null)
            {
                stringBuilder.Append("05");
                stringBuilder.Append(ActionRequestWithListAndFirstBlock.ToPduStringInHex());
            }
            else if (ActionRequestWithBlock != null)
            {
                stringBuilder.Append("06");
                stringBuilder.Append(ActionRequestWithBlock.ToPduStringInHex());
            }

            return stringBuilder.ToString();
        }
    }
}