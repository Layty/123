using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;

namespace MyDlmsStandard.ApplicationLay.CosemObjects
{
    public interface IDlmsBase
    {
        string[] GetNames();
        int AttributeCount { get; }

        int MethodCount { get; }

        DataType GetDataType(int index);
    }
}