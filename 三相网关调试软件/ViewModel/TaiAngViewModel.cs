using System.Collections.ObjectModel;
using System.Text;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MySerialPortMaster;
using 三相智慧能源网关调试软件.DLMS;
using 三相智慧能源网关调试软件.Model;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class TaiAngViewModel : ViewModelBase
    {
        public DLMSClient Client { get; set; }

        private ObservableCollection<DLMSTaiAngModel> _taiAngCollection;

        public ObservableCollection<DLMSTaiAngModel> TaiAngCollection
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
                TaiAngCollection = new ObservableCollection<DLMSTaiAngModel>
                {
                    new DLMSTaiAngModel {LogicalName = "1.1.98.0.128.255"},
                    new DLMSTaiAngModel {LogicalName = "1.2.98.0.128.255"},
                    new DLMSTaiAngModel {LogicalName = "1.3.98.0.128.255"},
                    new DLMSTaiAngModel {LogicalName = "1.4.98.0.128.255"}
                };
            }
            else
            {
                Client = CommonServiceLocator.ServiceLocator.Current.GetInstance<DLMSClient>();
                TaiAngCollection = new ObservableCollection<DLMSTaiAngModel>
                {
                    new DLMSTaiAngModel {LogicalName = "1.1.98.0.128.255"},
                    new DLMSTaiAngModel {LogicalName = "1.2.98.0.128.255"},
                    new DLMSTaiAngModel {LogicalName = "1.3.98.0.128.255"},
                    new DLMSTaiAngModel {LogicalName = "1.4.98.0.128.255"},
                    new DLMSTaiAngModel {LogicalName = "1.5.98.0.128.255"},
                    new DLMSTaiAngModel {LogicalName = "1.6.98.0.128.255"},
                    new DLMSTaiAngModel {LogicalName = "1.7.98.0.128.255"},
                    new DLMSTaiAngModel {LogicalName = "1.8.98.0.128.255"},
                    new DLMSTaiAngModel {LogicalName = "1.9.98.0.128.255"},
                    new DLMSTaiAngModel {LogicalName = "1.10.98.0.128.255"},
                    new DLMSTaiAngModel {LogicalName = "1.11.98.0.128.255"},
                    new DLMSTaiAngModel {LogicalName = "1.12.98.0.128.255"},
                    new DLMSTaiAngModel {LogicalName = "1.13.98.0.128.255"},
                    new DLMSTaiAngModel {LogicalName = "1.14.98.0.128.255"},
                    new DLMSTaiAngModel {LogicalName = "1.15.98.0.128.255"},
                    new DLMSTaiAngModel {LogicalName = "1.16.98.0.128.255"}
                };
                GetLogicNameDataCommand = new RelayCommand<DLMSTaiAngModel>(
                    async t =>
                    {
                        t.DataForShow = "";
                        var dataResult = await Client.GetRequest(t.GetLogicName());
                        t.DataForShow = NormalDataParse.ParsePduData(dataResult);
                    });
                GetMeterAddressData = new RelayCommand<DLMSTaiAngModel>(async t =>
                    {
                        t.DataForShow = "";
                        var dataResult = await Client.GetRequest(t.GetTableId());
                        t.DataForShow = NormalDataParse.ParsePduData(dataResult);
                    }
                );
                GetDataLengthData = new RelayCommand<DLMSTaiAngModel>(async t =>
                    {
                        t.DataForShow = "";
                        var dataResult = await Client.GetRequest(t.GetLength());
                        t.DataForShow = NormalDataParse.ParsePduData(dataResult);
                    }
                );
                GetBuffData = new RelayCommand<DLMSTaiAngModel>(async t =>
                    {
                        t.DataForShow = "";
                        var dataResult = await Client.GetRequest(t.GetBuffer());
                        t.DataForShow =
                            Encoding.Default.GetString(NormalDataParse.ParsePduData(dataResult).StringToByte());
                    }
                );
            }
        }

        private RelayCommand<DLMSTaiAngModel> _getLogicNameDataCommand;

        public RelayCommand<DLMSTaiAngModel> GetLogicNameDataCommand
        {
            get => _getLogicNameDataCommand;
            set
            {
                _getLogicNameDataCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand<DLMSTaiAngModel> _etMeterAddressData;

        public RelayCommand<DLMSTaiAngModel> GetMeterAddressData
        {
            get => _etMeterAddressData;
            set
            {
                _etMeterAddressData = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand<DLMSTaiAngModel> _getDataLengthData;

        public RelayCommand<DLMSTaiAngModel> GetDataLengthData
        {
            get => _getDataLengthData;
            set
            {
                _getDataLengthData = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand<DLMSTaiAngModel> _getBuffData;

        public RelayCommand<DLMSTaiAngModel> GetBuffData
        {
            get => _getBuffData;
            set
            {
                _getBuffData = value;
                RaisePropertyChanged();
            }
        }
    }
}