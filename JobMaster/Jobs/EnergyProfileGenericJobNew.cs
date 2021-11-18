using DotNetty.Transport.Channels;
using JobMaster.Handlers;
using JobMaster.Models;
using JobMaster.ViewModels;
using MyDlmsStandard;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.CosemObjects;
using MyDlmsStandard.ApplicationLay.CosemObjects.ProfileGeneric;
using MyDlmsStandard.ApplicationLay.Get;
using Newtonsoft.Json;
using Quartz;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace JobMaster.Jobs
{

    public abstract class ProfileGenericJobBaseNew : IProfileGenericJob
    {
        protected ProfileGenericJobBaseNew(NetLoggerViewModel netLoggerViewModel, DlmsClient dlmsClient)
        {
            Client = dlmsClient;
            NetLogViewModel = netLoggerViewModel;
        }
        public CustomCosemProfileGenericModel CustomCosemProfileGenericModel { get; set; }
        public DlmsClient Client { get; set; }
        public string JobName { get; set; }
        public int Period { get; set; }
        public NetLoggerViewModel NetLogViewModel { get; set; }
        public virtual Task Execute(IJobExecutionContext context)
        {
            return Task.CompletedTask;
            //throw new NotImplementedException();
        }
    }

    public class EnergyProfileGenericJobNew : ProfileGenericJobBaseNew, IJobWebApi<Energy>
    {
        public virtual string BaseUriString { get; set; } = $"http://localhost:5000/api/Meter/EnergyData/";

        public RestClient RestClient { get; set; } = new RestClient();
        public RestRequest RestRequest { get; set; } = new RestRequest(Method.POST);

        public EnergyProfileGenericJobNew(NetLoggerViewModel netLoggerViewModel, MainServerViewModel mainServerViewModel, DlmsClient dlmsClient) : base(netLoggerViewModel, dlmsClient)
        {
            JobName = "1分钟电量曲线任务";
            Period = 5;
            var from = DateTime.Now.Subtract(new TimeSpan(0, 0, Period, 0, 0));
            var to = DateTime.Now;
            netLoggerViewModel.MyServerNetLogModel.Log = $"1分钟电量曲线任务 起始:{from} 结束:{to}\r\n";

            CustomCosemProfileGenericModel = new CustomCosemProfileGenericModel("1.0.99.1.0.255")
            {
                ProfileGenericRangeDescriptor = new ProfileGenericRangeDescriptor()
                {
                    RestrictingObject = new CaptureObjectDefinition()
                    { AttributeIndex = 2, ClassId = 8, DataIndex = 0, LogicalName = "0.0.1.0.0.255" },
                    FromValue = new DlmsDataItem(DataType.OctetString,
                        new CosemClock(from).GetDateTimeBytes()
                            .ByteToString()),
                    ToValue = new DlmsDataItem(DataType.OctetString,
                        new CosemClock(to).GetDateTimeBytes().ByteToString()),
                    SelectedValues = new List<CaptureObjectDefinition>()
                }
            };

            MeterIdMatchSockets = mainServerViewModel.MeterIdMatchSockets;
        }
        public Dictionary<IChannelHandlerContext, GetResponse> CaptureObjectsResponsesBindingSocketNew { get; set; } =
            new Dictionary<IChannelHandlerContext, GetResponse>();
        public Dictionary<IChannelHandlerContext, List<GetResponse>> DataBufferResponsesBindingSocketNew { get; set; } =
        new Dictionary<IChannelHandlerContext, List<GetResponse>>();

        public ObservableCollection<MeterIdMatchSocketNew> MeterIdMatchSockets { get; set; }
        public override async Task Execute(IJobExecutionContext context)
        {
            if (MeterIdMatchSockets.Count == 0) return;

            for (int i = 0; i < MeterIdMatchSockets.Count; i++)
            {
                var index = i;//处理闭包

                _ = Task.Run(async () =>
                 {
                     var tmp = MeterIdMatchSockets[index];
                     try
                     {
                         await Task.Run(async () =>
                         {
                             var Business = new NettyBusiness(Client.DlmsSettingsViewModel, tmp.MySocket, NetLogViewModel);
                             NetLogViewModel.MyServerNetLogModel.Log = "正在执行" + JobName + "\r\n";
                             NetLogViewModel.MyServerNetLogModel.Log = "开始执行初始化请求\r\n";
                             var strIp = tmp.MySocket.Channel.RemoteAddress.ToString();
                             try
                             {
                                 await Business.InitRequestAsyncNetty();
                                 await Task.Delay(2000);
                                 if (!AssiactionResponseHandler.Successors[strIp])
                                 {
                                     return;
                                 }
                             }
                             catch (Exception e)
                             {
                                 var str = e.Message;
                                 return;
                             }

                             NetLogViewModel.MyServerNetLogModel.Log = "正在执行读取曲线捕获对象\r\n";
                             await Business.GetRequestAndWaitResponseNetty(CustomCosemProfileGenericModel
                               .CaptureObjectsAttributeDescriptor);
                             await Task.Delay(2000);
                             //解析捕获对象,必须及时解析
                             var CaptureObjects = CaptureObjectsResponseHandler.CaptureObjectsResponsesBindingSocketNew[strIp];
                             if (CaptureObjects == null)
                             {
                                 return;
                             }
                             else
                             {
                                 if (CaptureObjects.GetResponseNormal.Result.Data.DataType == DataType.Array)
                                 {
                                     var CaptureObjectsArray = new DLMSArray();
                                     var ar = CaptureObjects.GetResponseNormal.Result.Data.ToPduStringInHex();
                                     if (!CaptureObjectsArray.PduStringInHexConstructor(ref ar))
                                     {
                                         return;
                                     }
                                 }

                             }

                             NetLogViewModel.MyServerNetLogModel.Log = "正在执行读取曲线Buffer\r\n";
                             var ResponsesBuffer = new List<GetResponse>();
                             await Business.GetRequestAndWaitResponseArrayNetty(CustomCosemProfileGenericModel
                                .GetBufferAttributeDescriptorWithSelectionByRange);
                             await Task.Delay(2000);
                             var Responses = BufferResponseHandler.ResponsesBuffer[strIp];
                             var Energies = new List<Energy>();
                             if (Responses == null)
                             {
                                 return;
                             }
                             else
                             {
                                 var dlmsStructures = CustomCosemProfileGenericModel.ParseBuffer(Responses);
                                 if (dlmsStructures != null)
                                 {

                                     foreach (var item in dlmsStructures)
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

                                             Energies.Add(new Energy()
                                             {
                                                 EnergyData = JsonConvert.SerializeObject(energyCaptureObjects),
                                                 Id = Guid.NewGuid(),
                                                 DateTime = clock.ToDateTime(),
                                                 MeterId = tmp.MeterId
                                             });
                                         }
                                     }
                                 }
                                 else { return; }
                             }

                             NetLogViewModel.MyServerNetLogModel.Log = "正在执行释放请求\r\n";
                             await Business.ReleaseRequestAsyncNetty(true);
                             await Task.Delay(2000);

                             var Release = ReleaseResponseHandler.ReleaseSuccessors[strIp];
                             if (!Release)
                             {
                                 return;
                             }



                             //提交、存储
                             InsertData(tmp.MeterId, Energies);
                         });
                     }
                     catch (Exception e)
                     {
                         NetLogViewModel.MyServerNetLogModel.Log = e.Message;
                     }
                 });
            }

        }



        public void InsertData(string meterId, List<Energy> Energies)
        {
            if (Energies.Count == 0)
            {
                NetLogViewModel.MyServerNetLogModel.Log = meterId + "电能数据返回个数为0,不调用API写数据库";
                return;
            }
            RestClient.BaseUrl = new Uri(BaseUriString + meterId);
            RestRequest.AddHeader("Content-Type", "application/json");
            var str = JsonConvert.SerializeObject(Energies, Formatting.Indented);
            NetLogViewModel.MyServerNetLogModel.Log = str;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in Energies)
            {
                stringBuilder.Append(item.DateTime + "\r\n");
            }
            NetLogViewModel.MyServerNetLogModel.Log = stringBuilder.ToString();
            RestRequest.AddParameter("CurrentEnergy", str, ParameterType.RequestBody);
            IRestResponse restResponse = RestClient.Execute(RestRequest);
            NetLogViewModel.MyServerNetLogModel.Log = "插入数据库" + (restResponse.IsSuccessful ? "成功" : "失败") + "\r\n";
        }


    }

    public class PowerProfileGenericJobNew : ProfileGenericJobBaseNew, IJobWebApi<Power>
    {
        public virtual string BaseUriString { get; set; } = $"http://localhost:5000/api/Meter/PowerData/";

        public RestClient RestClient { get; set; } = new RestClient();
        public RestRequest RestRequest { get; set; } = new RestRequest(Method.POST);

        public PowerProfileGenericJobNew(NetLoggerViewModel netLoggerViewModel, MainServerViewModel mainServerViewModel, DlmsClient dlmsClient) : base(netLoggerViewModel, dlmsClient)
        {
            JobName = "15分钟功率负荷曲线任务";
            Period = 15;
            var from = DateTime.Now.Subtract(new TimeSpan(0, 0, Period, 0, 0));
            var to = DateTime.Now;
            netLoggerViewModel.MyServerNetLogModel.Log = $"15分钟功率负荷曲线任务 起始{from.ToString()}结束{to.ToString()}\r\n";

            CustomCosemProfileGenericModel = new CustomCosemProfileGenericModel("1.0.99.2.0.255")
            {
                ProfileGenericRangeDescriptor = new ProfileGenericRangeDescriptor()
                {
                    RestrictingObject = new CaptureObjectDefinition()
                    { AttributeIndex = 2, ClassId = 8, DataIndex = 0, LogicalName = "0.0.1.0.0.255" },
                    FromValue = new DlmsDataItem(DataType.OctetString,
                     new CosemClock(from).GetDateTimeBytes().ByteToString()),
                    ToValue = new DlmsDataItem(DataType.OctetString,
                     new CosemClock(to).GetDateTimeBytes().ByteToString()),
                    SelectedValues = new List<CaptureObjectDefinition>()
                }
            };

            MeterIdMatchSockets = mainServerViewModel.MeterIdMatchSockets;
        }
        public Dictionary<IChannelHandlerContext, GetResponse> CaptureObjectsResponsesBindingSocketNew { get; set; } =
            new Dictionary<IChannelHandlerContext, GetResponse>();
        public Dictionary<IChannelHandlerContext, List<GetResponse>> DataBufferResponsesBindingSocketNew { get; set; } =
        new Dictionary<IChannelHandlerContext, List<GetResponse>>();

        public ObservableCollection<MeterIdMatchSocketNew> MeterIdMatchSockets { get; set; }
        public override async Task Execute(IJobExecutionContext context)
        {
            if (MeterIdMatchSockets.Count == 0) return;

            for (int i = 0; i < MeterIdMatchSockets.Count; i++)
            {
                var index = i;//处理闭包

                _ = Task.Run(async () =>
                {
                    var tmp = MeterIdMatchSockets[index];
                    try
                    {
                        await Task.Run(async () =>
                        {
                            var Business = new NettyBusiness(Client.DlmsSettingsViewModel, tmp.MySocket, NetLogViewModel);
                            NetLogViewModel.MyServerNetLogModel.Log = "正在执行" + JobName + "\r\n";
                            NetLogViewModel.MyServerNetLogModel.Log = "开始执行初始化请求\r\n";
                            var strIp = tmp.MySocket.Channel.RemoteAddress.ToString();
                            try
                            {
                                await Business.InitRequestAsyncNetty();
                                await Task.Delay(2000);
                                if (!AssiactionResponseHandler.Successors[strIp])
                                {
                                    return;
                                }
                            }
                            catch (Exception e)
                            {
                                var str = e.Message;
                                return;
                            }

                            NetLogViewModel.MyServerNetLogModel.Log = "正在执行读取曲线捕获对象\r\n";
                            await Business.GetRequestAndWaitResponseNetty(CustomCosemProfileGenericModel
                              .CaptureObjectsAttributeDescriptor);
                            await Task.Delay(2000);
                            //解析捕获对象,必须及时解析
                            var CaptureObjects = CaptureObjectsResponseHandler.CaptureObjectsResponsesBindingSocketNew[strIp];
                            if (CaptureObjects == null)
                            {
                                return;
                            }
                            else
                            {
                                if (CaptureObjects.GetResponseNormal.Result.Data.DataType == DataType.Array)
                                {
                                    var CaptureObjectsArray = new DLMSArray();
                                    var ar = CaptureObjects.GetResponseNormal.Result.Data.ToPduStringInHex();
                                    if (!CaptureObjectsArray.PduStringInHexConstructor(ref ar))
                                    {
                                        return;
                                    }
                                }

                            }

                            NetLogViewModel.MyServerNetLogModel.Log = "正在执行读取曲线Buffer\r\n";
                            var ResponsesBuffer = new List<GetResponse>();
                            await Business.GetRequestAndWaitResponseArrayNetty(CustomCosemProfileGenericModel
                               .GetBufferAttributeDescriptorWithSelectionByRange);
                            await Task.Delay(2000);
                            var Responses = BufferResponseHandler.ResponsesBuffer[strIp];
                            var Powers = new List<Power>();
                            if (Responses == null)
                            {
                                return;
                            }
                            else
                            {
                                var dlmsStructures = CustomCosemProfileGenericModel.ParseBuffer(Responses);
                                if (dlmsStructures != null)
                                {

                                    foreach (var item in dlmsStructures)
                                    {
                                        var dataItems = item.Items;
                                        var clock = new CosemClock();
                                        string dt = dataItems[0].Value.ToString();
                                        var b = clock.DlmsClockParse(dt.StringToByte());
                                        if (b)
                                        {
                                            PowerCaptureObjects powerCaptureObjects = new PowerCaptureObjects
                                            {
                                                DateTime = clock.ToDateTime(),
                                                ImportActivePowerTotal = dataItems[1].ValueString,
                                                ExportActivePowerTotal = dataItems[2].ValueString,
                                                A相电压 = dataItems[3].ValueString,
                                                B相电压 = dataItems[4].ValueString,
                                                C相电压 = dataItems[5].ValueString,
                                                A相电流 = dataItems[6].ValueString,
                                                B相电流 = dataItems[7].ValueString,
                                                C相电流 = dataItems[8].ValueString
                                            };

                                            Powers.Add(new Power()
                                            {
                                                PowerData = JsonConvert.SerializeObject(powerCaptureObjects),
                                                Id = Guid.NewGuid(),
                                                DateTime = clock.ToDateTime(),
                                                MeterId = tmp.MeterId
                                            });
                                        }
                                    }
                                }
                                else { return; }
                            }

                            NetLogViewModel.MyServerNetLogModel.Log = "正在执行释放请求\r\n";
                            await Business.ReleaseRequestAsyncNetty(true);
                            await Task.Delay(2000);

                            var Release = ReleaseResponseHandler.ReleaseSuccessors[strIp];
                            if (!Release)
                            {
                                return;
                            }



                            //提交、存储
                            InsertData(tmp.MeterId, Powers);
                        });
                    }
                    catch (Exception e)
                    {
                        NetLogViewModel.MyServerNetLogModel.Log = e.Message;
                    }
                });
            }

        }



        public void InsertData(string meterId, List<Power> powers)
        {
            if (powers.Count == 0)
            {
                NetLogViewModel.MyServerNetLogModel.Log = meterId + "电能数据返回个数为0,不调用API写数据库";
                return;
            }
            RestClient.BaseUrl = new Uri(BaseUriString + meterId);
            RestRequest.AddHeader("Content-Type", "application/json");
            var str = JsonConvert.SerializeObject(powers, Formatting.Indented);
            NetLogViewModel.MyServerNetLogModel.Log = str;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in powers)
            {
                stringBuilder.Append(item.DateTime + "\r\n");
            }
            NetLogViewModel.MyServerNetLogModel.Log = stringBuilder.ToString();
            RestRequest.AddParameter("CurrentPower", str, ParameterType.RequestBody);
            IRestResponse restResponse = RestClient.Execute(RestRequest);
            NetLogViewModel.MyServerNetLogModel.Log = "插入数据库" + (restResponse.IsSuccessful ? "成功" : "失败") + "\r\n";
        }


    }
    public class DayProfileGenericJobNew : ProfileGenericJobBaseNew, IJobWebApi<Day>
    {
        public virtual string BaseUriString { get; set; } = $"http://localhost:5000/api/Meter/DayData/";

        public RestClient RestClient { get; set; } = new RestClient();
        public RestRequest RestRequest { get; set; } = new RestRequest(Method.POST);

        public DayProfileGenericJobNew(NetLoggerViewModel netLoggerViewModel, MainServerViewModel mainServerViewModel, DlmsClient dlmsClient) : base(netLoggerViewModel, dlmsClient)
        {
            JobName = "日冻结曲线任务";
            Period = 60 * 24;
            var from = DateTime.Today.Date;
            var to = DateTime.Now.Date.Add(new TimeSpan(0, 23, 59, 59));
            netLoggerViewModel.MyServerNetLogModel.Log = $"日冻结曲线任务 起始{from.ToString()}结束{to.ToString()}\r\n";

            CustomCosemProfileGenericModel = new CustomCosemProfileGenericModel("1.0.98.1.1.255")
            {
                ProfileGenericRangeDescriptor = new ProfileGenericRangeDescriptor()
                {
                    RestrictingObject = new CaptureObjectDefinition()
                    { AttributeIndex = 2, ClassId = 8, DataIndex = 0, LogicalName = "0.0.1.0.0.255" },
                    FromValue = new DlmsDataItem(DataType.OctetString,
                     new CosemClock(from).GetDateTimeBytes().ByteToString()),
                    ToValue = new DlmsDataItem(DataType.OctetString,
                     new CosemClock(to).GetDateTimeBytes().ByteToString()),
                    SelectedValues = new List<CaptureObjectDefinition>()
                }
            };

            MeterIdMatchSockets = mainServerViewModel.MeterIdMatchSockets;
        }
        public Dictionary<IChannelHandlerContext, GetResponse> CaptureObjectsResponsesBindingSocketNew { get; set; } =
            new Dictionary<IChannelHandlerContext, GetResponse>();
        public Dictionary<IChannelHandlerContext, List<GetResponse>> DataBufferResponsesBindingSocketNew { get; set; } =
        new Dictionary<IChannelHandlerContext, List<GetResponse>>();

        public ObservableCollection<MeterIdMatchSocketNew> MeterIdMatchSockets { get; set; }
        public override async Task Execute(IJobExecutionContext context)
        {
            if (MeterIdMatchSockets.Count == 0) return;

            for (int i = 0; i < MeterIdMatchSockets.Count; i++)
            {
                var index = i;//处理闭包

                _ = Task.Run(async () =>
                {
                    var tmp = MeterIdMatchSockets[index];
                    try
                    {
                        await Task.Run(async () =>
                        {
                            var Business = new NettyBusiness(Client.DlmsSettingsViewModel, tmp.MySocket, NetLogViewModel);
                            NetLogViewModel.MyServerNetLogModel.Log = "正在执行" + JobName + "\r\n";
                            NetLogViewModel.MyServerNetLogModel.Log = "开始执行初始化请求\r\n";
                            var strIp = tmp.MySocket.Channel.RemoteAddress.ToString();
                            try
                            {
                                await Business.InitRequestAsyncNetty();
                                await Task.Delay(2000);
                                if (!AssiactionResponseHandler.Successors[strIp])
                                {
                                    return;
                                }
                            }
                            catch (Exception e)
                            {
                                var str = e.Message;
                                return;
                            }

                            NetLogViewModel.MyServerNetLogModel.Log = "正在执行读取曲线捕获对象\r\n";
                            await Business.GetRequestAndWaitResponseNetty(CustomCosemProfileGenericModel
                              .CaptureObjectsAttributeDescriptor);
                            await Task.Delay(2000);
                            //解析捕获对象,必须及时解析
                            var CaptureObjects = CaptureObjectsResponseHandler.CaptureObjectsResponsesBindingSocketNew[strIp];
                            if (CaptureObjects == null)
                            {
                                return;
                            }
                            else
                            {
                                if (CaptureObjects.GetResponseNormal.Result.Data.DataType == DataType.Array)
                                {
                                    var CaptureObjectsArray = new DLMSArray();
                                    var ar = CaptureObjects.GetResponseNormal.Result.Data.ToPduStringInHex();
                                    if (!CaptureObjectsArray.PduStringInHexConstructor(ref ar))
                                    {
                                        return;
                                    }
                                }

                            }

                            NetLogViewModel.MyServerNetLogModel.Log = "正在执行读取曲线Buffer\r\n";
                            var ResponsesBuffer = new List<GetResponse>();
                            await Business.GetRequestAndWaitResponseArrayNetty(CustomCosemProfileGenericModel
                               .GetBufferAttributeDescriptorWithSelectionByRange);
                            await Task.Delay(2000);
                            var Responses = BufferResponseHandler.ResponsesBuffer[strIp];
                            var Days = new List<Day>();
                            if (Responses == null)
                            {
                                return;
                            }
                            else
                            {
                                var dlmsStructures = CustomCosemProfileGenericModel.ParseBuffer(Responses);
                                if (dlmsStructures != null)
                                {

                                    foreach (var item in dlmsStructures)
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

                                            Days.Add(new Day()
                                            {
                                                DayData = JsonConvert.SerializeObject(energyCaptureObjects),
                                                Id = Guid.NewGuid(),
                                                DateTime = clock.ToDateTime(),
                                                MeterId = tmp.MeterId
                                            });
                                        }
                                    }
                                }
                                else { return; }
                            }

                            NetLogViewModel.MyServerNetLogModel.Log = "正在执行释放请求\r\n";
                            await Business.ReleaseRequestAsyncNetty(true);
                            await Task.Delay(2000);

                            var Release = ReleaseResponseHandler.ReleaseSuccessors[strIp];
                            if (!Release)
                            {
                                return;
                            }



                            //提交、存储
                            InsertData(tmp.MeterId, Days);
                        });
                    }
                    catch (Exception e)
                    {
                        NetLogViewModel.MyServerNetLogModel.Log = e.Message;
                    }
                });
            }

        }



        public void InsertData(string meterId, List<Day> days)
        {
            if (days.Count == 0)
            {
                NetLogViewModel.MyServerNetLogModel.Log = meterId + "电能数据返回个数为0,不调用API写数据库";
                return;
            }
            RestClient.BaseUrl = new Uri(BaseUriString + meterId);
            RestRequest.AddHeader("Content-Type", "application/json");
            var str = JsonConvert.SerializeObject(days, Formatting.Indented);
            NetLogViewModel.MyServerNetLogModel.Log = str;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in days)
            {
                stringBuilder.Append(item.DateTime + "\r\n");
            }
            NetLogViewModel.MyServerNetLogModel.Log = stringBuilder.ToString();
            RestRequest.AddParameter("CurrentDay", str, ParameterType.RequestBody);
            IRestResponse restResponse = RestClient.Execute(RestRequest);
            NetLogViewModel.MyServerNetLogModel.Log = "插入数据库" + (restResponse.IsSuccessful ? "成功" : "失败") + "\r\n";
        }


    }

}