using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage;
using MyDlmsStandard.Axdr;

namespace 三相智慧能源网关调试软件.Model
{
    public class CustomCosemDataModel : CosemData
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

        public CustomCosemDataModel(string logicalName) : base(logicalName)
        {
        }
        public CustomCosemDataModel(string logicalName, ObjectType objectType) : base(logicalName, objectType)
        {

        }
        public CustomCosemDataModel(string logicalName, ObjectType objectType, AxdrInteger8 attr) : base(logicalName, objectType)
        {
            Attr = attr;
        }
    }
}