using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.CosemObjects;
using MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage;
using MyDlmsStandard.ApplicationLay.Get;
using MyDlmsStandard.Common;
using Newtonsoft.Json;
using Quartz;
using RestSharp;
using 三相智慧能源网关调试软件.ViewModel;
using 三相智慧能源网关调试软件.ViewModel.DlmsViewModels;

namespace 三相智慧能源网关调试软件.Model.Jobs
{
    /// <summary>
    /// 1分钟电量曲线任务
    /// </summary>
    public class EnergyProfileGenericJob : ProfileGenericJobBase
    {
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
            var tcpServerViewModel = SimpleIoc.Default.GetInstance<TcpServerViewModel>();
            var ttttt1 = Client.CurrentSocket.RemoteEndPoint.ToString();
            var t = tcpServerViewModel.ListBoxExtend.FirstOrDefault(i =>
                    i.IpString == Client.CurrentSocket.RemoteEndPoint.ToString())
                ;

            await Task.Run(() =>
            {
                var client =
                    new RestClient($"{Properties.Settings.Default.WebApiUrl}/Meter/CreateEnergyData{t.MeterAddress}");
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
//                            EnergyCaptureObjects energyCaptureObjects = new EnergyCaptureObjects();
//                            var clock = new CosemClock();
//
//                            if (clock.DlmsClockParse(array.Items[0].Value.ToString().StringToByte()))
//                            {
//                                energyCaptureObjects.DateTime = clock.ToDateTime();
//                            }
//
//                            energyCaptureObjects.ImportActiveEnergyTotal = array.Items[1].ValueString;
//                            energyCaptureObjects.ImportActiveEnergyT1 = array.Items[2].ValueString;
//                            energyCaptureObjects.ImportActiveEnergyT2 = array.Items[3].ValueString;
//                            energyCaptureObjects.ImportActiveEnergyT3 = array.Items[4].ValueString;
//                            energyCaptureObjects.ImportActiveEnergyT4 = array.Items[5].ValueString;
//                            energyCaptureObjects.ExportActiveEnergyTotal = array.Items[6].ValueString;
//                            energyCaptureObjects.ImportReactiveEnergyTotal = array.Items[7].ValueString;
//                            energyCaptureObjects.ExportReactiveEnergyTotal = array.Items[8].ValueString;
                            // JsonConvert.SerializeObject(Energy);
                        }
                    }
                }

                Energy = new List<Energy>();
                StringBuilder stringBuilder = new StringBuilder();

                var response = base.Responses;
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
                                var tmp = ((DlmsStructure) item.Value).Items;
                                var clock = new CosemClock();
                                string str33 = tmp[0].Value.ToString();
                                var b = clock.DlmsClockParse(str33.StringToByte());
                                if (b)
                                {
                                    clock.ToDateTime();
                                }

                                EnergyCaptureObjects energyCaptureObjects = new EnergyCaptureObjects();

                                energyCaptureObjects.DateTime = clock.ToDateTime();
                                energyCaptureObjects.ImportActiveEnergyTotal = tmp[1].ValueString;
                                energyCaptureObjects.ImportActiveEnergyT1 = tmp[2].ValueString;
                                energyCaptureObjects.ImportActiveEnergyT2 = tmp[3].ValueString;
                                energyCaptureObjects.ImportActiveEnergyT3 = tmp[4].ValueString;
                                energyCaptureObjects.ImportActiveEnergyT4 = tmp[5].ValueString;
                                energyCaptureObjects.ExportActiveEnergyTotal = tmp[6].ValueString;
                                energyCaptureObjects.ImportReactiveEnergyTotal = tmp[7].ValueString;
                                energyCaptureObjects.ExportReactiveEnergyTotal = tmp[8].ValueString;
                                Energy.Add(new Energy()
                                {
                                    EnergyData = JsonConvert.SerializeObject(energyCaptureObjects), Id = Guid.NewGuid(),
                                    MeterId = t.MeterAddress, DateTime = clock.ToDateTime()
                                });
                            }

                            //    DispatcherHelper.CheckBeginInvokeOnUI(() => { t.Buffer = structures; });
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
                                Energy.Add(new Energy()
                                {
                                    EnergyData = JsonConvert.SerializeObject(((DlmsStructure) item.Value).Items),
                                    Id = Guid.NewGuid(), MeterId = t.MeterAddress,
                                    DateTime = DateTime.Now
                                });
                            }

                            //  DispatcherHelper.CheckBeginInvokeOnUI(() => { t.Buffer = structures; });
                        }
                    }
                }

                var str = JsonConvert.SerializeObject(Energy);
                request.AddParameter("CurrentEnergy", str, ParameterType.RequestBody);
                IRestResponse restResponse = client.Execute(request);
                var netLogViewModel = SimpleIoc.Default.GetInstance<NetLogViewModel>();
                netLogViewModel.MyServerNetLogModel.Log = restResponse.IsSuccessful ? "成功" : "失败";
            });
        }
    }
}