using System.Collections.Generic;
using System.Collections.ObjectModel;
using 三相智慧能源网关调试软件.Model;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage;

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

        public RelayCommand<CustomCosemProfileGenericModel> GetCaptureObjectsCommand { get; set; }

        public RelayCommand<CustomCosemProfileGenericModel> SetCaptureObjectsCommand { get; set; }


        public RelayCommand<CustomCosemProfileGenericModel> GetCapturePeriodCommand { get; set; }


        public RelayCommand<CustomCosemProfileGenericModel> SetCapturePeriodCommand { get; set; }


        public RelayCommand<CustomCosemProfileGenericModel> GetEntriesInUseCommand { get; set; }

        public RelayCommand<CustomCosemProfileGenericModel> GetProfileEntriesCommand { get; set; }


        public RelayCommand<CustomCosemProfileGenericModel> GetSortMethodCommand { get; set; }


        public RelayCommand<CustomCosemProfileGenericModel> GetBufferCommand { get; set; }

        public DlmsClient Client { get; set; }


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
            Client = CommonServiceLocator.ServiceLocator.Current.GetInstance<DlmsClient>();
            ExcelHelper excel = new ExcelHelper(Properties.Settings.Default.ExcelFileName);
            var dataTable = excel.GetExcelDataTable(Properties.Settings.Default.DlmsProfileGenericSheetName);

            ProfileGenericCollection = new ObservableCollection<CustomCosemProfileGenericModel>();

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                ProfileGenericCollection.Add(new CustomCosemProfileGenericModel(dataTable.Rows[i][0].ToString())
                    {ProfileGenericName = dataTable.Rows[i][1].ToString()});
            }

            GetCaptureObjectsCommand = new RelayCommand<CustomCosemProfileGenericModel>(async (t) =>
            {
                var response = await Client.GetRequestAndWaitResponse(t.GetCaptureObjectsAttributeDescriptor()
                );
                if (response != null)
                {
                    if (response.GetResponseNormal.Result.Data.DataType == DataType.Array)
                    {
                        var array = new DLMSArray();
                        var ar = response.GetResponseNormal.Result.Data.ToPduStringInHex();
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
                List<DlmsDataItem> dataItems = new List<DlmsDataItem>();

                foreach (var captureObjectDefinition in t.CaptureObjects)
                {
                    dataItems.Add(captureObjectDefinition.ToDlmsDataItem());
                }

                DLMSArray array = new DLMSArray() {Items = dataItems.ToArray()};
                await Client.SetRequestAndWaitResponse(t.GetCaptureObjectsAttributeDescriptor(),
                    new DlmsDataItem(DataType.Array, array));
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
                var response = await Client.GetRequestAndWaitResponse(t.GetProfileEntriesAttributeDescriptor());
                if (response != null)
                {
                    t.ProfileEntries.Value = response.GetResponseNormal.Result.Data.ValueString;
                }
            });
            GetSortMethodCommand = new RelayCommand<CustomCosemProfileGenericModel>(async
                (t) =>
            {
                var response = await Client.GetRequestAndWaitResponse(t.GetSortMethodAttributeDescriptor());
                if (response != null)
                    t.SortMethod = (SortMethod) ushort.Parse(response.GetResponseNormal.Result.Data.ValueString);
            });

            GetBufferCommand = new RelayCommand<CustomCosemProfileGenericModel>(async
                (t) =>
            {
                var response = await Client.GetRequestAndWaitResponse(t.GetBufferAttributeDescriptor());
//                var stringTo = NormalDataParse.ParsePduData(resultBytes).StringToByte();
//                var splitCountLength = (stringTo.Length - 1) / stringTo[0];
//                ListObservableCollection = new ObservableCollection<byte[]>();
//                for (int i = 0; i < stringTo[0]; i++)
//                {
//                    var index = 1 + (i * splitCountLength);
//                    ListObservableCollection.Add(stringTo.Skip(index).Take(splitCountLength).ToArray());
//                }
            });
        }
    }
}