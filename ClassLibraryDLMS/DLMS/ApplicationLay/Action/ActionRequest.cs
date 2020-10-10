using System.Collections.Generic;
using System.Xml.Serialization;
using ClassLibraryDLMS.DLMS.ApplicationLay.ApplicationLayEnums;

namespace ClassLibraryDLMS.DLMS.ApplicationLay.Action
{
    public class ActionResponse
    {

    }

    public class ActionResponseNextPBlock
    {

    }
    public class ActionResponseNormal
    {

    }
    public class ActionResponseWithList
    {

    }

    public class ActionResponseWithPBlock
    {

    }
    public class ActionRequest : IToPduBytes
    {
        [XmlIgnore] public Command Command { get; set; } = Command.MethodRequest;
        public ActionRequestNormal ActionRequestNormal { get; set; }
        public ActionRequestNextBlock ActionRequestNextBlock { get; set; }
        public ActionRequestWithList ActionRequestWithList { get; set; }
        public ActionRequestWithFirstBlock ActionRequestWithFirstBlock { get; set; }
        public ActionRequestWithListAndFirstBlock ActionRequestWithListAndFirstBlock { get; set; }
        public ActionRequestWithBlock ActionRequestWithBlock { get; set; }

    

        public byte[] ToPduBytes()
        {
            List<byte> listActionRequest=new List<byte>();
            listActionRequest.Add((byte)Command);
            if (ActionRequestNormal!=null)
            {
                listActionRequest.AddRange(ActionRequestNormal.ToPduBytes());
            }

            if (ActionRequestNextBlock!=null)
            {
                listActionRequest.AddRange(ActionRequestNextBlock.ToPduBytes());
            }
          
            if (ActionRequestWithList != null)
            {
                listActionRequest.AddRange(ActionRequestWithList.ToPduBytes());
            }

            else if (ActionRequestWithFirstBlock != null)
            {
                listActionRequest.AddRange(ActionRequestWithFirstBlock.ToPduBytes());
            }
            else if (ActionRequestWithListAndFirstBlock != null)
            {
                listActionRequest.AddRange(ActionRequestWithListAndFirstBlock.ToPduBytes());
            }
            else if (ActionRequestWithBlock != null)
            {
                listActionRequest.AddRange(ActionRequestWithBlock.ToPduBytes());
            }

            return listActionRequest.ToArray();
        }
    }
}