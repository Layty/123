using MyDlmsNetCore.ApplicationLay.ApplicationLayEnums;
using MyDlmsNetCore.Common;

namespace MyDlmsNetCore.ApplicationLay.CosemObjects.DataStorage
{
    public class CosemDemandRegister : CosemExtendedRegister
    {
        public CosemDemandRegister(string logicName) : base(logicName)
        {
            LogicalName = logicName;
            ClassId = MyConvert.GetClassIdByObjectType(ObjectType.DemandRegister);
        }
    }
}