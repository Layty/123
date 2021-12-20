using JobMaster.Handlers;
using JobMaster.Models;
using JobMaster.Services;
using JobMaster.ViewModels;
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

    public class DayProfileGenericJobNew : ProfileGenericJobBaseNew, IJobWebApi<Day>
    {
        public virtual string BaseUriString { get; set; } = $"http://localhost:5000/api/Meter/DayData/";

        public RestClient RestClient { get; set; } = new RestClient();
        public RestRequest RestRequest { get; set; } = new RestRequest(Method.POST);

        public DayProfileGenericJobNew(NetLoggerViewModel netLoggerViewModel, MainServerViewModel mainServerViewModel,
           IProtocol protocol, DlmsSettingsViewModel dlmsSettingsViewModel) : base(netLoggerViewModel, protocol, dlmsSettingsViewModel)
        {
            JobName = "日冻结曲线任务";
            Period = 60 * 24;
            var from = DateTime.Today.Date;
            var to = DateTime.Now.Date.Add(new TimeSpan(0, 23, 59, 59));
            netLoggerViewModel.LogFront($"任务名称:{JobName}\r\n间隔{Period}min\r\n起始:{from}\r\n结束:{to}\r\n");

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


        private ObservableCollection<MeterIdMatchSocketNew> MeterIdMatchSockets;

        public override async Task Execute(IJobExecutionContext context)
        {
            if (MeterIdMatchSockets.Count == 0) return;
            NetLogViewModel.LogDebug("In DayTask Execute");
            for (int i = 0; i < MeterIdMatchSockets.Count; i++)
            {
                var index = i; //处理闭包

                _ = Task.Run(async () =>
                {
                    var tmp = MeterIdMatchSockets[index];
                    try
                    {
                        await Task.Run(async () =>
                        {
                            ILinkLayer linkLayer = new NettyLinkLayer(tmp.MySocket, NetLogViewModel);
                            var Business = new NettyBusiness(Protocol, linkLayer);
                            NetLogViewModel.LogDebug("正在执行" + JobName);
                            NetLogViewModel.LogDebug("开始执行协商请求");
                            var strIp = tmp.MySocket.Channel.RemoteAddress.ToString();
                            try
                            {
                                await Business.AssociationRequestAsyncNetty();
                                await Task.Delay(2000);
                                if (!AssiactionResponseHandler.Successors[strIp])
                                {
                                    NetLogViewModel.LogWarn("协商请求失败");
                                    return;
                                }
                            }
                            catch (Exception e)
                            {
                                var str = e.Message;
                                return;
                            }

                            NetLogViewModel.LogDebug("正在执行读取曲线捕获对象");
                            await Business.GetRequestAndWaitResponseNetty(CustomCosemProfileGenericModel
                                .CaptureObjectsAttributeDescriptor);
                            await Task.Delay(2000);
                            //解析捕获对象,必须及时解析
                            var CaptureObjects =
                            CaptureObjectsResponseHandler.CaptureObjectsResponsesBindingSocketNew[strIp];
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

                            NetLogViewModel.LogDebug("正在执行读取曲线Buffer");
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
                                else
                                {
                                    return;
                                }
                            }

                            NetLogViewModel.LogDebug("正在执行释放请求");
                            await Business.ReleaseRequestAsyncNetty(true);
                            await Task.Delay(2000);

                            var Release = ReleaseResponseHandler.ReleaseSuccessors[strIp];
                            if (!Release)
                            {
                                NetLogViewModel.LogWarn("释放失败");
                                return;
                            }


                            //提交、存储
                            InsertData(tmp.MeterId, Days);
                        });
                    }
                    catch (Exception e)
                    {
                        NetLogViewModel.LogError(e.Message);
                    }
                });
            }
        }


        public void InsertData(string meterId, List<Day> days)
        {
            if (days.Count == 0)
            {
                NetLogViewModel.LogWarn(meterId + "日电能数据返回个数为0,不调用API写数据库");
                return;
            }

            RestClient.BaseUrl = new Uri(BaseUriString + meterId);
            RestRequest.AddHeader("Content-Type", "application/json");
            var str = JsonConvert.SerializeObject(days, Formatting.Indented);
            NetLogViewModel.LogInfo(str);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in days)
            {
                stringBuilder.Append(item.DateTime + "\r\n");
            }

            NetLogViewModel.LogInfo(stringBuilder.ToString());
            RestRequest.AddParameter("CurrentDay", str, ParameterType.RequestBody);
            IRestResponse restResponse = RestClient.Execute(RestRequest);
            NetLogViewModel.LogInfo("插入数据库" + (restResponse.IsSuccessful ? "成功" : "失败"));
        }
    }
}