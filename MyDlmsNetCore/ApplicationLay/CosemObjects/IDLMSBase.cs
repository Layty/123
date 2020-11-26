using MyDlmsNetCore.ApplicationLay.ApplicationLayEnums;

namespace MyDlmsNetCore.ApplicationLay.CosemObjects
{
    public interface IDlmsBase
    {
        string[] GetNames();
        int GetAttributeCount();
        int GetMethodCount();
        DataType GetDataType(int index);
    }
}