using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using 三相智慧能源网关调试软件.DLMS;
using 三相智慧能源网关调试软件.Model;

namespace 三相智慧能源网关调试软件.MyControl
{
    public class DataViewModel : ViewModelBase
    {
        public ObservableCollection<DLMSSelfDefineData> Registers
        {
            get => _registers;
            set
            {
                _registers = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<DLMSSelfDefineData> _registers;

        public RelayCommand<DLMSSelfDefineData> GetValueCommand
        {
            get => _getValueCommand;
            set
            {
                _getValueCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand<DLMSSelfDefineData> _getValueCommand;

        public DLMSClient Client { get; set; }

        public DataViewModel()
        {
            Client = CommonServiceLocator.ServiceLocator.Current.GetInstance<DLMSClient>();
            Registers = new ObservableCollection<DLMSSelfDefineData>()
            {
                new DLMSSelfDefineData("0.0.128.50.128.255") {DataName = "泰昂抽屉柜个数"},
                new DLMSSelfDefineData("0.0.96.14.0.255") {DataName = "Currently active tariff"},
                new DLMSSelfDefineData("1.0.0.2.0.255") {DataName = "Software version"},
                new DLMSSelfDefineData("0.0.96.5.0.255") {DataName = "工厂模式"},
            };
            GetValueCommand = new RelayCommand<DLMSSelfDefineData>(async t =>
            {
                t.Value = "";
                var dataResult = await Client.GetRequest(t.GetValue());
                t.Value = NormalDataParse.ParsePduData(dataResult);
            });
        }
    }
}