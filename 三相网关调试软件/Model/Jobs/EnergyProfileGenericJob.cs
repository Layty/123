using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
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
    public interface IWebApi
    {
        string BaseUriString { get; set; }
        string MeterId { get; set; }
        RestClient RestClient { get; set; }
        RestRequest RestRequest { get; set; }
        void InsertData();
    }

    /// <summary>
    /// 1分钟电量曲线任务
    /// </summary>
    public class EnergyProfileGenericJob : ProfileGenericJobBase, IWebApi
    {
        public string BaseUriString { get; set; } = $"{Properties.Settings.Default.WebApiUrl}/Meter/EnergyData/";
        public string MeterId { get; set; }
        public RestClient RestClient { get; set; } = new RestClient();
        public RestRequest RestRequest { get; set; }

        public EnergyProfileGenericJob()
        {
            JobName = "1分钟电量曲线任务";
            Period = 5;
            CustomCosemProfileGenericModel = new CustomCosemProfileGenericModel("1.0.99.1.0.255")
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

        public override async Task Execute(IJobExecutionContext context)
        {
            await base.Execute(context);

            //遍历列表
            await Task.Run(() =>
            {
                if (CaptureObjects != null)
                {
                    if (CaptureObjects.GetResponseNormal.Result.Data.DataType == DataType.Array)
                    {
                        var array = new DLMSArray();
                        var ar = CaptureObjects.GetResponseNormal.Result.Data.ToPduStringInHex();
                        if (array.PduStringInHexConstructor(ref ar))
                        {
                        }
                    }
                }

                Energy = new List<Energy>();
                StringBuilder stringBuilder = new StringBuilder();
                var responses = Responses;
                if (responses != null)
                {
                    DLMSArray array = null;

                    if (responses.Count == 1)
                    {
                        if (responses[0].GetResponseNormal.Result.Data.DataType == DataType.Array)
                        {
                            array = (DLMSArray) responses[0].GetResponseNormal.Result.Data.Value;
                        }
                    }
                    else
                    {
                        foreach (var getResponse in responses)
                        {
                            stringBuilder.Append(getResponse.GetResponseWithDataBlock.DataBlockG.RawData.Value);
                        }

                        var stringInHex = stringBuilder.ToString();
                        DlmsDataItem vDataItem = new DlmsDataItem();

                        var foo = vDataItem.PduStringInHexConstructor(ref stringInHex);
                        if (!foo)
                        {
                            return;
                        }

                        if (vDataItem.DataType == DataType.Array)
                        {
                            array = (DLMSArray) vDataItem.Value;
                        }
                    }

                    if (array != null)
                    {
                        foreach (var item in array.Items)
                        {
                            var dataItems = ((DlmsStructure) item.Value).Items;
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
                }


                InsertData();
            });
        }

        public List<Energy> Energy { get; set; }

        public void InsertData()
        {
            var tcpServerViewModel = SimpleIoc.Default.GetInstance<TcpServerViewModel>();
            var t = tcpServerViewModel.MeterIdMatchSockets.FirstOrDefault(i =>
                i.IpString == Client.CurrentSocket.RemoteEndPoint.ToString());
            if (t == null)
            {
                NetLogViewModel.MyServerNetLogModel.Log = "未找到相应表号,不调用API写数据库";
                return;
            }

            if (Energy.Count == 0)
            {
                NetLogViewModel.MyServerNetLogModel.Log = "电能数据返回个数为0,不调用API写数据库";
                return;
            }

            RestClient.BaseUrl = new Uri($"{BaseUriString}{t.MeterId}");
            RestRequest = new RestRequest(Method.POST);
            RestRequest.AddHeader("Content-Type", "application/json");
            var str = JsonConvert.SerializeObject(Energy);
            RestRequest.AddParameter("CurrentEnergy", str, ParameterType.RequestBody);
            IRestResponse restResponse = RestClient.Execute(RestRequest);
            NetLogViewModel.MyServerNetLogModel.Log = "插入数据库" + (restResponse.IsSuccessful ? "成功" : "失败");
        }
    }
}