using System.ComponentModel;
using System.Runtime.CompilerServices;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects;

namespace 三相智慧能源网关调试软件.Model
{
    public class DLMSSelfDefineUtilityTablesModel : DLMSUtilityTables, INotifyPropertyChanged
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}