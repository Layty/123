namespace ClassLibraryDLMS.DLMS.ApplicationLay.Set
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