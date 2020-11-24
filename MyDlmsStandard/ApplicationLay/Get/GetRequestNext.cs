using System.Xml.Serialization;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Axdr;

namespace MyDlmsStandard.ApplicationLay.Get
{
    public class GetRequestNext : IGetRequest,IToPduStringInHex
    {
        [XmlIgnore] public GetRequestType GetRequestType { get; } = GetRequestType.Next;
        public AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; }=new AxdrIntegerUnsigned8("C1");
        public AxdrIntegerUnsigned32 BlockNumber { get; set; }

        public GetRequestNext()
        {
            
        }

//        public byte[] ToPduBytes()
//        {
//            List<byte> pduBytes = new List<byte>();
//            pduBytes.Add((byte) GetRequestType);
//            pduBytes.AddRange(InvokeIdAndPriority.ToPduStringInHex().StringToByte());
//            pduBytes.AddRange(BlockNumber.ToPduStringInHex().StringToByte());
//            return pduBytes.ToArray();
//        }

        public string ToPduStringInHex()
        {
            return "02"+ InvokeIdAndPriority.ToPduStringInHex() + BlockNumber.ToPduStringInHex();
        }
    }
}