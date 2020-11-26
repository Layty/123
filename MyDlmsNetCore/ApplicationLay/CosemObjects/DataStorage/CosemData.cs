using MyDlmsNetCore.ApplicationLay.ApplicationLayEnums;
using MyDlmsNetCore.Common;

namespace MyDlmsNetCore.ApplicationLay.CosemObjects.DataStorage
{
    public class CosemData : CosemObject, IDlmsBase
    {
        public DlmsDataItem Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        private DlmsDataItem _value;

        public CosemData(string logicalName) : this(logicalName, ObjectType.Data)
        {
            LogicalName = logicalName;
        }

        public CosemData(string logicalName, ObjectType objectType)
        {
            LogicalName = logicalName;
            ClassId = MyConvert.GetClassIdByObjectType(objectType);
        }

        public CosemAttributeDescriptor GetLogicNameAttributeDescriptor() => GetCosemAttributeDescriptor(1);
        public CosemAttributeDescriptor GetValueAttributeDescriptor() => GetCosemAttributeDescriptor(2);

        public virtual string[] GetNames() => new[] {LogicalName, "Value"};

        public virtual int GetAttributeCount() => 2;
        public virtual int GetMethodCount() => 0;

        public virtual DataType GetDataType(int index)
        {
            DataType dataType = new DataType();
            switch (index)
            {
                case 1:
                    dataType = DataType.OctetString;
                    break;
                case 2:
                    dataType = Value.DataType;
                    break;
                default: break;
            }

            return dataType;
        }
    }
}