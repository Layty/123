using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using MyDlmsStandard;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.CosemObjects;
using MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage;
using MyDlmsStandard.Common;
using NLog;
using Quartz;
using 三相智慧能源网关调试软件.ViewModel.DlmsViewModels;

namespace 三相智慧能源网关调试软件.Model
{
    public interface IProfileGenericJob : IJob
    {
        DlmsClient Client { get; set; }

        /// <summary>
        /// 曲线任务的名称
        /// </summary>
        string ProfileGenericJobName { get; set; }


        int Period { get; set; }

        /// <summary>
        /// 曲线对象
        /// </summary>
        CustomCosemProfileGenericModel CustomCosemProfileGenericModel { get; set; }
    }

    public abstract class ProfileGenericJobBase : IProfileGenericJob
    {
        protected ProfileGenericJobBase()
        {
            Client = SimpleIoc.Default.GetInstance<DlmsClient>();
        }

        public DlmsClient Client { get; set; }
        public string ProfileGenericJobName { get; set; }
        public int Period { get; set; }
        public CustomCosemProfileGenericModel CustomCosemProfileGenericModel { get; set; }

        public virtual async Task Execute(IJobExecutionContext context)
        {
            try
            {
                foreach (var socket in Client.Socket.SocketClientList)
                {
                    var so = socket;
                    await Task.Run(async () =>
                    {
                        var tempClient = Client;
                        tempClient.CurrentSocket = so;
                        tempClient.DlmsSettingsViewModel.InterfaceType = InterfaceType.WRAPPER;
                        tempClient.DlmsSettingsViewModel.CommunicationType = CommunicationType.FrontEndProcess;
                        var netLogViewModel = SimpleIoc.Default.GetInstance<NetLogViewModel>();
                        netLogViewModel.MyServerNetLogModel.Log =
                            "正在执行" + ProfileGenericJobName + "\r\n";
                        netLogViewModel.MyServerNetLogModel.Log =
                            "正在执行初始化请求";
                        await tempClient.InitRequest();

                        await Task.Delay(2000);
                        netLogViewModel.MyServerNetLogModel.Log =
                            "正在执行读取曲线捕获对象";
                        await tempClient.GetRequestAndWaitResponse(CustomCosemProfileGenericModel
                            .GetCaptureObjectsAttributeDescriptor());
                        await Task.Delay(2000);
                        netLogViewModel.MyServerNetLogModel.Log =
                            "正在执行读取曲线Buffer";
                        await tempClient.GetRequestAndWaitResponseArray(CustomCosemProfileGenericModel
                            .GetBufferAttributeDescriptorWithSelectionByRange());
                        await Task.Delay(2000);
                        netLogViewModel.MyServerNetLogModel.Log =
                            "正在执行释放请求";
                        await tempClient.ReleaseRequest(true);
                    });
                }
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(ProfileGenericJobName + e.Message);
            }
        }
    }

    /// <summary>
    /// 1分钟电量曲线任务
    /// </summary>
    public class EnergyProfileGenericJob : ProfileGenericJobBase
    {
        public EnergyProfileGenericJob()
        {
            ProfileGenericJobName = "1分钟电量曲线任务";
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
    }

    /// <summary>
    /// 15分钟功率曲线任务
    /// </summary>
    public class PowerProfileGenericJob : ProfileGenericJobBase
    {
        public PowerProfileGenericJob()
        {
            Period = 15;
            ProfileGenericJobName = "15分钟功率负荷曲线任务";

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
    }

    /// <summary>
    /// 日冻结电量曲线任务
    /// </summary>
    public class DayProfileGenericJob : ProfileGenericJobBase
    {
        public DayProfileGenericJob()
        {
            Period = 60 * 24;
            ProfileGenericJobName = "日冻结曲线任务";
            CustomCosemProfileGenericModel = new CustomCosemProfileGenericModel("1.0.98.1.1.255")
            {
                ProfileGenericRangeDescriptor = new ProfileGenericRangeDescriptor()
                {
                    RestrictingObject = new CaptureObjectDefinition()
                        {AttributeIndex = 2, ClassId = 8, DataIndex = 0, LogicalName = "0.0.1.0.0.255"},
                    FromValue = new DlmsDataItem(DataType.OctetString,
                        new CosemClock(DateTime.Today.Date).GetDateTimeBytes()
                            .ByteToString()),
                    ToValue = new DlmsDataItem(DataType.OctetString,
                        new CosemClock(DateTime.Now.Date.Add(new TimeSpan(0, 23, 59, 59))).GetDateTimeBytes()
                            .ByteToString()),
                    SelectedValues = new List<CaptureObjectDefinition>()
                }
            };
        }
    }


    public class MonthProfileGenericJob : ProfileGenericJobBase
    {
        public MonthProfileGenericJob()
        {
            ProfileGenericJobName = "月结算曲线任务";
            Period = 60 * 24 * 31;
            CustomCosemProfileGenericModel = new CustomCosemProfileGenericModel("0.0.98.1.0.255")
            {
            };
        }
    }
}