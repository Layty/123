using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects.DataStorage;
using 三相智慧能源网关调试软件.DLMS.Axdr;

namespace 三相智慧能源网关调试软件.Model
{
    public class DlmsSelfDefineCosemProfileGeneric : CosemProfileGeneric
    {
        public DlmsSelfDefineCosemProfileGeneric(string logicalName) : base(logicalName)
        {
        }
    }

    public class CosemSelfDefineRegisterModel : CosemRegister
    {
        public ErrorCode LastResult
        {
            get => _lastResult;
            set
            {
                _lastResult = value;
                OnPropertyChanged();
            }
        }

        private ErrorCode _lastResult;
        public string RegisterName { get; set; }

        public CosemSelfDefineRegisterModel(string logicName) : base(logicName)
        {
        }
    }

    public class CosemSelfDefineData : CosemData
    {
     
        public string DataName { get; set; }
 
        public AxdrInteger8 Attr { get; set; }
        public ErrorCode LastResult
        {
            get => _lastResult;
            set
            {
                _lastResult = value;
                OnPropertyChanged();
            }
        }

        private ErrorCode _lastResult;

        public CosemSelfDefineData(string logicalName) : base(logicalName)
        {
        }
        public CosemSelfDefineData(string logicalName ,ObjectType objectType) : base(logicalName,objectType)
        {
            
        }
        public CosemSelfDefineData(string logicalName, ObjectType objectType,AxdrInteger8 attr) : base(logicalName, objectType)
        {
            Attr = attr;
        }
    }
}