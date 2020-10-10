using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;
using 三相智慧能源网关调试软件.DLMS.Axdr;

namespace 三相智慧能源网关调试软件.DLMS.Wrapper
{
    public class NetFrame : IToPduStringInHex, IPduStringInHexConstructor
    {
        public AxdrUnsigned16 Version { get; set; }

        public AxdrUnsigned16 SourceAddress { get; set; }
        public AxdrUnsigned16 DestAddress { get; set; }
        AxdrUnsigned16 Length { get; set; }
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
            Version = new AxdrUnsigned16();
            if (!Version.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            SourceAddress = new AxdrUnsigned16();
            if (!SourceAddress.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            DestAddress = new AxdrUnsigned16();
            if (!DestAddress.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            Length = new AxdrUnsigned16();
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