using System.Collections.ObjectModel;
using System.Linq;
using MySerialPortMaster;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects.DataStorage;
using 三相智慧能源网关调试软件.Model;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{
    public class ProfileGenericViewModel : ObservableObject
    {
        public ObservableCollection<DlmsSelfDefineCosemProfileGeneric> ProfileGenericCollection
        {
            get => _profileGenericCollection;
            set
            {
                _profileGenericCollection = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<DlmsSelfDefineCosemProfileGeneric> _profileGenericCollection;

        public AsyncRelayCommand<DlmsSelfDefineCosemProfileGeneric> GetCaptureObjectsCommand
        {
            get => _getCaptureObjectsCommand;
            set
            {
                _getCaptureObjectsCommand = value;
                OnPropertyChanged();
            }
        }

        private AsyncRelayCommand<DlmsSelfDefineCosemProfileGeneric> _getCaptureObjectsCommand;

        public RelayCommand<DlmsSelfDefineCosemProfileGeneric> GetCapturePeriodCommand
        {
            get => _getCapturePeriodCommand;
            set
            {
                _getCapturePeriodCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand<DlmsSelfDefineCosemProfileGeneric> _getCapturePeriodCommand;

        public RelayCommand<DlmsSelfDefineCosemProfileGeneric> SetCapturePeriodCommand
        {
            get => _setCapturePeriodCommand;
            set
            {
                _setCapturePeriodCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand<DlmsSelfDefineCosemProfileGeneric> _setCapturePeriodCommand;


        public RelayCommand<DlmsSelfDefineCosemProfileGeneric> GetEntriesInUseCommand
        {
            get => _getEntriesInUseCommand;
            set
            {
                _getEntriesInUseCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand<DlmsSelfDefineCosemProfileGeneric> _getEntriesInUseCommand;

        public RelayCommand<DlmsSelfDefineCosemProfileGeneric> GetProfileEntriesCommand
        {
            get => _getProfileEntriesCommand;
            set
            {
                _getProfileEntriesCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand<DlmsSelfDefineCosemProfileGeneric> _getProfileEntriesCommand;


        public RelayCommand<DlmsSelfDefineCosemProfileGeneric> GetSortMethodCommand
        {
            get => _getSortMethodCommand;
            set
            {
                _getSortMethodCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand<DlmsSelfDefineCosemProfileGeneric> _getSortMethodCommand;


        public RelayCommand<DlmsSelfDefineCosemProfileGeneric> GetBufferCommand
        {
            get => _getBufferCommand;
            set
            {
                _getBufferCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand<DlmsSelfDefineCosemProfileGeneric> _getBufferCommand;


        public DLMSClient Client { get; set; }


        public ObservableCollection<byte[]> ListObservableCollection
        {
            get => _listObservableCollection;
            set
            {
                _listObservableCollection = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<byte[]> _listObservableCollection;


        public ProfileGenericViewModel()
        {
            
            Client = CommonServiceLocator.ServiceLocator.Current.GetInstance<DLMSClient>();
            ExcelHelper excel = new ExcelHelper(Properties.Settings.Default.ExcelFileName);
            var dataTable = excel.GetExcelDataTable(Properties.Settings.Default.DlmsProfileGenericSheetName);

            ProfileGenericCollection = new ObservableCollection<DlmsSelfDefineCosemProfileGeneric>();

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                ProfileGenericCollection.Add(new DlmsSelfDefineCosemProfileGeneric(dataTable.Rows[i][0].ToString())
                    {ProfileGenericName = dataTable.Rows[i][1].ToString()});
            }

            GetCaptureObjectsCommand = new AsyncRelayCommand<DlmsSelfDefineCosemProfileGeneric>(async (t) =>
            {
                var responese = await Client.GetRequestAndWaitResponse(t.GetCaptureObjectsAttributeDescriptor()
                );
                if (responese != null)
                {
                    if (responese.GetResponseNormal.Result.Data.DataType == DataType.Array)
                    {
                        var array = new DLMSArray();
                        var ar = responese.GetResponseNormal.Result.Data.ToPduStringInHex();
                        ar = ar.Substring(2);
                        if (array.PduStringInHexConstructor(ref ar))
                        {
                            t.CaptureObjects.Clear();
                            for (int i = 0; i < array.Items.Length; i++)
                            {
                                var captureObjectDefinition =
                                    CaptureObjectDefinition.CreateFromDlmsData(array.Items[i]);
                                if (captureObjectDefinition != null)
                                {
                                    t.CaptureObjects.Add(captureObjectDefinition);
                                }
                            }
                        }
                    }
                }
            });

            GetCapturePeriodCommand = new RelayCommand<DlmsSelfDefineCosemProfileGeneric>(async
                (t) =>
            {
                var response = await Client.GetRequestAndWaitResponse(t.GetCosemAttributeDescriptor(4));
                if (response != null)
                {
                    response.GetResponseNormal.Result.Data.ValueDisplay.UInt32ValueDisplayFormat =
                        UInt32ValueDisplayFormat.IntValue;
                    t.CapturePeriod1.Value = response.GetResponseNormal.Result.Data.ValueDisplay.ValueString;
                }
            });
            SetCapturePeriodCommand = new RelayCommand<DlmsSelfDefineCosemProfileGeneric>(async (t) =>
            {
                await Client.SetRequestAndWaitResponse(t.GetCapturePeriodAttributeDescriptor(),
                    new DLMSDataItem(DataType.UInt32, t.CapturePeriod1.Value));
            });
            GetEntriesInUseCommand = new RelayCommand<DlmsSelfDefineCosemProfileGeneric>(async
                (t) =>
            {
                var response = await Client.GetRequestAndWaitResponse(t.GetEntriesInUseAttributeDescriptor());
                if (response != null)
                {
                    response.GetResponseNormal.Result.Data.ValueDisplay.UInt32ValueDisplayFormat =
                        UInt32ValueDisplayFormat.IntValue;
                    t.EntriesInUse.Value = response.GetResponseNormal.Result.Data.ValueDisplay.ValueString;
                }
            });
            GetProfileEntriesCommand = new RelayCommand<DlmsSelfDefineCosemProfileGeneric>(async
                (t) =>
            {
                var resultBytes = await Client.GetRequest(t.GetProfileEntriesAttributeDescriptor());
                t.ProfileEntries = uint.Parse(NormalDataParse.ParsePduData(resultBytes));
            });
            GetSortMethodCommand = new RelayCommand<DlmsSelfDefineCosemProfileGeneric>(async
                (t) =>
            {
                var resultBytes = await Client.GetRequest(t.GetSortMethodAttributeDescriptor());
                t.SortMethod = (SortMethod) ushort.Parse(NormalDataParse.ParsePduData(resultBytes));
            });

            GetBufferCommand = new RelayCommand<DlmsSelfDefineCosemProfileGeneric>(async
                (t) =>
            {
                var resultBytes = await Client.GetRequest(t.GetBufferAttributeDescriptor());
                var stringTo = NormalDataParse.ParsePduData(resultBytes).StringToByte();
                var splitCountLength = (stringTo.Length - 1) / stringTo[0];
                ListObservableCollection = new ObservableCollection<byte[]>();
                for (int i = 0; i < stringTo[0]; i++)
                {
                    var index = 1 + (i * splitCountLength);
                    ListObservableCollection.Add(stringTo.Skip(index).Take(splitCountLength).ToArray());
                }
            });
        }
    }
}