namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay
{
    public interface IPduBytesToConstructor
    {
        bool PduBytesToConstructor(byte[] pduBytes);
    }
    public interface IPduRefBytesToConstructor
    {
        bool PduBytesToConstructor(ref byte[] pduBytes);
    }
}