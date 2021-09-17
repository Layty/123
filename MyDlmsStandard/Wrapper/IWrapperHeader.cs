using MyDlmsStandard.Axdr;

namespace MyDlmsStandard.Wrapper
{
    public interface IWrapperHeader:IToPduStringInHex, IPduStringInHexConstructor
    {
        /// <summary>
        /// 版本号
        /// </summary>
        AxdrIntegerUnsigned16 Version { get; set; }
        /// <summary>
        /// 源地址
        /// </summary>
        AxdrIntegerUnsigned16 SourceAddress { get; set; }
        /// <summary>
        /// 目的地址
        /// </summary>
        AxdrIntegerUnsigned16 DestAddress { get; set; }
        /// <summary>
        /// WrapperData帧的字节长度，与WrapperData相关
        /// </summary>
        AxdrIntegerUnsigned16 Length { get; set; }
    }
}