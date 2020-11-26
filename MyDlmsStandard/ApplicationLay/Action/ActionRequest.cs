using System.Text;
using System.Xml.Serialization;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;

namespace MyDlmsStandard.ApplicationLay.Action
{
    public class ActionRequest :IToPduStringInHex
    {
        [XmlIgnore] public Command Command { get; set; } = Command.MethodRequest;
        public ActionRequestNormal ActionRequestNormal { get; set; }
        public ActionRequestNextBlock ActionRequestNextBlock { get; set; }
        public ActionRequestWithList ActionRequestWithList { get; set; }
        public ActionRequestWithFirstBlock ActionRequestWithFirstBlock { get; set; }
        public ActionRequestWithListAndFirstBlock ActionRequestWithListAndFirstBlock { get; set; }
        public ActionRequestWithBlock ActionRequestWithBlock { get; set; }

      
//        public byte[] ToPduBytes()
//        {
//            List<byte> listActionRequest=new List<byte>();
//            listActionRequest.Add((byte)Command);
//            if (ActionRequestNormal!=null)
//            {
//                listActionRequest.AddRange(ActionRequestNormal.ToPduBytes());
//            }
//
//            if (ActionRequestNextBlock!=null)
//            {
//                listActionRequest.AddRange(ActionRequestNextBlock.ToPduBytes());
//            }
//          
//            if (ActionRequestWithList != null)
//            {
//                listActionRequest.AddRange(ActionRequestWithList.ToPduBytes());
//            }
//
//            else if (ActionRequestWithFirstBlock != null)
//            {
//                listActionRequest.AddRange(ActionRequestWithFirstBlock.ToPduBytes());
//            }
//            else if (ActionRequestWithListAndFirstBlock != null)
//            {
//                listActionRequest.AddRange(ActionRequestWithListAndFirstBlock.ToPduBytes());
//            }
//            else if (ActionRequestWithBlock != null)
//            {
//                listActionRequest.AddRange(ActionRequestWithBlock.ToPduBytes());
//            }
//
//            return listActionRequest.ToArray();
//        }

        public string ToPduStringInHex()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("C3");
            if (ActionRequestNormal != null)
            {
                stringBuilder.Append(ActionRequestNormal.ToPduStringInHex());
            }
//            else if (ActionRequestNextBlock != null)
//            {
//                stringBuilder.Append("02");
//                stringBuilder.Append(ActionRequestNextBlock.ToPduStringInHex());
//            }
//            else if (ActionRequestWithList != null)
//            {
//                stringBuilder.Append("03");
//                stringBuilder.Append(ActionRequestWithList.ToPduStringInHex());
//            }
//            else if (ActionRequestWithFirstPblock != null)
//            {
//                stringBuilder.Append("04");
//                stringBuilder.Append(actionRequestWithFirstPblock.ToPduStringInHex());
//            }
//            else if (actionRequestWithListAndFirstPblock != null)
//            {
//                stringBuilder.Append("05");
//                stringBuilder.Append(actionRequestWithListAndFirstPblock.ToPduStringInHex());
//            }
//            else if (actionRequestWithPblock != null)
//            {
//                stringBuilder.Append("06");
//                stringBuilder.Append(actionRequestWithPblock.ToPduStringInHex());
//            }
            return stringBuilder.ToString();
        }
    }
}