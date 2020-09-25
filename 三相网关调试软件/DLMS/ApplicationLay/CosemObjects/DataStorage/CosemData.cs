using System.ComponentModel;
using System.Runtime.CompilerServices;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.Common;


namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects.DataStorage
{
    public class CosemData : CosemObject, IDLMSBase, INotifyPropertyChanged
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


        public CosemData(string logicalName)
        {
            LogicalName = logicalName;

            ClassId = MyConvert.GetClassIdByObjectType(ObjectType.Data);
        }
        public CosemData(string logicalName,ObjectType objectType)
        {
            LogicalName = logicalName;
            ClassId = MyConvert.GetClassIdByObjectType(objectType);
        }
        public CosemAttributeDescriptor GetLogicNameAttributeDescriptor() => GetCosemAttributeDescriptor(1);
        public CosemAttributeDescriptor GetValueAttributeDescriptor() => GetCosemAttributeDescriptor(2);
       
        

        string[] IDLMSBase.GetNames() => new[] {LogicalName, "Value"};

        int IDLMSBase.GetAttributeCount() => 2;
        int IDLMSBase.GetMethodCount() => 0;

        public DataType GetDataType(int index)
        {
            return new DataType();
            //switch (index)
            //{
            //    case 1:
            //        return DataType.OctetString;
            //    case 2:
            //        {
            //            DataType dataType = base.GetDataType(index);
            //            if (dataType == DataType.None && Value != null)
            //            {
            //                dataType = GXCommon.GetDLMSDataType(Value.GetType());
            //            }
            //            return dataType;
            //        }
            //    default: break;

            //}
        }

        public event PropertyChangedEventHandler PropertyChanged;

      
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}