using System.Text;
using System.Xml.Serialization;
using MyDlmsStandard.Axdr;

namespace MyDlmsStandard.ApplicationLay
{
    public class DataBlockSA
    {
        public AxdrIntegerBoolean LastBlock { get; set; }
        public AxdrIntegerUnsigned32 BlockNumber { get; set; }
        public AxdrIntegerOctetString RawData { get; set; }

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

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            if (string.IsNullOrEmpty(pduStringInHex))
            {
                return false;
            }

            LastBlock = new AxdrIntegerBoolean();
            if (!LastBlock.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            BlockNumber = new AxdrIntegerUnsigned32();
            if (!BlockNumber.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            RawData = new AxdrIntegerOctetString();
            if (!RawData.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            return true;
        }
    }
}