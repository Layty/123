using DLMSMaster.ApplicationLay.Enums;

namespace 三相智慧能源网关调试软件.DLMS.CosemObjects
{
    public interface IDLMSBase
    {
        string[] GetNames();
        int GetAttributeCount();
        int GetMethodCount();
        DataType GetDataType(int index);
    }
}