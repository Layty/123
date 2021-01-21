using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using CommonServiceLocator;
using 三相智慧能源网关调试软件.Model;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.CosemObjects;
using MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage;
using MyDlmsStandard.Common;
using Newtonsoft.Json;
using RestSharp;

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

        public RelayCommand<CustomCosemProfileGenericModel> GetAllBufferCommand { get; set; }

        public RelayCommand<CustomCosemProfileGenericModel> GetBufferByRangeCommand { get; set; }
        public RelayCommand<CustomCosemProfileGenericModel> GetBufferByEntryCommand { get; set; }

        public RelayCommand<CustomCosemProfileGenericModel> GetBufferByClockCommand { get; set; }

        public DlmsClient Client { get; set; }

        public ProfileGenericViewModel()
        {
            Client = ServiceLocator.Current.GetInstance<DlmsClient>();
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
                        if (array.PduStringInHexConstructor(ref ar))
                        {
                            t.CaptureObjects.Clear();
                            foreach (var t1 in array.Items)
                            {
                                var captureObjectDefinition =
                                    CaptureObjectDefinition.CreateFromDlmsData(t1);
                                if (captureObjectDefinition != null)
                                {
                                    var client = new RestClient(
                                        $"{Properties.Settings.Default.WebApiUrl}/CosemObjects/ByObis/{captureObjectDefinition.LogicalName}");
                                    var request = new RestRequest(Method.GET);
                                    IRestResponse responseWebApi = client.Execute(request);
                                    var getCosemObject =
                                        JsonConvert.DeserializeObject<CosemObjectEditModel>(responseWebApi.Content);
                                    if (getCosemObject != null)
                                    {
                                        captureObjectDefinition.Description = getCosemObject.Name;
                                    }

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

            GetAllBufferCommand = new RelayCommand<CustomCosemProfileGenericModel>(async
                (t) =>
            {
                if (t.CaptureObjects == null)
                {
                    return;
                }

                var response = await Client.GetRequestAndWaitResponse(t.GetBufferAttributeDescriptor());
            });
            GetBufferByRangeCommand = new RelayCommand<CustomCosemProfileGenericModel>(async (t) =>
            {
                var response =
                    await Client.GetRequestAndWaitResponseArray(t.GetBufferAttributeDescriptorWithSelectionByRange());
            });
            GetBufferByEntryCommand = new RelayCommand<CustomCosemProfileGenericModel>(async (t) =>
            {
                t.Buffer.Clear();
                var response =
                    await Client.GetRequestAndWaitResponseArray(t.GetBufferAttributeDescriptorWithSelectionByEntry());
                StringBuilder stringBuilder = new StringBuilder();
                if (response != null)
                {
                    if (response.Count == 1)
                    {
                        var ttttt = (DLMSArray) response[0].GetResponseNormal.Result.Data.Value;

                        if (ttttt.DataType == DataType.Array)
                        {
                            DLMSArray array = ttttt;
                            List<DlmsStructure> structures = new List<DlmsStructure>();
                            foreach (var item in array.Items)
                            {
                                structures.Add((DlmsStructure) item.Value);
                            }

                            DispatcherHelper.CheckBeginInvokeOnUI(() => { t.Buffer = structures; });
                        }
                    }
                    else
                    {
                        foreach (var getResponse in response)
                        {
                            stringBuilder.Append(getResponse.GetResponseWithDataBlock.DataBlockG.RawData.Value);
                        }

                        var strr = stringBuilder.ToString();
                        DlmsDataItem vDataItem = new DlmsDataItem();

                        var foo = vDataItem.PduStringInHexConstructor(ref strr);
                        if (!foo)
                        {
                            return;
                        }

                        if (vDataItem.DataType == DataType.Array)
                        {
                            DLMSArray array = (DLMSArray) vDataItem.Value;
                            List<DlmsStructure> structures = new List<DlmsStructure>();
                            foreach (var item in array.Items)
                            {
                                structures.Add((DlmsStructure) item.Value);
                            }

                            DispatcherHelper.CheckBeginInvokeOnUI(() => { t.Buffer = structures; });
                        }
                    }


                    ;
                }
            });

            GetBufferByClockCommand = new RelayCommand<CustomCosemProfileGenericModel>(async (t) =>
            {
                t.Buffer.Clear();
                t.ProfileGenericRangeDescriptor = new ProfileGenericRangeDescriptor()
                {
                    RestrictingObject = new CaptureObjectDefinition()
                        {AttributeIndex = 2, ClassId = 8, DataIndex = 0, LogicalName = "0.0.1.0.0.255"},
                    FromValue = new DlmsDataItem(DataType.OctetString,
                        new CosemClock(DateTime.Now.Subtract(new TimeSpan(0, 0, 5, 0))).GetDateTimeBytes()
                            .ByteToString()),
                    ToValue = new DlmsDataItem(DataType.OctetString,
                        new CosemClock(DateTime.Now).GetDateTimeBytes().ByteToString()),
                    SelectedValues = new List<CaptureObjectDefinition>()
                };
                Console.WriteLine(t.ProfileGenericRangeDescriptor.ToDlmsDataItem().ToPduStringInHex());
                var response =
                    await Client.GetRequestAndWaitResponseArray(t.GetBufferAttributeDescriptorWithSelectionByRange());

                StringBuilder stringBuilder = new StringBuilder();
                if (response != null)
                {
                    if (response.Count == 1)
                    {
                        var ttttt = (DLMSArray) response[0].GetResponseNormal.Result.Data.Value;

                        if (ttttt.DataType == DataType.Array)
                        {
                            DLMSArray array = ttttt;
                            List<DlmsStructure> structures = new List<DlmsStructure>();
                            foreach (var item in array.Items)
                            {
                                structures.Add((DlmsStructure) item.Value);
                            }

                            DispatcherHelper.CheckBeginInvokeOnUI(() => { t.Buffer = structures; });
                        }
                    }
                    else
                    {
                        foreach (var getResponse in response)
                        {
                            stringBuilder.Append(getResponse.GetResponseWithDataBlock.DataBlockG.RawData.Value);
                        }

                        var strr = stringBuilder.ToString();
                        DlmsDataItem vDataItem = new DlmsDataItem();

                        var foo = vDataItem.PduStringInHexConstructor(ref strr);
                        if (!foo)
                        {
                            return;
                        }

                        if (vDataItem.DataType == DataType.Array)
                        {
                            DLMSArray array = (DLMSArray) vDataItem.Value;
                            List<DlmsStructure> structures = new List<DlmsStructure>();
                            foreach (var item in array.Items)
                            {
                                structures.Add((DlmsStructure) item.Value);
                            }

                            DispatcherHelper.CheckBeginInvokeOnUI(() => { t.Buffer = structures; });
                        }
                    }


                    ;
                }
            });
        }
    }
}