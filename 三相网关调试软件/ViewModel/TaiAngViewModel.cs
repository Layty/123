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

        private ObservableCollection<DLMSSelfDefineTaiAngModel> _taiAngCollection;

        public ObservableCollection<DLMSSelfDefineTaiAngModel> TaiAngCollection
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
                TaiAngCollection = new ObservableCollection<DLMSSelfDefineTaiAngModel>
                {
                    new DLMSSelfDefineTaiAngModel {LogicalName = "1.1.98.0.128.255"},
                    new DLMSSelfDefineTaiAngModel {LogicalName = "1.2.98.0.128.255"},
                    new DLMSSelfDefineTaiAngModel {LogicalName = "1.3.98.0.128.255"},
                    new DLMSSelfDefineTaiAngModel {LogicalName = "1.4.98.0.128.255"}
                };
            }
            else
            {
                Client = CommonServiceLocator.ServiceLocator.Current.GetInstance<DLMSClient>();
                TaiAngCollection = new ObservableCollection<DLMSSelfDefineTaiAngModel>
                {
                    new DLMSSelfDefineTaiAngModel {LogicalName = "1.1.98.0.128.255", Name = "泰昂COSEM1"},
                    new DLMSSelfDefineTaiAngModel {LogicalName = "1.2.98.0.128.255", Name = "泰昂COSEM2"},
                    new DLMSSelfDefineTaiAngModel {LogicalName = "1.3.98.0.128.255", Name = "泰昂COSEM3"},
                    new DLMSSelfDefineTaiAngModel {LogicalName = "1.4.98.0.128.255", Name = "泰昂COSEM4"},
                    new DLMSSelfDefineTaiAngModel {LogicalName = "1.5.98.0.128.255", Name = "泰昂COSEM5"},
                    new DLMSSelfDefineTaiAngModel {LogicalName = "1.6.98.0.128.255", Name = "泰昂COSEM6"},
                    new DLMSSelfDefineTaiAngModel {LogicalName = "1.7.98.0.128.255", Name = "泰昂COSEM7"},
                    new DLMSSelfDefineTaiAngModel {LogicalName = "1.8.98.0.128.255", Name = "泰昂COSEM8"},
                    new DLMSSelfDefineTaiAngModel {LogicalName = "1.9.98.0.128.255", Name = "泰昂COSEM9"},
                    new DLMSSelfDefineTaiAngModel {LogicalName = "1.10.98.0.128.255", Name = "泰昂COSEM10"},
                    new DLMSSelfDefineTaiAngModel {LogicalName = "1.11.98.0.128.255", Name = "泰昂COSEM11"},
                    new DLMSSelfDefineTaiAngModel {LogicalName = "1.12.98.0.128.255", Name = "泰昂COSEM12"},
                    new DLMSSelfDefineTaiAngModel {LogicalName = "1.13.98.0.128.255", Name = "泰昂COSEM13"},
                    new DLMSSelfDefineTaiAngModel {LogicalName = "1.14.98.0.128.255", Name = "泰昂COSEM14"},
                    new DLMSSelfDefineTaiAngModel {LogicalName = "1.15.98.0.128.255", Name = "泰昂COSEM15"},
                    new DLMSSelfDefineTaiAngModel {LogicalName = "1.16.98.0.128.255", Name = "泰昂COSEM16"}
                };
                GetLogicNameDataCommand = new RelayCommand<DLMSSelfDefineTaiAngModel>(
                    async t =>
                    {
                        t.DataForShow = "";
                        var dataResult = await Client.GetRequest(t.GetLogicName());
                        t.DataForShow = NormalDataParse.ParsePduData(dataResult);
                    });
                GetMeterAddressData = new RelayCommand<DLMSSelfDefineTaiAngModel>(async t =>
                    {
                        t.DataForShow = "";
                        var dataResult = await Client.GetRequest(t.GetTableId());
                        t.DataForShow = NormalDataParse.ParsePduData(dataResult);
                    }
                );
                GetDataLengthData = new RelayCommand<DLMSSelfDefineTaiAngModel>(async t =>
                    {
                        t.DataForShow = "";
                        var dataResult = await Client.GetRequest(t.GetLength());
                        t.DataForShow = NormalDataParse.ParsePduData(dataResult);
                    }
                );
                GetBuffData = new RelayCommand<DLMSSelfDefineTaiAngModel>(async t =>
                    {
                        t.DataForShow = "";
                        var dataResult = await Client.GetRequest(t.GetBuffer());
                        t.DataForShow =
                            Encoding.Default.GetString(NormalDataParse.ParsePduData(dataResult).StringToByte());
                    }
                );
            }
        }

        private RelayCommand<DLMSSelfDefineTaiAngModel> _getLogicNameDataCommand;

        public RelayCommand<DLMSSelfDefineTaiAngModel> GetLogicNameDataCommand
        {
            get => _getLogicNameDataCommand;
            set
            {
                _getLogicNameDataCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand<DLMSSelfDefineTaiAngModel> _etMeterAddressData;

        public RelayCommand<DLMSSelfDefineTaiAngModel> GetMeterAddressData
        {
            get => _etMeterAddressData;
            set
            {
                _etMeterAddressData = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand<DLMSSelfDefineTaiAngModel> _getDataLengthData;

        public RelayCommand<DLMSSelfDefineTaiAngModel> GetDataLengthData
        {
            get => _getDataLengthData;
            set
            {
                _getDataLengthData = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand<DLMSSelfDefineTaiAngModel> _getBuffData;

        public RelayCommand<DLMSSelfDefineTaiAngModel> GetBuffData
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