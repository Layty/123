using System.ComponentModel;
using System.Runtime.CompilerServices;
using 三相智慧能源网关调试软件.DLMS.CosemObjects;

namespace 三相智慧能源网关调试软件.Model
{
    public class DLMSSelfDefineTaiAngModel : DLMSUtilityTables, INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string NameDescription { get; set; } = "logical_name";
        public string AddrDescription { get; set; } = "抽屉柜modbus_id";
        public string LengthDescription { get; set; } = "buffer-length";
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