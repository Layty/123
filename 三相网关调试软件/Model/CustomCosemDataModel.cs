using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects.DataStorage;
using 三相智慧能源网关调试软件.DLMS.Axdr;

namespace 三相智慧能源网关调试软件.Model
{
    public class CustomCosemDataModel : CosemData
    {
     
        public string DataName { get; set; }
 
        public AxdrIntegerInteger8 Attr { get; set; }
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

        public CustomCosemDataModel(string logicalName) : base(logicalName)
        {
        }
        public CustomCosemDataModel(string logicalName ,ObjectType objectType) : base(logicalName,objectType)
        {
            
        }
        public CustomCosemDataModel(string logicalName, ObjectType objectType,AxdrIntegerInteger8 attr) : base(logicalName, objectType)
        {
            Attr = attr;
        }
    }
}