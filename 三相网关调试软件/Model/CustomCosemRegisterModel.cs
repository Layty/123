using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects.DataStorage;

namespace 三相智慧能源网关调试软件.Model
{
    public class CustomCosemRegisterModel : CosemRegister
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

        public CustomCosemRegisterModel(string logicName) : base(logicName)
        {
        }
    }
}