using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Common;
using System;

namespace MyDlmsStandard.ApplicationLay.CosemObjects
{
    public class CosemDisconnectControl : CosemObject, IDlmsBase
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

        public int AttributeCount => throw new NotImplementedException();

        public int MethodCount => throw new NotImplementedException();


        public DataType GetDataType(int index)
        {
            throw new NotImplementedException();
        }


    }
}