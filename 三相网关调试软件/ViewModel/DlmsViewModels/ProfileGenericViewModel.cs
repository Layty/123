using System.Collections.Generic;
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
        public ObservableCollection<CustomCosemProfileGenericModel> ProfileGenericCollection
        {
            get => _profileGenericCollection;
            set
            {
                _profileGenericCollection = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<CustomCosemProfileGenericModel> _profileGenericCollection;

        public AsyncRelayCommand<CustomCosemProfileGenericModel> GetCaptureObjectsCommand
        {
            get => _getCaptureObjectsCommand;
            set
            {
                _getCaptureObjectsCommand = value;
                OnPropertyChanged();
            }
        }

        private AsyncRelayCommand<CustomCosemProfileGenericModel> _getCaptureObjectsCommand;


        public RelayCommand<CustomCosemProfileGenericModel> SetCaptureObjectsCommand
        {
            get => _setCaptureObjectsCommand;
            set
            {
                _setCaptureObjectsCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand<CustomCosemProfileGenericModel> _setCaptureObjectsCommand;


        public RelayCommand<CustomCosemProfileGenericModel> GetCapturePeriodCommand
        {
            get => _getCapturePeriodCommand;
            set
            {
                _getCapturePeriodCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand<CustomCosemProfileGenericModel> _getCapturePeriodCommand;

        public RelayCommand<CustomCosemProfileGenericModel> SetCapturePeriodCommand
        {
            get => _setCapturePeriodCommand;
            set
            {
                _setCapturePeriodCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand<CustomCosemProfileGenericModel> _setCapturePeriodCommand;


        public RelayCommand<CustomCosemProfileGenericModel> GetEntriesInUseCommand
        {
            get => _getEntriesInUseCommand;
            set
            {
                _getEntriesInUseCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand<CustomCosemProfileGenericModel> _getEntriesInUseCommand;

        public RelayCommand<CustomCosemProfileGenericModel> GetProfileEntriesCommand
        {
            get => _getProfileEntriesCommand;
            set
            {
                _getProfileEntriesCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand<CustomCosemProfileGenericModel> _getProfileEntriesCommand;


        public RelayCommand<CustomCosemProfileGenericModel> GetSortMethodCommand
        {
            get => _getSortMethodCommand;
            set
            {
                _getSortMethodCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand<CustomCosemProfileGenericModel> _getSortMethodCommand;


        public RelayCommand<CustomCosemProfileGenericModel> GetBufferCommand
        {
            get => _getBufferCommand;
            set
            {
                _getBufferCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand<CustomCosemProfileGenericModel> _getBufferCommand;


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

            ProfileGenericCollection = new ObservableCollection<CustomCosemProfileGenericModel>();

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                ProfileGenericCollection.Add(new CustomCosemProfileGenericModel(dataTable.Rows[i][0].ToString())
                    {ProfileGenericName = dataTable.Rows[i][1].ToString()});
            }

            GetCaptureObjectsCommand = new AsyncRelayCommand<CustomCosemProfileGenericModel>(async (t) =>
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
            SetCaptureObjectsCommand = new RelayCommand<CustomCosemProfileGenericModel>(async (t) =>
            {
                List<DlmsDataItem> dataItems = new List<DlmsDataItem>() { };
                DlmsStructure structures = new DlmsStructure();
                foreach (var captureObjectDefinition in t.CaptureObjects)
                {
                    dataItems.Add(captureObjectDefinition.ToDlmsDataItem());
                }
                structures.Items = dataItems.ToArray();

                DlmsDataItem iii = new DlmsDataItem(DataType.Structure, structures);
                await Client.SetRequestAndWaitResponse(t.GetCaptureObjectsAttributeDescriptor(),
                    new DlmsDataItem(DataType.Array, iii));
            });
            GetCapturePeriodCommand = new RelayCommand<CustomCosemProfileGenericModel>(async
                (t) =>
            {
                var response = await Client.GetRequestAndWaitResponse(t.GetCosemAttributeDescriptor(4));
                if (response != null)
                {
                    response.GetResponseNormal.Result.Data.UInt32ValueDisplayFormat =
                        UInt32ValueDisplayFormat.IntValue;
                    t.CapturePeriod.Value = response.GetResponseNormal.Result.Data.ValueString;
                }
            });
            SetCapturePeriodCommand = new RelayCommand<CustomCosemProfileGenericModel>(async (t) =>
            {
                await Client.SetRequestAndWaitResponse(t.GetCapturePeriodAttributeDescriptor(),
                    new DlmsDataItem(DataType.UInt32, t.CapturePeriod.Value));
            });
            GetEntriesInUseCommand = new RelayCommand<CustomCosemProfileGenericModel>(async
                (t) =>
            {
                var response = await Client.GetRequestAndWaitResponse(t.GetEntriesInUseAttributeDescriptor());
                if (response != null)
                {
                    response.GetResponseNormal.Result.Data.UInt32ValueDisplayFormat =
                        UInt32ValueDisplayFormat.IntValue;
                    t.EntriesInUse.Value = response.GetResponseNormal.Result.Data.ValueString;
                }
            });
            GetProfileEntriesCommand = new RelayCommand<CustomCosemProfileGenericModel>(async
                (t) =>
            {
                var resultBytes = await Client.GetRequest(t.GetProfileEntriesAttributeDescriptor());
                t.ProfileEntries = uint.Parse(NormalDataParse.ParsePduData(resultBytes));
            });
            GetSortMethodCommand = new RelayCommand<CustomCosemProfileGenericModel>(async
                (t) =>
            {
                var resultBytes = await Client.GetRequest(t.GetSortMethodAttributeDescriptor());
                t.SortMethod = (SortMethod) ushort.Parse(NormalDataParse.ParsePduData(resultBytes));
            });

            GetBufferCommand = new RelayCommand<CustomCosemProfileGenericModel>(async
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