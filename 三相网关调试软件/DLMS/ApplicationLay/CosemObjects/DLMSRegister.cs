using System;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects
{
    public class DLMSRegister :DLMSData,  IDLMSBase
    {
        public double Scalar
        {
            get => _scalar;
            set
            {
                _scalar = value;
                OnPropertyChanged();
            }
        }

        private double _scalar = 1.0;

        public Unit Unit
        {
            get => _unit;
            set
            {
                _unit = value;
                OnPropertyChanged();
            }
        }

        private Unit _unit;


        public DLMSRegister(string logicName) : base(logicName)
        {
            LogicalName = logicName;
            this.ObjectType = ObjectType.Register;
            Version = 0;
        }
        public AttributeDescriptor GetScalar_UnitAttributeDescriptor() => GetCosemAttributeDescriptor(3);
        public byte[] GetScalar_Unit() => GetCosemAttributeDescriptor(3).ToPduBytes();

        public string[] GetNames()
        {
            return new[] {LogicalName, "Value", "Scalar_Unit"};
        }

        public int GetAttributeCount()
        {
            return 3;
        }

        public int GetMethodCount()
        {
            return 1;
        }

        public DataType GetDataType(int index)
        {
            throw new NotImplementedException();
        }

    }
}