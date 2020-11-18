using MyDlmsStandard.Axdr;
using MyDlmsStandard.Common;

namespace MyDlmsStandard.Wrapper
{
    public class NetFrame : IToPduStringInHex, IPduStringInHexConstructor
    {
        public AxdrIntegerUnsigned16 Version { get; set; }

        public AxdrIntegerUnsigned16 SourceAddress { get; set; }
        public AxdrIntegerUnsigned16 DestAddress { get; set; }
        AxdrIntegerUnsigned16 Length { get; set; }
        public byte[] DLMSApduDataBytes { get; set; }


        public NetFrame()
        {
        }


        public void OverturnDestinationSource()
        {
            var tm = DestAddress;
            DestAddress = SourceAddress;
            SourceAddress = tm;
        }

        public string ToPduStringInHex()
        {
            return Version.ToPduStringInHex() + SourceAddress.ToPduStringInHex() + DestAddress.ToPduStringInHex() +
                   DLMSApduDataBytes.Length.ToString("X4") + DLMSApduDataBytes.ByteToString();
        }

        public byte[] ToPduBytes()
        {
            return ToPduStringInHex().StringToByte();
        }


        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            Version = new AxdrIntegerUnsigned16();
            if (!Version.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            SourceAddress = new AxdrIntegerUnsigned16();
            if (!SourceAddress.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            DestAddress = new AxdrIntegerUnsigned16();
            if (!DestAddress.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            Length = new AxdrIntegerUnsigned16();
            if (!Length.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            if (Length.GetEntityValue() != pduStringInHex.StringToByte().Length)
            {
                return false;
            }
            DLMSApduDataBytes = pduStringInHex.StringToByte();

            return true;
        }
    }
}