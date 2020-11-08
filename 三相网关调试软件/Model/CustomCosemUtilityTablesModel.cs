using 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects.DataStorage;

namespace 三相智慧能源网关调试软件.Model
{
    public class CustomCosemUtilityTablesModel : CosemUtilityTables
    {
        public string Name { get; set; }
        public string NameDescription { get; set; } = "LogicalName";
        public string AddrDescription { get; set; } = "ModbusId";
        public string LengthDescription { get; set; } = "BufferLength";
        public string DataDescription { get; set; } = "Buffer";
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