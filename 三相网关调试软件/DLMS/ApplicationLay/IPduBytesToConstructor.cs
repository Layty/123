namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay
{
    public interface IPduBytesToConstructor
    {
        bool PduBytesToConstructor(byte[] pduBytes);
    }

    public interface IToPduStringInHex
    {
        string ToPduStringInHex();
    }
    public interface IPduStringInHexConstructor
    {
        bool PduStringInHexConstructor(ref string pduStringInHex);
    }

}