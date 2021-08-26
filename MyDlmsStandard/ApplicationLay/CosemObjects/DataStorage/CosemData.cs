using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Common;

namespace MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage
{
    /// <summary>
    /// Cosem 的类1 Data类
    /// </summary>
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
        /// <summary>
        /// 进行一层简单的封装，返回LogicName对应的描述,即属性1
        /// </summary>
        /// <returns></returns>
        public CosemAttributeDescriptor LogicNameAttributeDescriptor => GetCosemAttributeDescriptor(1);
        /// <summary>
        /// 进行一层简单的封装，返回Value对应的描述,即属性2
        /// </summary>
        /// <returns></returns>
        public CosemAttributeDescriptor ValueAttributeDescriptor => GetCosemAttributeDescriptor(2);

        public virtual string[] GetNames() => new[] {LogicalName, "Value"};

        public virtual int AttributeCount => 2;
        public virtual int MethodCount => 0;

        /// <summary>
        /// 返回相应属性的数据类型,只有Value的是未确定的
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
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