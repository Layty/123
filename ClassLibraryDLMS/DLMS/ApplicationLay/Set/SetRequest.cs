using System.Collections.Generic;
using System.Xml.Serialization;
using ClassLibraryDLMS.DLMS.ApplicationLay.ApplicationLayEnums;

namespace ClassLibraryDLMS.DLMS.ApplicationLay.Set
{
    public class SetRequest : IToPduBytes
    {
        [XmlIgnore] public Command Command { get; set; } = Command.SetRequest;
        public SetRequestNormal SetRequestNormal { get; set; }
        public SetRequestWithFirstDataBlock SetRequestWithFirstDataBlock { get; set; }
        public SetRequestWithDataBlock SetRequestWithDataBlock { get; set; }

        public SetRequestWithList SetRequestWithList { get; set; }

        public SetRequestWithListAndFirstDatablock SetRequestWithListAndFirstDatablock { get; set; }


        public byte[] ToPduBytes()
        {
            List<byte> list = new List<byte>();
            list.Add((byte) Command);
            if (SetRequestNormal != null)
            {
                list.AddRange(SetRequestNormal.ToPduBytes());
            }
            else if (SetRequestWithFirstDataBlock != null)
            {
                list.AddRange(SetRequestWithFirstDataBlock.ToPduBytes());
            }
            else if (SetRequestWithDataBlock != null)
            {
                list.AddRange(SetRequestWithDataBlock.ToPduBytes());
            }
            else if (SetRequestWithList != null)
            {
                list.AddRange(SetRequestWithList.ToPduStringInHex().StringToByte());
            }
            else if (SetRequestWithListAndFirstDatablock != null)
            {
                list.AddRange(SetRequestWithListAndFirstDatablock.ToPduStringInHex().StringToByte());
            }

            return list.ToArray();
        }
    }
}