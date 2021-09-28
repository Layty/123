using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonServiceLocator;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.CosemObjects;
using MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage;
using MyDlmsStandard.Common;
using Newtonsoft.Json;
using Quartz;
using RestSharp;
using 三相智慧能源网关调试软件.ViewModel;
using 三相智慧能源网关调试软件.ViewModel.DlmsViewModels;

namespace 三相智慧能源网关调试软件.Model.Jobs
{
    /// <summary>
    /// 15分钟功率曲线任务
    /// </summary>
    public class PowerProfileGenericJob : ProfileGenericJobBase, IJobWebApi
    {
        public PowerProfileGenericJob()
        {
            Period = 15;
            JobName = "15分钟功率负荷曲线任务";
            CustomCosemProfileGenericModel = new CustomCosemProfileGenericModel("1.0.99.2.0.255")
            {
                ProfileGenericRangeDescriptor = new ProfileGenericRangeDescriptor()
                {
                    RestrictingObject = new CaptureObjectDefinition()
                        {AttributeIndex = 2, ClassId = 8, DataIndex = 0, LogicalName = "0.0.1.0.0.255"},
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

        public override async Task Execute(IJobExecutionContext context)
        {
            await base.Execute(context);

            await Task.Run(() =>
            {
                if (CaptureObjects != null)
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


                Powers = new List<Power>();
                var ttt = ProfileGenericViewModel.ParseBuffer(Responses);

                if (ttt != null)
                {
                    foreach (var item in ttt)
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
                                DateTime = clock.ToDateTime()
                            });
                        }
                    }
                }


                InsertData();
            });
        }
       

        public string BaseUriString { get; set; } = $"{Properties.Settings.Default.WebApiUrl}/Meter/PowerData/";
        public string MeterId { get; set; }
        public RestClient RestClient { get; set; } = new RestClient();
        public RestRequest RestRequest { get; set; }= new RestRequest(Method.POST);

        public void InsertData()
        {
            var tcpServerViewModel = ServiceLocator.Current.GetInstance<TcpServerViewModel>();
            var t = tcpServerViewModel.MeterIdMatchSockets.FirstOrDefault(i =>
                i.IpString == Client.CurrentSocket.RemoteEndPoint.ToString());
            if (t == null)
            {
                NetLogViewModel.MyServerNetLogModel.Log = "未找到相应表号,不调用API写数据库";
                return;
            }

            if (Powers.Count == 0)
            {
                NetLogViewModel.MyServerNetLogModel.Log = "电能数据返回个数为0,不调用API写数据库";
                return;
            }

            RestClient.BaseUrl = new Uri($"{BaseUriString}{t.MeterId}");
          
            RestRequest.AddHeader("Content-Type", "application/json");
            var str = JsonConvert.SerializeObject(Powers);
            RestRequest.AddParameter("CurrentPower", str, ParameterType.RequestBody);
            IRestResponse restResponse = RestClient.Execute(RestRequest);
            NetLogViewModel.MyServerNetLogModel.Log = "插入数据库" + (restResponse.IsSuccessful ? "成功" : "失败");
        }
    }
}