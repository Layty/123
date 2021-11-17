using JobMaster.Models;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.CosemObjects;
using MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage;
using MyDlmsStandard.ApplicationLay.CosemObjects.ProfileGeneric;
using Newtonsoft.Json;
using Quartz;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobMaster.ViewModels;
using MyDlmsStandard;

namespace JobMaster.Jobs
{
    /// <summary>
    /// 15分钟功率曲线任务
    /// </summary>
    public class PowerProfileGenericJob : ProfileGenericJobBase, IJobWebApi<Power>
    {
        public PowerProfileGenericJob(NetLoggerViewModel netLoggerViewModel, DlmsClient dlmsClient) : base(netLoggerViewModel, dlmsClient)
        {
            Period = 15;
            JobName = "15分钟功率负荷曲线任务";
            CustomCosemProfileGenericModel = new CustomCosemProfileGenericModel("1.0.99.2.0.255")
            {
                ProfileGenericRangeDescriptor = new ProfileGenericRangeDescriptor()
                {
                    RestrictingObject = new CaptureObjectDefinition()
                    { AttributeIndex = 2, ClassId = 8, DataIndex = 0, LogicalName = "0.0.1.0.0.255" },
                    FromValue = new DlmsDataItem(DataType.OctetString,
                        new CosemClock(DateTime.Now.Subtract(new TimeSpan(0, 0, Period, 0, 0))).GetDateTimeBytes()
                            .ByteToString()),
                    ToValue = new DlmsDataItem(DataType.OctetString,
                        new CosemClock(DateTime.Now).GetDateTimeBytes().ByteToString()),
                    SelectedValues = new List<CaptureObjectDefinition>()
                }
            };
        }

        public List<Power> Powers { get; set; }

        public string BaseUriString { get; set; } = $"http://localhost:5000/api/Meter/PowerData/";

        public RestClient RestClient { get; set; } = new RestClient();
        public RestRequest RestRequest { get; set; } = new RestRequest(Method.POST);
        public override async Task Execute(IJobExecutionContext context)
        {

            if (Client.TcpServerViewModel.TcpServerHelper.SocketClientList.Count == 0)
            {
                return;
            }
            foreach (var socket in Client.TcpServerViewModel.TcpServerHelper.SocketClientList)
            {
                var tempClient = Client;
                tempClient.TcpServerViewModel.CurrentSocketClient = socket;
                var t = Client.TcpServerViewModel.TcpServerHelper.MeterIdMatchSockets.FirstOrDefault(i => i.IpString == socket.RemoteEndPoint.ToString());
                if (t == null)
                {
                    NetLogViewModel.MyServerNetLogModel.Log = $"{socket.RemoteEndPoint}未从上报心跳中识别到相应的表号";
                    continue;
                }

                //初始化设置读取方式协议为47协议+以太网,后续放出去配置
                tempClient.DlmsSettingsViewModel.ProtocolInterfaceType = ProtocolInterfaceType.WRAPPER;
                tempClient.DlmsSettingsViewModel.PhysicalChanelType = PhysicalChanelType.FrontEndProcess;

                //采集
                await base.Execute(context);
                //解析和存储
                await Task.Run(() =>
                {
                    var CaptureObjects = CaptureObjectsResponsesBindingSocket[socket];
                    //解析捕获对象
                    if (CaptureObjects == null) return;
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

                    var Responses = DataBufferResponsesBindingSocket[socket];
                    if (Responses == null) return;

                    Powers = new List<Power>();
                    var dlmsStructures = CosemProfileGeneric.ParseBuffer(Responses);

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
                                    MeterId = t.MeterId

                                });
                            }
                        }
                    }
                    else { return; }

                    InsertData(t.MeterId);
                });
            }
        }
        public void InsertData(string meterId)
        {
            if (Powers.Count == 0)
            {
                NetLogViewModel.MyServerNetLogModel.Log = "电能数据返回个数为0,不调用API写数据库";
                return;
            }

            RestClient.BaseUrl = new Uri($"{BaseUriString}{meterId}");

            RestRequest.AddHeader("Content-Type", "application/json");
            var str = JsonConvert.SerializeObject(Powers, Formatting.Indented);
            NetLogViewModel.MyServerNetLogModel.Log = str;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in Powers)
            {
                stringBuilder.Append(item.DateTime + "\r\n");
            }
            NetLogViewModel.MyServerNetLogModel.Log = stringBuilder.ToString();
            RestRequest.AddParameter("CurrentPower", str, ParameterType.RequestBody);
            IRestResponse restResponse = RestClient.Execute(RestRequest);
            NetLogViewModel.MyServerNetLogModel.Log = "插入数据库" + (restResponse.IsSuccessful ? "成功" : "失败");
        }

        public void InsertData(string meterId, List<Power> data)
        {
            throw new NotImplementedException();
        }
    }
}