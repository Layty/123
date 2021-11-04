namespace MyDlmsStandard.ApplicationLay.ApplicationLayEnums
{
    public enum DataType : byte
    {
        NullData = 0,
        /// <summary>
        /// 数组类型元素在 COSEMIC 规范的属性或方法 描述部分中定义。
        /// </summary>
        Array = 1,
        /// <summary>
        /// 结构类型元素在 COSEMIC 规范的属性或方法 描述部分中定义。
        /// </summary>
        Structure = 2,
        /// <summary>
        /// 布尔数
        /// </summary>
        Boolean = 3,
        /// <summary>
        /// 布尔值的有序序列
        /// </summary>
        BitString = 4,
        /// <summary>
        /// 双长整:32位整数(Integer32)
        /// </summary>
        Int32 = 5,
        /// <summary>
        /// 32位无符号数(Unsigned32)
        /// </summary>
        UInt32 = 6,

        double32 = 7,//DL/T790.441—2004 中 “浮 点”类 型 的 标 签,在 DLMS/COSEM 中不可用。见标签[23]和[24]
        double64 = 8,
        /// <summary>
        /// 有序的八位元序列(8位的字节串)
        /// </summary>
        OctetString = 9,
        /// <summary>
        /// 有序的 ASCII序列
        /// </summary>
        VisibleString = 10,
        /// <summary>
        /// 按 UTF-8编码的字符有序序列
        /// </summary>
        UTF8String = 12,
        /// <summary>
        /// 二进制编码的十进制数
        /// </summary>
        Bcd = 13,
        /// <summary>
        /// 整型:8位整数(Integer8)
        /// </summary>
        Int8 = 15,
        /// <summary>
        /// 长整型:16位整数(Integer16)
        /// </summary>
        Int16 = 16,
        /// <summary>
        /// 8位无符号数(Unsigned8)
        /// </summary>
        UInt8 = 17,
        /// <summary>
        /// 16位无符号数(Unsigned16)
        /// </summary>
        UInt16 = 18,
        /// <summary>
        /// 提供了一种可替代的复杂数据的压缩编码
        /// </summary>
        CompactArray = 19,
        /// <summary>
        /// 64位整数(Integer64)
        /// </summary>
        Int64 = 20,
        /// <summary>
        /// 64位无符号数(Unsigned64)
        /// </summary>
        UInt64 = 21,
        /// <summary>
        /// 枚举类型元素在 COSEMIC 规范的属性或方法 描述部分中定义
        /// </summary>
        Enum = 22,
        /// <summary>
        /// 字节串(大小(4))OCTETSTRING (SIZE(4))
        /// </summary>
        Float32 = 23,
        /// <summary>
        /// 字节串(大小(8))OCTETSTRING (SIZE(8))
        /// </summary>
        Float64 = 24,
        /// <summary>
        /// 字节串(大小(12))OCTETSTRING (SIZE(12))
        /// </summary>
        DateTime = 25,
        /// <summary>
        /// 字节串(大小(5))OCTETSTRING (SIZE(5))
        /// </summary>
        Date = 26,
        /// <summary>
        /// 字节串(大小(4))OCTETSTRING (SIZE(4))
        /// </summary>
        Time = 27,
        DoNotCare = 255










    }
}