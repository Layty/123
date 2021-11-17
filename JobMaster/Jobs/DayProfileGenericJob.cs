using JobMaster.Models;
using JobMaster.ViewModels;
using MyDlmsStandard;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.CosemObjects;
using MyDlmsStandard.ApplicationLay.CosemObjects.ProfileGeneric;
using Newtonsoft.Json;
using Quartz;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobMaster.Jobs
{
    /// <summary>
    /// 日冻结跟分钟电量的一样 故使用Energy
    /// </summary>
    /// <summary>
    /// 日冻结电量曲线任务
    /// </summary>
    public class DayProfileGenericJob : ProfileGenericJobBase, IJobWebApi<Day>
    {
        public virtual string BaseUriString { get; set; } = $"http://localhost:5000/api/Meter/DayData/";
     
        public RestClient RestClient { get; set; } = new RestClient();
        public RestRequest RestRequest { get; set; } = new RestRequest(Method.POST);
        public DayProfileGenericJob(NetLoggerViewModel netLoggerViewModel, DlmsClient dlmsClient):base(netLoggerViewModel,dlmsClient)
        {
            Period = 60 * 24;
            JobName = "日冻结曲线任务";
            CustomCosemProfileGenericModel = new CustomCosemProfileGenericModel("1.0.98.1.1.255")
            {
                ProfileGenericRangeDescriptor = new ProfileGenericRangeDescriptor()
                {
                    RestrictingObject = new CaptureObjectDefinition()
                    { AttributeIndex = 2, ClassId = 8, DataIndex = 0, LogicalName = "0.0.1.0.0.255" },
                    FromValue = new DlmsDataItem(DataType.OctetString,
                        new CosemClock(DateTime.Today.Date).GetDateTimeBytes()
                            .ByteToString()),
                    ToValue = new DlmsDataItem(DataType.OctetString,
                        new CosemClock(DateTime.Now.Date.Add(new TimeSpan(0, 23, 59, 59))).GetDateTimeBytes()
                            .ByteToString()),
                    SelectedValues = new List<CaptureObjectDefinition>()
                }
              
        };
            BaseUriString = $"http://localhost:5000/api/Meter/DayData/";

        }


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
                    NetLogViewModel.MyServerNetLogModel.Log = $"{socket.RemoteEndPoint}未收到心跳或者未从上报心跳中识别到相应的表号";
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
                    if (Responses == null)
                    {
                        return;
                    }
                    Days = new List<Day>();
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
                                    MeterId = t.MeterId
                                }) ;
                            }
                        }
                    }
                    else { return; }

                    InsertData(t.MeterId);
                });
            }
        }
        public List<Day> Days { get; set; }
        public void InsertData(string meterId)
        {
            if (Days.Count == 0)
            {
                NetLogViewModel.MyServerNetLogModel.Log = "电能数据返回个数为0,不调用API写数据库";
                return;
            }
            RestClient.BaseUrl = new Uri(BaseUriString + meterId);
            RestRequest.AddHeader("Content-Type", "application/json");
            var str = JsonConvert.SerializeObject(Days, Formatting.Indented);
            NetLogViewModel.MyServerNetLogModel.Log = str;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in Days)
            {
                stringBuilder.Append(item.DateTime + "\r\n");
            }
            NetLogViewModel.MyServerNetLogModel.Log = stringBuilder.ToString();
            RestRequest.AddParameter("CurrentDay", str, ParameterType.RequestBody);
            IRestResponse restResponse = RestClient.Execute(RestRequest);
            NetLogViewModel.MyServerNetLogModel.Log = "插入数据库" + (restResponse.IsSuccessful ? "成功" : "失败") + "\r\n";
        }

        public void InsertData(string meterId, List<Day> data)
        {
            throw new NotImplementedException();
        }
    }
}