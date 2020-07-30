using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using 三相智慧能源网关调试软件.Annotations;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects
{
    public class DLMSData : DLMSObject, IDLMSBase, INotifyPropertyChanged
    {

      

        public DLMSDataItem Value
        {
            get => _value;
            set { _value = value; OnPropertyChanged(); }
        }
        private DLMSDataItem _value;


        public DLMSData(string logicalName)
        {
            LogicalName = logicalName;
            ObjectType = ObjectType.Data;
        }

        public byte[] GetLogicName() => GetAttributeData(1);
        public byte[] GetValue() => GetAttributeData(2);
        public byte[] SetValue(DLMSDataItem ddi) => SetAttributeData(2, ddi);




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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}