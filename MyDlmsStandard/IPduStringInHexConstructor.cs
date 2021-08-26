namespace MyDlmsStandard
{
    /// <summary>
    /// 将pdu的HexString 报文装换为该对象的，返回成功与否，成功则对内部对象进行赋值，不成功则返回false不赋值
    /// </summary>
    public interface IPduStringInHexConstructor
    {
        bool PduStringInHexConstructor(ref string pduStringInHex);
    }
}