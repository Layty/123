namespace MyDlmsStandard.Wrapper
{
    public class WrapperBody : IWrapperBody
    {
        public byte[] DataBytes { get; set; }
        public int Length => DataBytes.Length;
    }
}