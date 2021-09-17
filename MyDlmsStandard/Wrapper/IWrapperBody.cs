namespace MyDlmsStandard.Wrapper
{
    public interface IWrapperBody
    {
        byte[] DataBytes { get; set; }
        int Length { get; }
    }
}