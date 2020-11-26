namespace MyDlmsNetCore.ApplicationLay
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