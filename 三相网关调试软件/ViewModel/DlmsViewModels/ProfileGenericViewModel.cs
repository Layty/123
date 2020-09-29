using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MySerialPortMaster;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects.DataStorage;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.Get;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.Set;
using 三相智慧能源网关调试软件.Model;
using Common = 三相智慧能源网关调试软件.Commom.Common;

namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{
    public class ProfileGenericViewModel : ViewModelBase
    {
        public DlmsSelfDefineCosemProfileGeneric Generic
        {
            get => _generic;
            set
            {
                _generic = value;
                RaisePropertyChanged();
            }
        }

        private DlmsSelfDefineCosemProfileGeneric _generic;


        public RelayCommand GetCaptureObjectsCommand
        {
            get => _GetCaptureObjectsCommand;
            set
            {
                _GetCaptureObjectsCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _GetCaptureObjectsCommand;

        public RelayCommand GetCapturePeriodCommand
        {
            get => _getCapturePeriodCommand;
            set
            {
                _getCapturePeriodCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _getCapturePeriodCommand;

        public RelayCommand SetCapturePeriodCommand
        {
            get => _SetCapturePeriodCommand;
            set
            {
                _SetCapturePeriodCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _SetCapturePeriodCommand;

        public RelayCommand GetEntriesInUseCommand { get; private set; }
        public RelayCommand GetProfileEntriesCommand { get; private set; }
        public RelayCommand GetSortMethodCommand { get; private set; }
        public RelayCommand GetBufferCommand { get; private set; }


        public DLMSClient Client { get; set; }


        public ObservableCollection<byte[]> ListObservableCollection
        {
            get => _listObservableCollection;
            set
            {
                _listObservableCollection = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<byte[]> _listObservableCollection;

        public SetRequest SetRequest = new SetRequest();

        public ProfileGenericViewModel()
        {
            Client = CommonServiceLocator.ServiceLocator.Current.GetInstance<DLMSClient>();
            Generic = new DlmsSelfDefineCosemProfileGeneric("1.0.99.1.0.255");
            Generic.CaptureObjects = new ObservableCollection<CaptureObjectDefinition>();

            GetCaptureObjectsCommand = new RelayCommand(async () =>
            {
                var responese = await Client.GetRequestAndWaitResponse(Generic.GetCaptureObjectsAttributeDescriptor()
                );
                if (responese != null)
                {
                    if (responese.GetResponseNormal.Result.Data.DataType == DataType.Array)
                    {
                        var Array = new DLMSArray();
                        var ar = responese.GetResponseNormal.Result.Data.ToPduStringInHex();
                        ar = ar.Substring(2);
                        if (Array.PduStringInHexConstructor(ref ar))
                        {
                            Generic.CaptureObjects.Clear();
                            for (int i = 0; i < Array.Items.Length; i++)
                            {
                                var captureObjectDefinition =
                                    CaptureObjectDefinition.CreateFromDlmsData(Array.Items[i]);
                                if (captureObjectDefinition != null)
                                {
                                    Generic.CaptureObjects.Add(captureObjectDefinition);
                                }
                            }
                        }
                    }
                }
            });

            GetCapturePeriodCommand = new RelayCommand(async
                () =>
            {
                var response = await Client.GetRequestAndWaitResponse(Generic.GetCosemAttributeDescriptor(4));
                if (response != null)
                {
                    response.GetResponseNormal.Result.Data.ValueDisplay.UInt32ValueDisplayFormat =
                        UInt32ValueDisplayFormat.IntValue;
                    Generic.CapturePeriod1.Value = response.GetResponseNormal.Result.Data.ValueDisplay.ValueString;
                }
            });
            SetCapturePeriodCommand = new RelayCommand(async () =>
            {
                await Client.SetRequestAndWaitResponse(Generic.GetCapturePeriodAttributeDescriptor(),
                    new DLMSDataItem(DataType.UInt32, Generic.CapturePeriod1.Value));
            });
            GetEntriesInUseCommand = new RelayCommand(async
                () =>
            {
                var resultBytes = await Client.GetRequest(Generic.GetEntriesInUseAttributeDescriptor());
                Generic.EntriesInUse = uint.Parse(NormalDataParse.ParsePduData(resultBytes));
            });
            GetProfileEntriesCommand = new RelayCommand(async
                () =>
            {
                var resultBytes = await Client.GetRequest(Generic.GetProfileEntriesAttributeDescriptor());
                Generic.ProfileEntries = uint.Parse(NormalDataParse.ParsePduData(resultBytes));
            });
            GetSortMethodCommand = new RelayCommand(async
                () =>
            {
                var resultBytes = await Client.GetRequest(Generic.GetSortMethodAttributeDescriptor());
                Generic.SortMethod = (SortMethod) ushort.Parse(NormalDataParse.ParsePduData(resultBytes));
            });

            GetBufferCommand = new RelayCommand(async
                () =>
            {
                var resultBytes = await Client.GetRequest(Generic.GetBufferAttributeDescriptor());
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