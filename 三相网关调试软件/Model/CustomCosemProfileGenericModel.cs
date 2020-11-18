using MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage;

namespace 三相智慧能源网关调试软件.Model
{
    public class CustomCosemProfileGenericModel : CosemProfileGeneric
    {
        public string ProfileGenericName { get; set; }
        public CustomCosemProfileGenericModel(string logicalName) : base(logicalName)
        {
        }
    }
}