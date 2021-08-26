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
    /// <summary>
    /// 15分钟功率曲线任务
    /// </summary>
    public class PowerProfileGenericJob : ProfileGenericJobBase
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

        public override async Task Execute(IJobExecutionContext context)
        {
            await base.Execute(context);
            var tcpServerViewModel = SimpleIoc.Default.GetInstance<TcpServerViewModel>();

            var t = tcpServerViewModel.MeterIdMatchSockets.FirstOrDefault(i =>
                i.IpString == Client.CurrentSocket.RemoteEndPoint.ToString());
            await Task.Run(() =>
            {
                var client =
                    new RestClient($"{Properties.Settings.Default.WebApiUrl}/Meter/PowerData/{t.MeterId}");
                var request = new RestRequest(Method.POST);

                request.AddHeader("Content-Type", "application/json");

                if (base.CaptureObjects != null)
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

                Powers = new List<Power>();
                StringBuilder stringBuilder = new StringBuilder();

                var responses = base.Responses;
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
                                    MeterId = t.MeterId,
                                    DateTime = clock.ToDateTime()
                                });
                            }
                        }
                    }
                }

                var str = JsonConvert.SerializeObject(Powers);
                request.AddParameter("CurrentPower", str, ParameterType.RequestBody);
                IRestResponse restResponse = client.Execute(request);
                var netLogViewModel = SimpleIoc.Default.GetInstance<NetLogViewModel>();
                netLogViewModel.MyServerNetLogModel.Log = "插入数据库" + (restResponse.IsSuccessful ? "成功" : "失败");
            });
        }
    }
}