using System.Linq;
using System.Xml.Serialization;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.Get
{
    public class GetResponse : IPduBytesToConstructor
    {
        [XmlIgnore] public Command Command { get; set; } = Command.GetResponse;
        public GetResponseNormal GetResponseNormal { get; set; }
        public GetResponseWithDataBlock GetResponseWithDataBlock { get; set; }
        public GetResponseWithList GetResponseWithList { get; set; }

        public bool PduBytesToConstructor(byte[] pduBytes)
        {
            if (pduBytes==null||pduBytes.Length == 0)
            {
                return false;
            }

            if (pduBytes[0] == (byte) Command)
            {
                var getResponseType = pduBytes[1];
                switch (getResponseType)
                {
                    case (byte) GetResponseType.Normal:
                        GetResponseNormal = new GetResponseNormal();
                        return GetResponseNormal.PduBytesToConstructor(pduBytes.Skip(1).ToArray());

                    case (byte) GetResponseType.WithDataBlock:
                        GetResponseWithDataBlock = new GetResponseWithDataBlock();
                        return GetResponseWithDataBlock.PduBytesToConstructor(pduBytes);
                     
                    case (byte) GetResponseType.WithList:
                        GetResponseWithList = new GetResponseWithList();
                        return GetResponseWithList.PduBytesToConstructor(pduBytes);
                     
                }
            }

            return false;
        }

    }
}