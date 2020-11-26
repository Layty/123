using MyDlmsNetCore.ApplicationLay.ApplicationLayEnums;
using MyDlmsNetCore.Common;

namespace MyDlmsNetCore.ApplicationLay.CosemObjects.DataStorage
{
    public class CosemRegisterTable : CosemObject
    {
        public CosemRegisterTable(string logicName)
        {
            LogicalName = logicName;
            ClassId = MyConvert.GetClassIdByObjectType(ObjectType.RegisterActivation);
        }
    }
}