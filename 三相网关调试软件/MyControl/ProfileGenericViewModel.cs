using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MySerialPortMaster;
using 三相智慧能源网关调试软件.DLMS;
using 三相智慧能源网关调试软件.DLMS.ApplicationLayEnums;
using 三相智慧能源网关调试软件.Model;

namespace 三相智慧能源网关调试软件.MyControl
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
            get => _GetCapturePeriodCommand;
            set
            {
                _GetCapturePeriodCommand = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand GetEntriesInUseCommand { get; private set; }
        public RelayCommand GetProfileEntriesCommand { get; private set; }
        public RelayCommand GetSortMethodCommand { get; private set; }
        public RelayCommand GetBufferCommand { get; private set; }

        private RelayCommand _GetCapturePeriodCommand;
        public DLMSClient Client { get; set; }


        public ObservableCollection<byte[]> ListObservableCollection
        {
            get => _ListObservableCollection;
            set { _ListObservableCollection = value; RaisePropertyChanged(); }
        }
        private ObservableCollection<byte[]> _ListObservableCollection;


        public ProfileGenericViewModel()
        {
            Client = CommonServiceLocator.ServiceLocator.Current.GetInstance<DLMSClient>();
            Generic = new DLMSSelfDefineProfileGeneric("1.0.99.1.0.255");
            GetCapturePeriodCommand = new RelayCommand(async
                () =>
            {
                var resultBytes = await Client.GetRequest(Generic.GetCapturePeriod());
                Generic.CapturePeriod = uint.Parse(NormalDataParse.ParsePduData(resultBytes));
            });
            GetEntriesInUseCommand = new RelayCommand(async
                () =>
            {
                var resultBytes = await Client.GetRequest(Generic.GetEntriesInUse());
                Generic.EntriesInUse = uint.Parse(NormalDataParse.ParsePduData(resultBytes));
            });
            GetProfileEntriesCommand = new RelayCommand(async
                () =>
            {
                var resultBytes = await Client.GetRequest(Generic.GetProfileEntries());
                Generic.ProfileEntries = uint.Parse(NormalDataParse.ParsePduData(resultBytes));
            });
            GetSortMethodCommand = new RelayCommand(async
                () =>
            {
                var resultBytes = await Client.GetRequest(Generic.GetSortMethod());
                Generic.SortMethod = (SortMethod) ushort.Parse(NormalDataParse.ParsePduData(resultBytes));
            });

            GetBufferCommand = new RelayCommand(async
                () =>
            {
                var resultBytes = await Client.GetRequest(Generic.GetBuffer());
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