using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.Common;


namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects.DataStorage
{
    public class CosemData : CosemObject, IDlmsBase
    {
        public DLMSDataItem Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        private DLMSDataItem _value;


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

       
       
        public string[] GetNames() => new[] {LogicalName, "Value"};

        public int GetAttributeCount() => 2;
        public int GetMethodCount() => 0;

        public DataType GetDataType(int index)
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