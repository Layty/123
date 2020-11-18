using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;

namespace MyDlmsStandard.ApplicationLay.CosemObjects
{
    public interface IDlmsBase
    {
        string[] GetNames();
        int GetAttributeCount();
        int GetMethodCount();
        DataType GetDataType(int index);
    }
}