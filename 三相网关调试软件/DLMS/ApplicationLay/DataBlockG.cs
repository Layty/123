namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay
{
    public class DataBlockG
    {
        public bool IsLastBlock { get; set; }
        public uint BlockNumber { get; set; }

        public string RawData { get; set; }
        public byte DataAccessResult { get; set; }
    }
}