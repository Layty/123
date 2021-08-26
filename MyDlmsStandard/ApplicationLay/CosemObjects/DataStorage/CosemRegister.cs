using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;

namespace MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage
{
    public class CosemRegister : CosemData
    {
        public ScalarUnit ScalarUnit
        {
            get => _scalarUnit;
            set
            {
                _scalarUnit = value;
                OnPropertyChanged();
            }
        }

        private ScalarUnit _scalarUnit;

        public CosemRegister(string logicName) : base(logicName, ObjectType.Register)
        {
            LogicalName = logicName;
        }
        
        public virtual CosemAttributeDescriptor ScalarUnitAttributeDescriptor => GetCosemAttributeDescriptor(3);

        public CosemMethodDescriptor ResetMethodDescriptor => GetCosemMethodDescriptor(1);

        public override string[] GetNames()
        {
            return new[] {LogicalName, "Value", "Scalar_Unit"};
        }

        public override int AttributeCount => 3;

        public override int MethodCount => 1;

        public override DataType GetDataType(int index)
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
                case 3:
                    dataType = DataType.Structure;
                    break;
            }

            return dataType;
        }
    }
}