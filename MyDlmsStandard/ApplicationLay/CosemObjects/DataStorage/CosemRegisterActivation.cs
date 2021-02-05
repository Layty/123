using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Common;

namespace MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage
{
    public class CosemRegisterActivation : CosemObject
    {

        public DLMSArray RegisterAssignment
        {
            get => _RegisterAssignment;
            set { _RegisterAssignment = value; OnPropertyChanged(); }
        }
        private DLMSArray _RegisterAssignment;

        public DLMSArray MaskList
        {
            get => _MaskList;
            set { _MaskList = value; OnPropertyChanged(); }
        }
        private DLMSArray _MaskList;




        public CosemRegisterActivation(string logicName)
        {
            LogicalName = logicName;
            ClassId = MyConvert.GetClassIdByObjectType(ObjectType.RegisterActivation);
        }
    }
}