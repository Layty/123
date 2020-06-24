using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using 三相智慧能源网关调试软件.DLMS;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;
using 三相智慧能源网关调试软件.Model;

namespace 三相智慧能源网关调试软件.ViewModel
{
   
    public class DataViewModel : ViewModelBase
    {
        public ObservableCollection<DLMSSelfDefineData> DataCollection
        {
            get => _dataCollection;
            set
            {
                _dataCollection = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<DLMSSelfDefineData> _dataCollection;

        public RelayCommand<DLMSSelfDefineData> GetLogicNameCommand
        {
            get => _getLogicNameCommand;
            set
            {
                _getLogicNameCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand<DLMSSelfDefineData> _getLogicNameCommand;

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

        public RelayCommand<DLMSSelfDefineData> SetValueCommand
        {
            get => _setValueCommand;
            set
            {
                _setValueCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand<DLMSSelfDefineData> _setValueCommand;

        public DLMSClient Client { get; set; }

        public DataViewModel()
        {
            Client = CommonServiceLocator.ServiceLocator.Current.GetInstance<DLMSClient>();
            ExcelHelper excel = new ExcelHelper("DLMS设备信息.xls");
            var dataTable = excel.GetExcelDataTable("Data$");

            DataCollection = new ObservableCollection<DLMSSelfDefineData>();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                DataCollection.Add(new DLMSSelfDefineData(dataTable.Rows[i][0].ToString())
                    { DataName = dataTable.Rows[i][1].ToString() });
            }
            //DataCollection = new ObservableCollection<DLMSSelfDefineData>()
            //{
            //    new DLMSSelfDefineData("0.0.128.50.128.255") {DataName = "泰昂抽屉柜个数"},
            //    new DLMSSelfDefineData("0.0.96.14.0.255") {DataName = "Currently active tariff"},
            //    new DLMSSelfDefineData("1.0.0.2.0.255") {DataName = "Software version"},
            //    new DLMSSelfDefineData("0.0.96.5.0.255") {DataName = "工厂模式"},
            //};
            GetLogicNameCommand=new RelayCommand<DLMSSelfDefineData>(async t =>
            {
                t.DataValue = "";
                var dataResult = await Client.GetRequest(t.GetLogicName());
                t.DataValue = NormalDataParse.ParsePduData(dataResult);

            });
            GetValueCommand = new RelayCommand<DLMSSelfDefineData>(async t =>
            {
                t.DataValue = "";
                var dataResult = await Client.GetRequest(t.GetValue());
                t.DataValue = NormalDataParse.ParsePduData(dataResult);
            });
          SetValueCommand=new RelayCommand<DLMSSelfDefineData>(async (t) =>
          {
              //t.DataValue = "";
              //var dataResult = await Client.SetRequest(t.SetValue(new DLMSDataItem()));
          });
        }
    }
}