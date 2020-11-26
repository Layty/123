using MyDlmsNetCore.ApplicationLay.ApplicationLayEnums;
using MyDlmsNetCore.Common;

namespace MyDlmsNetCore.ApplicationLay.CosemObjects.DataStorage
{
    public class CosemExtendedRegister : CosemRegister
    {
        public DlmsDataItem Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        private DlmsDataItem _status;

        public CosemExtendedRegister(string logicName) : base(logicName)
        {
            LogicalName = logicName;
            ClassId = MyConvert.GetClassIdByObjectType(ObjectType.ExtendedRegister);
        }
    }
}