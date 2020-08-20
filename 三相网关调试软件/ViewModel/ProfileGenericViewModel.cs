using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MySerialPortMaster;
using 三相智慧能源网关调试软件.DLMS;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.Get;
using 三相智慧能源网关调试软件.Model;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class ProfileGenericViewModel : ViewModelBase
    {
        public DLMSSelfDefineProfileGeneric Generic
        {
            get => _generic;
            set
            {
                _generic = value;
                RaisePropertyChanged();
            }
        }

        private DLMSSelfDefineProfileGeneric _generic;


        public RelayCommand GetCapturePeriodCommand
        {
            get => _getCapturePeriodCommand;
            set
            {
                _getCapturePeriodCommand = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand GetEntriesInUseCommand { get; private set; }
        public RelayCommand GetProfileEntriesCommand { get; private set; }
        public RelayCommand GetSortMethodCommand { get; private set; }
        public RelayCommand GetBufferCommand { get; private set; }

        private RelayCommand _getCapturePeriodCommand;
        public DLMSClient Client { get; set; }


        public ObservableCollection<byte[]> ListObservableCollection
        {
            get => _listObservableCollection;
            set { _listObservableCollection = value; RaisePropertyChanged(); }
        }
        private ObservableCollection<byte[]> _listObservableCollection;
         public   GetRequest getRequest = new GetRequest();

        public ProfileGenericViewModel()
        {
            Client = CommonServiceLocator.ServiceLocator.Current.GetInstance<DLMSClient>();
            Generic = new DLMSSelfDefineProfileGeneric("1.0.99.1.0.255");
            getRequest.GetRequestNormal=new GetRequestNormal();
         
            GetCapturePeriodCommand = new RelayCommand(async
                () =>
            {
                getRequest.GetRequestNormal.AttributeDescriptor = Generic.GetCosemAttributeDescriptor(4);
                 var resultBytes = await Client.GetRequest(getRequest);
                Generic.CapturePeriod = uint.Parse(NormalDataParse.ParsePduData(resultBytes));
            });
            GetEntriesInUseCommand = new RelayCommand(async
                () =>
            {
                getRequest.GetRequestNormal = new GetRequestNormal(Generic.GetEntriesInUseAttributeDescriptor());
                var resultBytes = await Client.GetRequest(getRequest);
                Generic.EntriesInUse = uint.Parse(NormalDataParse.ParsePduData(resultBytes));
            });
            GetProfileEntriesCommand = new RelayCommand(async
                () =>
            {
                getRequest.GetRequestNormal.AttributeDescriptor = Generic.GetProfileEntriesAttributeDescriptor();
                var resultBytes = await Client.GetRequest(getRequest);
                Generic.ProfileEntries = uint.Parse(NormalDataParse.ParsePduData(resultBytes));
            });
            GetSortMethodCommand = new RelayCommand(async
                () =>
            {
                getRequest.GetRequestNormal.AttributeDescriptor = Generic.GetSortMethodAttributeDescriptor();
                var resultBytes = await Client.GetRequest(getRequest);
                Generic.SortMethod = (SortMethod) ushort.Parse(NormalDataParse.ParsePduData(resultBytes));
            });

            GetBufferCommand = new RelayCommand(async
                () =>
            {
                getRequest.GetRequestNormal.AttributeDescriptor = Generic.GetBufferAttributeDescriptor();
                var resultBytes = await Client.GetRequest(getRequest);
                var stringTo = NormalDataParse.ParsePduData(resultBytes).StringToByte();
                var splitCountLength = (stringTo.Length - 1) / stringTo[0];
                var Index = 1;
                ListObservableCollection = new ObservableCollection<byte[]>();
                for (int i = 0; i < stringTo[0]; i++)
                {
                    Index = 1 + (i * splitCountLength);
                    ListObservableCollection.Add(stringTo.Skip(Index).Take(splitCountLength).ToArray()); 
                }
            });
        }
    }
}