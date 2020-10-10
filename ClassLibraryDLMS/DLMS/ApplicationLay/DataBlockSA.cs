using System.Text;
using System.Xml.Serialization;
using ClassLibraryDLMS.DLMS.Axdr;

namespace ClassLibraryDLMS.DLMS.ApplicationLay
{
    public class DataBlockSA
    {
        public AxdrBoolean LastBlock { get; set; }
        public AxdrUnsigned32 BlockNumber { get; set; }
        public AxdrOctetString RawData { get; set; }

        [XmlIgnore] public int Length => CalculateLength();

        private int CalculateLength()
        {
            int num = 0;
            if (LastBlock != null)
            {
                num += LastBlock.Length;
            }

            if (BlockNumber != null)
            {
                num += BlockNumber.Length;
            }

            if (RawData != null)
            {
                num += RawData.Length;
            }

            return num;
        }

        public string ToPduStringInHex()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(LastBlock.ToPduStringInHex());
            stringBuilder.Append(BlockNumber.ToPduStringInHex());
            stringBuilder.Append(RawData.ToPduStringInHex());
            return stringBuilder.ToString();
        }

        public bool PduStringInHexContructor(ref string pduStringInHex)
        {
            if (string.IsNullOrEmpty(pduStringInHex))
            {
                return false;
            }

            LastBlock = new AxdrBoolean();
            if (!LastBlock.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            BlockNumber = new AxdrUnsigned32();
            if (!BlockNumber.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            RawData = new AxdrOctetString();
            if (!RawData.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            return true;
        }
    }
}