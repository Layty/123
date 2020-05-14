using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using 三相智慧能源网关调试软件.DLMS;
using 三相智慧能源网关调试软件.DLMS.CosemObjects;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class TaiAngViewModel : ViewModelBase
    {
        //public DLMSTaiAng DlmsTai { get; set; }
        private ObservableCollection<DLMSTaiAng> _taiAngCollection;

        public ObservableCollection<DLMSTaiAng> TaiAngCollection
        {
            get => _taiAngCollection;
            set
            {
                _taiAngCollection = value;
                RaisePropertyChanged();
            }
        }

        public TaiAngViewModel()
        {
            if (IsInDesignMode)
            {
                TaiAngCollection = new ObservableCollection<DLMSTaiAng>
                {
                    new DLMSTaiAng {LogicalName = "1.1.98.0.128.255"},
                    new DLMSTaiAng {LogicalName = "1.2.98.0.128.255"},
                    new DLMSTaiAng {LogicalName = "1.3.98.0.128.255"},
                    new DLMSTaiAng {LogicalName = "1.4.98.0.128.255"}
                };
            }
            else
            {
                TaiAngCollection = new ObservableCollection<DLMSTaiAng>
                {
                    new DLMSTaiAng {LogicalName = "1.1.98.0.128.255"},
                    new DLMSTaiAng {LogicalName = "1.2.98.0.128.255"},
                    new DLMSTaiAng {LogicalName = "1.3.98.0.128.255"},
                    new DLMSTaiAng {LogicalName = "1.4.98.0.128.255"},
                    new DLMSTaiAng {LogicalName = "1.5.98.0.128.255"},
                    new DLMSTaiAng {LogicalName = "1.6.98.0.128.255"},
                    new DLMSTaiAng {LogicalName = "1.7.98.0.128.255"},
                    new DLMSTaiAng {LogicalName = "1.8.98.0.128.255"},
                    new DLMSTaiAng {LogicalName = "1.9.98.0.128.255"},
                    new DLMSTaiAng {LogicalName = "1.10.98.0.128.255"},
                    new DLMSTaiAng {LogicalName = "1.11.98.0.128.255"},
                    new DLMSTaiAng {LogicalName = "1.12.98.0.128.255"},
                    new DLMSTaiAng {LogicalName = "1.13.98.0.128.255"},
                    new DLMSTaiAng {LogicalName = "1.14.98.0.128.255"},
                    new DLMSTaiAng {LogicalName = "1.15.98.0.128.255"},
                    new DLMSTaiAng {LogicalName = "1.16.98.0.128.255"}
                };
                GetLogicNameDataCommand = new RelayCommand<DLMSTaiAng>(async t => await t.GetLogicName());
                GetMeterAddressData = new RelayCommand<DLMSTaiAng>(async t => await t.GetTableId());
                GetDataLengthData = new RelayCommand<DLMSTaiAng>(async t => await t.GetLength());
                GetBuffData = new RelayCommand<DLMSTaiAng>(async t =>
                    {
                        t.DataForShow = "";
                        var DataResult = await t.GetBuffer();
                        var bytes = NormalDataParse.GetUtilityTablesDataContent(DataResult, 3, out bool result);
                        if (result)
                        {
                            t.DataForShow = Encoding.Default.GetString(bytes);
                        }
                    }
                );
            }
        }

        private RelayCommand<DLMSTaiAng> _getLogicNameDataCommand;

        public RelayCommand<DLMSTaiAng> GetLogicNameDataCommand
        {
            get { return _getLogicNameDataCommand; }
            set
            {
                _getLogicNameDataCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand<DLMSTaiAng> _etMeterAddressData;

        public RelayCommand<DLMSTaiAng> GetMeterAddressData
        {
            get { return _etMeterAddressData; }
            set
            {
                _etMeterAddressData = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand<DLMSTaiAng> _getDataLengthData;

        public RelayCommand<DLMSTaiAng> GetDataLengthData
        {
            get { return _getDataLengthData; }
            set
            {
                _getDataLengthData = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand<DLMSTaiAng> _getBuffData;

        public RelayCommand<DLMSTaiAng> GetBuffData
        {
            get { return _getBuffData; }
            set
            {
                _getBuffData = value;
                RaisePropertyChanged();
            }
        }
    }

    public class DLMSTaiAng : DLMSUtilityTables, INotifyPropertyChanged
    {
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