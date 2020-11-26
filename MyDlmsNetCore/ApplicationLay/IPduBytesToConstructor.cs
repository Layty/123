namespace MyDlmsNetCore.ApplicationLay
{
    public interface IPduBytesToConstructor
    {
        bool PduBytesToConstructor(byte[] pduBytes);
    }
}