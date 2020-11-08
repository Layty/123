namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay
{
    public class AccessParameters:IToPduStringInHex,IPduStringInHexConstructor
    {
        public DlmsDataItem Data { get; set; }
   

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            Data = new DlmsDataItem();
            return Data.PduStringInHexConstructor(ref pduStringInHex);
        }

        public string ToPduStringInHex()
        {
            return Data.ToPduStringInHex();
        }
    }
}