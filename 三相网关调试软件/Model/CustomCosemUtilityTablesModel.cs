using MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage;

namespace 三相智慧能源网关调试软件.Model
{
    public class CustomCosemUtilityTablesModel : CosemUtilityTables
    {
        public string Name { get; set; }
        private string _dataForShow;

        public string DataForShow
        {
            get => _dataForShow;
            set
            {
                _dataForShow = value;
                OnPropertyChanged();
            }
        }
    }
}