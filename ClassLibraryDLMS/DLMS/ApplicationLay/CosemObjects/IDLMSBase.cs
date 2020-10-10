using ClassLibraryDLMS.DLMS.ApplicationLay.ApplicationLayEnums;

namespace ClassLibraryDLMS.DLMS.ApplicationLay.CosemObjects
{
    public interface IDlmsBase
    {
        string[] GetNames();
        int GetAttributeCount();
        int GetMethodCount();
        DataType GetDataType(int index);
    }
}