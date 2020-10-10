using ClassLibraryDLMS.DLMS.Axdr;

namespace ClassLibraryDLMS.DLMS.Wrapper
{
    public class NetFrame : IToPduStringInHex
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
    }
}