using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.Axdr;
using 三相智慧能源网关调试软件.DLMS.Common;


namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects
{
    public class CosemDisconnectControl : CosemObject, IDLMSBase, INotifyPropertyChanged
    {
        public bool OutputState
        {
            get => _outputState;
            set
            {
                _outputState = value;
                OnPropertyChanged();
            }
        }

        private bool _outputState;


        public CosemDisconnectControl()
        {
            LogicalName = "0.0.96.3.10.255";
            ClassId = MyConvert.GetClassIdByObjectType(ObjectType.DisconnectControl);
        }

        public CosemDisconnectControl(string logicalName, ObjectType objectType)
        {
            LogicalName = logicalName;
            ClassId = MyConvert.GetClassIdByObjectType(objectType);
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

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}