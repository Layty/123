namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay
{
    public class AccessParameters:IToPduBytes,IToPduStringInHex,IPduStringInHexConstructor
    {
        public DLMSDataItem Data { get; set; }
        public byte[] ToPduBytes()
        {
            return Data.ToPduBytes();
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            Data = new DLMSDataItem();
            return Data.PduStringInHexConstructor(ref pduStringInHex);
        }

        public string ToPduStringInHex()
        {
            return Data.ToPduStringInHex();
        }
    }
}