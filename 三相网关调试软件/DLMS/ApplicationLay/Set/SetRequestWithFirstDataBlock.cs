namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.Set
{
   
    public class SetRequestWithFirstDataBlock:IToPduBytes
    {
        public InvokeIdAndPriority InvokeIdAndPriority { get; set; }
        public byte[] ToPduBytes()
        {
            throw new System.NotImplementedException();
        }

    }
}