namespace ClassLibraryDLMS.DLMS.ApplicationLay
{
    public class AccessParameters:IToPduStringInHex,IPduStringInHexConstructor
    {
        public DLMSDataItem Data { get; set; }
   

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