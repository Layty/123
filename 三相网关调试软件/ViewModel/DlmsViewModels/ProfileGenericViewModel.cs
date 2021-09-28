using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CommonServiceLocator;
using 三相智慧能源网关调试软件.Model;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.CosemObjects;
using MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage;
using MyDlmsStandard.ApplicationLay.Get;
using MyDlmsStandard.Common;
using Newtonsoft.Json;
using RestSharp;
using System.Data;
using System;
using 三相智慧能源网关调试软件.Helpers;

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

        public CustomCosemProfileGenericModel CurrentCustomCosemProfileGenericModel
        {
            get => _currentCustomCosemProfileGenericModel;
            set
            {
                _currentCustomCosemProfileGenericModel = value;
                OnPropertyChanged();
            }
        }

        private CustomCosemProfileGenericModel _currentCustomCosemProfileGenericModel;

        public AsyncRelayCommand<CustomCosemProfileGenericModel> GetCaptureObjectsCommand { get; set; }
        public RelayCommand<CustomCosemProfileGenericModel> SetCaptureObjectsCommand { get; set; }
        public AsyncRelayCommand<CustomCosemProfileGenericModel> GetCapturePeriodCommand { get; set; }
        public AsyncRelayCommand<CustomCosemProfileGenericModel> SetCapturePeriodCommand { get; set; }
        public AsyncRelayCommand<CustomCosemProfileGenericModel> GetEntriesInUseCommand { get; set; }

        public AsyncRelayCommand<CustomCosemProfileGenericModel> GetProfileEntriesCommand { get; set; }

        public AsyncRelayCommand<CustomCosemProfileGenericModel> GetSortMethodCommand { get; set; }
        public RelayCommand<CustomCosemProfileGenericModel> GetAllAttrCommand { get;  set; }
        public RelayCommand<CustomCosemProfileGenericModel> GetAllBufferCommand { get; set; }

        public RelayCommand<CustomCosemProfileGenericModel> GetBufferByRangeCommand { get; set; }
        public RelayCommand<CustomCosemProfileGenericModel> GetBufferByEntryCommand { get; set; }
        public RelayCommand<CustomCosemProfileGenericModel> ClearBufferCommand { get; set; }
        public DlmsClient Client { get; set; }
        public NetLogViewModel NetLogViewModel { get; set; }
        public RelayCommand<CustomCosemProfileGenericModel> ExportBufferToDataBaseCommand { get; set; }
     
        public ProfileGenericViewModel()
        {                   
            NetLogViewModel = ServiceLocator.Current.GetInstance<NetLogViewModel>();
            Energy = new List<Energy>();
            Client = ServiceLocator.Current.GetInstance<DlmsClient>();
            ExcelHelper excel = new ExcelHelper(Properties.Settings.Default.ExcelFileName);
            var dataTable = excel.GetExcelDataTable(Properties.Settings.Default.DlmsProfileGenericSheetName);
            ExportBufferToDataBaseCommand = new RelayCommand<CustomCosemProfileGenericModel>((t) => {
                Energy = new List<Energy>();                              
                if (t.Buffer != null)
                {
                    foreach (var item in t.Buffer)
                    {
                        var dataItems = item.Items;
                        var clock = new CosemClock();
                        string dt = dataItems[0].Value.ToString();
                        var b = clock.DlmsClockParse(dt.StringToByte());
                        if (b)
                        {
                            EnergyCaptureObjects energyCaptureObjects = new EnergyCaptureObjects
                            {
                                DateTime = clock.ToDateTime(),
                                ImportActiveEnergyTotal = dataItems[1].ValueString,
                                ImportActiveEnergyT1 = dataItems[2].ValueString,
                                ImportActiveEnergyT2 = dataItems[3].ValueString,
                                ImportActiveEnergyT3 = dataItems[4].ValueString,
                                ImportActiveEnergyT4 = dataItems[5].ValueString,
                                ExportActiveEnergyTotal = dataItems[6].ValueString,
                                ImportReactiveEnergyTotal = dataItems[7].ValueString,
                                ExportReactiveEnergyTotal = dataItems[8].ValueString
                            };

                            Energy.Add(new Energy()
                            {
                                EnergyData = JsonConvert.SerializeObject(energyCaptureObjects),
                                Id = Guid.NewGuid(),
                                DateTime = clock.ToDateTime()
                            });
                        }
                    }
                }
                InsertData();
            });
            ProfileGenericCollection = new ObservableCollection<CustomCosemProfileGenericModel>();

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                ProfileGenericCollection.Add(new CustomCosemProfileGenericModel(dataTable.Rows[i][0].ToString())
                    {ProfileGenericName = dataTable.Rows[i][1].ToString()});
            }


            GetCaptureObjectsCommand = new AsyncRelayCommand<CustomCosemProfileGenericModel>(async
                (t) =>
            {
                t.CaptureObjects.Clear();
                var responses = await Client.GetRequestAndWaitResponseArray(t.CaptureObjectsAttributeDescriptor);
                StringBuilder stringBuilder = new StringBuilder();
                if (responses != null)
                {
                    if (responses.Count == 1)
                    {
                        if (!responses[0].GetResponseNormal.Result.IsSuccessed())
                        {
                            return;
                        }
                        var ttttt = (DLMSArray) responses[0].GetResponseNormal.Result.Data.Value;

                        if (ttttt.DataType == DataType.Array)
                        {
                            var array1 = ttttt;
                            var ar = responses[0].GetResponseNormal.Result.Data.ToPduStringInHex();
                            if (array1.PduStringInHexConstructor(ref ar))
                            {
                                t.CaptureObjects.Clear();
                                foreach (var t1 in array1.Items)
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
                                            JsonConvert.DeserializeObject<CosemObjectEditModel>(responseWebApi
                                                .Content);
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
                    else
                    {
                        foreach (var getResponse in responses)
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
                                        JsonConvert.DeserializeObject<CosemObjectEditModel>(responseWebApi
                                            .Content);
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
            SetCaptureObjectsCommand = new RelayCommand<CustomCosemProfileGenericModel>(async
                (t) =>
            {
                DLMSArray array = new DLMSArray()
                {
                    Items = t.CaptureObjects.Select(captureObjectDefinition => captureObjectDefinition.ToDlmsDataItem())
                        .ToArray()
                };
                await Client.SetRequestAndWaitResponse(t.CaptureObjectsAttributeDescriptor,
                    new DlmsDataItem(DataType.Array, array));
            });
            GetCapturePeriodCommand = new AsyncRelayCommand<CustomCosemProfileGenericModel>(async
                (t) =>
            {
                var response = await Client.GetRequestAndWaitResponse(t.GetCosemAttributeDescriptor(4));
                if (response != null && response.GetResponseNormal.Result.DataAccessResult.Value == "00")
                {
                    response.GetResponseNormal.Result.Data.UInt32ValueDisplayFormat =
                        UInt32ValueDisplayFormat.IntValue;
                    t.CapturePeriod.Value = response.GetResponseNormal.Result.Data.ValueString;
                }
            });
            SetCapturePeriodCommand = new AsyncRelayCommand<CustomCosemProfileGenericModel>(async
                (t) =>
            {
                await Client.SetRequestAndWaitResponse(t.CapturePeriodAttributeDescriptor,
                    new DlmsDataItem(DataType.UInt32, t.CapturePeriod.Value));
            });
            GetEntriesInUseCommand = new AsyncRelayCommand<CustomCosemProfileGenericModel>(async
                (t) =>
            {
                var response = await Client.GetRequestAndWaitResponse(t.EntriesInUseAttributeDescriptor);
                if (response != null && response.GetResponseNormal.Result.DataAccessResult.Value == "00")
                {
                    response.GetResponseNormal.Result.Data.UInt32ValueDisplayFormat =
                        UInt32ValueDisplayFormat.IntValue;
                    t.EntriesInUse.Value = response.GetResponseNormal.Result.Data.ValueString;
                }
            });
            GetProfileEntriesCommand = new AsyncRelayCommand<CustomCosemProfileGenericModel>(async
                (t) =>
            {
                var response = await Client.GetRequestAndWaitResponse(t.ProfileEntriesAttributeDescriptor);
                if (response != null && response.GetResponseNormal.Result.DataAccessResult.Value == "00")
                {
                    t.ProfileEntries.Value = response.GetResponseNormal.Result.Data.ValueString;
                }
            });
            GetSortMethodCommand = new AsyncRelayCommand<CustomCosemProfileGenericModel>(async
                (t) =>
            {
                var response = await Client.GetRequestAndWaitResponse(t.SortMethodAttributeDescriptor);
                if (response != null && response.GetResponseNormal.Result.DataAccessResult.Value == "00")
                    t.SortMethod = (SortMethod) ushort.Parse(response.GetResponseNormal.Result.Data.ValueString);
            });

            GetAllAttrCommand = new RelayCommand<CustomCosemProfileGenericModel>(async(t) =>
              {
                  t.CaptureObjects.Clear();
                  t.CapturePeriod.Value = string.Empty;
                  t.EntriesInUse.Value = string.Empty;
                  t.ProfileEntries.Value = string.Empty;
                  t.SortMethod = SortMethod.None;
                  await GetCaptureObjectsCommand.ExecuteAsync(t);
                  await GetCapturePeriodCommand.ExecuteAsync(t);
                  await GetEntriesInUseCommand.ExecuteAsync(t);
                  await GetProfileEntriesCommand.ExecuteAsync(t);
                  await GetSortMethodCommand.ExecuteAsync(t);
               
              });


            GetAllBufferCommand = new RelayCommand<CustomCosemProfileGenericModel>(async
                (t) =>
            {
                if (t.CaptureObjects == null)
                {
                    return;
                }

                var response =
                    await Client.GetRequestAndWaitResponseArray(t.BufferAttributeDescriptor
                    );

                var structures = ParseBuffer(response);
                if (structures != null )
                {
                
                    
                        DispatcherHelper.CheckBeginInvokeOnUI(() => { 
                            t.Buffer = structures;
                            if (t.Buffer.Count % t.PageSize != 0)
                            {
                                t.TotalPage = (t.Buffer.Count / t.PageSize) + 1;

                            }
                            else
                            {
                                t.TotalPage = t.Buffer.Count / t.PageSize;
                            }
                           t. FirstPageAction();

                        });
                  
                  
                }
                   
            });
           
            GetBufferByEntryCommand = new RelayCommand<CustomCosemProfileGenericModel>(async (t) =>
            {
                t.Buffer.Clear();
                var response =
                    await Client.GetRequestAndWaitResponseArray(t.GetBufferAttributeDescriptorWithSelectionByEntry);
                var structures = ParseBuffer(response);
              
                if (structures != null && t.CaptureObjects != null && t.CaptureObjects.Count != 0)
                {
                    var start = t.ProfileGenericEntryDescriptor.FromSelectedValue;
                    var end = t.ProfileGenericEntryDescriptor.ToSelectedValue;
                    //if (start>=0&&end<=start)
                    //{
                    //    if (start==0&&end==0)
                    //    {
                    //        foreach (var dlmsStructure in structures)
                    //        {
                    //            for (int i = 0; i < dlmsStructure.Items.Length; i++)
                    //            {
                    //                dlmsStructure.Items[i].ValueName = t.CaptureObjects[i].Description;
                    //            }
                    //        }
                    //    }
                    //    if (start==0)
                    //    {

                    //    }
                    //}
                    foreach (var dlmsStructure in structures)
                    {
                        for (int i = 0 ; i < dlmsStructure.Items.Length; i++)
                        {
                            dlmsStructure.Items[i].ValueName = t.CaptureObjects[i].Description;
                        }
                    }
      
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() => { t.Buffer = structures;
                           
                            if (t.Buffer.Count%t.PageSize!=0)
                            {
                                t.TotalPage = (t.Buffer.Count / t.PageSize)+1;

                            }
                            else
                            {
                                t.TotalPage = t.Buffer.Count / t.PageSize;
                            }
                            t.FirstPageAction();
                        });
                    }
                }

              
            });

            GetBufferByRangeCommand = new RelayCommand<CustomCosemProfileGenericModel>(async (t) =>
            {
                t.Buffer.Clear();
//                t.ProfileGenericRangeDescriptor = new ProfileGenericRangeDescriptor()
//                {
//                    RestrictingObject = new CaptureObjectDefinition()
//                        {AttributeIndex = 2, ClassId = 8, DataIndex = 0, LogicalName = "0.0.1.0.0.255"},
//                    FromValue = new DlmsDataItem(DataType.OctetString,
//                        new CosemClock(t.FromDateTime).GetDateTimeBytes()
//                            .ByteToString()),
//                    ToValue = new DlmsDataItem(DataType.OctetString,
//                        new CosemClock(t.ToDateTime).GetDateTimeBytes().ByteToString()),
//                    SelectedValues = new List<CaptureObjectDefinition>()
//                };
                t.ProfileGenericRangeDescriptor.FromValue = new DlmsDataItem(DataType.OctetString,
                    new CosemClock(t.FromDateTime).GetDateTimeBytes()
                        .ByteToString());
                t.ProfileGenericRangeDescriptor.ToValue = new DlmsDataItem(DataType.OctetString,
                    new CosemClock(t.ToDateTime).GetDateTimeBytes()
                        .ByteToString());
                t.ProfileGenericRangeDescriptor.SelectedValues = new List<CaptureObjectDefinition>();


                var response =
                    await Client.GetRequestAndWaitResponseArray(t.GetBufferAttributeDescriptorWithSelectionByRange);

                var structures = ParseBuffer(response);
                if (structures != null && t.CaptureObjects != null && t.CaptureObjects.Count != 0)
                {
                    foreach (var dlmsStructure in structures)
                    {
                        for (int i = 0; i < dlmsStructure.Items.Length; i++)
                        {
                            dlmsStructure.Items[i].ValueName = t.CaptureObjects[i].Description;
                        }
                    }
                  //  if (structures.Count<200)
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() => { t.Buffer = structures;
                            if (t.Buffer.Count % t.PageSize != 0)
                            {
                                t.TotalPage = (t.Buffer.Count / t.PageSize) + 1;

                            }
                            else
                            {
                                t.TotalPage = t.Buffer.Count / t.PageSize;
                            }
                            t.FirstPageAction();
                        });
                    }
                   
                }

               
            });
            ClearBufferCommand = new RelayCommand<CustomCosemProfileGenericModel>(t => { t.Buffer.Clear();t.FakeSource.Clear(); });

           
        }


        public static ObservableCollection<DlmsStructure> ParseBuffer(List<GetResponse> responses)
        {
            StringBuilder stringBuilder = new StringBuilder();
            DLMSArray array;
            ObservableCollection<DlmsStructure> structures = new ObservableCollection<DlmsStructure>();
            if (responses != null && responses.Count != 0)
            {
                DlmsDataItem vDataItem = new DlmsDataItem();
                string strr="";
                if (responses.Count == 1)
                {
                    if (responses[0].GetResponseNormal.Result.IsSuccessed())
                    {
                        strr = responses[0].GetResponseNormal.Result.Data.ToPduStringInHex();
                    }
                   
                }
                else
                {
                    //返回的是多个响应则进行拼接数据
                    foreach (var getResponse in responses)
                    {
                        stringBuilder.Append(getResponse.GetResponseWithDataBlock.DataBlockG.RawData.Value);
                    }

                    strr = stringBuilder.ToString();
                }

                //接着对字符串进行解析
                if (!vDataItem.PduStringInHexConstructor(ref strr))
                {
                    return null;
                }

                if (vDataItem.DataType == DataType.Array)
                {
                    array = (DLMSArray) vDataItem.Value;
                    foreach (var item in array.Items)
                    {
                        structures.Add((DlmsStructure) item.Value);
                    }

                    //将每个捕获对象的描述性文字赋值给ValueName用于界面展示
                }

                return structures;
            }

            return structures;
        }

        public string BaseUriString { get; set; } = $"{Properties.Settings.Default.WebApiUrl}/Meter/EnergyData/";
        public string MeterId { get; set; } = "000000003686";
        //public RestClient RestClient { get; set; } = new RestClient();
        //public RestRequest RestRequest { get; set; } = new RestRequest(Method.POST);
        public List<Energy> Energy { get; set; }
        
        public void InsertData()
        {

            //var tcpServerViewModel = SimpleIoc.Default.GetInstance<TcpServerViewModel>();
            //var t = tcpServerViewModel.MeterIdMatchSockets.FirstOrDefault(i =>
            //    i.IpString == Client.CurrentSocket.RemoteEndPoint.ToString());
            //if (t == null)
            //{
            //    NetLogViewModel.MyServerNetLogModel.Log = "未找到相应表号,不调用API写数据库";
            //    return;
            //}
            
            if (Energy.Count == 0)
            {
                NetLogViewModel.MyServerNetLogModel.Log = "电能数据返回个数为0,不调用API写数据库";
                return;
            }
            RestClient RestClient = new RestClient();

            RestClient.BaseUrl = new Uri($"{BaseUriString}{MeterId}");
            RestRequest RestRequest  = new RestRequest(Method.POST);
            RestRequest.AddHeader("Content-Type", "application/json");
            var str = JsonConvert.SerializeObject(Energy, Formatting.Indented);
            NetLogViewModel.MyServerNetLogModel.Log = str;
            RestRequest.AddParameter("CurrentEnergy", str, ParameterType.RequestBody);
            IRestResponse restResponse = RestClient.Execute(RestRequest);
            NetLogViewModel.MyServerNetLogModel.Log = "插入数据库" + (restResponse.IsSuccessful ? "成功" : "失败");
        }

    
    }
}