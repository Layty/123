using System.Xml.Serialization;
using MyDlmsNetCore.ApplicationLay.ApplicationLayEnums;
using MyDlmsNetCore.Axdr;

namespace MyDlmsNetCore.ApplicationLay.Set
{
    public class SetRequestWithDataBlock : ISetRequest
    {
        [XmlIgnore] public SetRequestType SetRequestType { get; } = SetRequestType.WithDataBlock;
        public AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; }
        public DataBlockSA DataBlockSA { get; set; }


        public string ToPduStringInHex()
        {
            return "03" + InvokeIdAndPriority.ToPduStringInHex() + DataBlockSA.ToPduStringInHex();
        }
    }
}