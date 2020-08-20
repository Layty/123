using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using 三相智慧能源网关调试软件.Annotations;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects
{
    public class DLMSDisconnectControl: DLMSObject, IDLMSBase, INotifyPropertyChanged
 {

        public bool OutputState
        {
            get => _OutputState;
            set { _OutputState = value;OnPropertyChanged();  }
        }
        private bool _OutputState;

        
        public DLMSDisconnectControl()
        {
            LogicalName = "0.0.96.3.10.255";
            ObjectType = ObjectType.DisconnectControl;
        }

        public DLMSDisconnectControl(string logicalName, ObjectType objectType)
        {
            LogicalName = logicalName;
            ObjectType = objectType;
        }
        public string[] GetNames()
     {
         throw new NotImplementedException();
     }

     public int GetAttributeCount()
     {
         throw new NotImplementedException();
     }

     public int GetMethodCount()
     {
         throw new NotImplementedException();
     }

     public DataType GetDataType(int index)
     {
         throw new NotImplementedException();
     }

     public event PropertyChangedEventHandler PropertyChanged;

     [NotifyPropertyChangedInvocator]
     protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
     {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
     }
    }

    public class DLMSData : DLMSObject, IDLMSBase, INotifyPropertyChanged
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


        public DLMSData(string logicalName)
        {
            LogicalName = logicalName;
            ObjectType = ObjectType.Data;
        }
        public DLMSData(string logicalName,ObjectType objectType)
        {
            LogicalName = logicalName;
            ObjectType = objectType;
        }
        public AttributeDescriptor GetLogicNameAttributeDescriptor() => GetCosemAttributeDescriptor(1);
        public AttributeDescriptor GetValueAttributeDescriptor() => GetCosemAttributeDescriptor(2);
       
        

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