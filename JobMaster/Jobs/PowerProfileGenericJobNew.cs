using DotNetty.Transport.Channels;
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
    public class PowerProfileGenericJobNew : ProfileGenericJobBaseNew, IJobWebApi<Power>
    {
        public virtual string BaseUriString { get; set; } = $"http://localhost:5000/api/Meter/PowerData/";

        public RestClient RestClient { get; set; } = new RestClient();
        public RestRequest RestRequest { get; set; } = new RestRequest(Method.POST);

        public PowerProfileGenericJobNew(NetLoggerViewModel netLoggerViewModel, MainServerViewModel mainServerViewModel,
             DlmsSettingsViewModel dlmsSettingsViewModel,IProtocol protocol) : base(netLoggerViewModel,protocol, dlmsSettingsViewModel)
        {
            JobName = "15分钟功率负荷曲线任务";
            Period = 15;
            var from = DateTime.Now.Subtract(new TimeSpan(0, 0, Period, 0, 0));
            var to = DateTime.Now;
            netLoggerViewModel.LogFront($"任务名称:{JobName}\r\n间隔{Period}min\r\n起始:{from}\r\n结束:{to}\r\n");

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
            NetLogViewModel.LogDebug("In PowerTask Execute");
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
                            InsertData(tmp.MeterId, Powers);
                        });
                    }
                    catch (Exception e)
                    {
                        NetLogViewModel.LogError(e.Message);
                    }
                });
            }
        }


        public void InsertData(string meterId, List<Power> powers)
        {
            if (powers.Count == 0)
            {
                NetLogViewModel.LogWarn(meterId + "功率数据返回个数为0,不调用API写数据库");
                return;
            }

            RestClient.BaseUrl = new Uri(BaseUriString + meterId);
            RestRequest.AddHeader("Content-Type", "application/json");
            var str = JsonConvert.SerializeObject(powers, Formatting.Indented);
            NetLogViewModel.LogInfo(str);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in powers)
            {
                stringBuilder.Append(item.DateTime + "\r\n");
            }

            NetLogViewModel.LogInfo(stringBuilder.ToString());
            RestRequest.AddParameter("CurrentPower", str, ParameterType.RequestBody);
            IRestResponse restResponse = RestClient.Execute(RestRequest);
            NetLogViewModel.LogInfo("插入数据库" + (restResponse.IsSuccessful ? "成功" : "失败"));
        }
    }
}