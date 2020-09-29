using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects
{
    public interface IDlmsBase
    {
        string[] GetNames();
        int GetAttributeCount();
        int GetMethodCount();
        DataType GetDataType(int index);
    }
}