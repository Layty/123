using System.Xml.Serialization;
using MyDlmsNetCore.ApplicationLay.ApplicationLayEnums;
using MyDlmsNetCore.Axdr;

namespace MyDlmsNetCore.ApplicationLay.Get
{
    public class GetResponseWithDataBlock : IToPduStringInHex, IGetResponse
    {
        [XmlIgnore] public GetResponseType GetResponseType { get; set; } = GetResponseType.WithDataBlock;
        public AxdrIntegerUnsigned8 InvokeIdAndPriority { get; set; }
        public DataBlockG DataBlockG { get; set; }


        public string ToPduStringInHex()
        {
            return "02" + InvokeIdAndPriority.ToPduStringInHex() + DataBlockG.ToPduStringInHex();
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            if (string.IsNullOrEmpty(pduStringInHex))
            {
                return false;
            }


            InvokeIdAndPriority = new AxdrIntegerUnsigned8();
            if (!InvokeIdAndPriority.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            DataBlockG = new DataBlockG();
            if (!DataBlockG.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            return true;
        }
    }
}