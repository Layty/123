using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using GalaSoft.MvvmLight.Ioc;
using MyDlmsStandard;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.CosemObjects;
using MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage;
using MyDlmsStandard.Common;
using Quartz;
using Quartz.Impl;
using 三相智慧能源网关调试软件.ViewModel.DlmsViewModels;

namespace 三相智慧能源网关调试软件.Model
{
    /// <summary>
    /// 曲线任务
    /// </summary>
    public class ProfileGenericJob : IJob
    {
        public DlmsClient Client { get; set; }

        /// <summary>
        /// 采集起始到结束的间隔范围,单位毫秒，默认5分钟即300000秒
        /// </summary>
        public int Period { get; set; } = 900000;

        public List<CustomCosemProfileGenericModel> CosemProfileGenericModels { get; set; }


        public ProfileGenericJob()
        {
            Client = SimpleIoc.Default.GetInstance<DlmsClient>();
            CosemProfileGenericModels = new List<CustomCosemProfileGenericModel>()
            {
                new CustomCosemProfileGenericModel("1.0.99.1.0.255")
                {
                    ProfileGenericRangeDescriptor = new ProfileGenericRangeDescriptor()
                    {
                        RestrictingObject = new CaptureObjectDefinition()
                            {AttributeIndex = 2, ClassId = 8, DataIndex = 0, LogicalName = "0.0.1.0.0.255"},
                        FromValue = new DlmsDataItem(DataType.OctetString,
                            new CosemClock(DateTime.Now.Subtract(new TimeSpan(0, 0, 0, 0, Period))).GetDateTimeBytes()
                                .ByteToString()),
                        ToValue = new DlmsDataItem(DataType.OctetString,
                            new CosemClock(DateTime.Now).GetDateTimeBytes().ByteToString()),
                        SelectedValues = new List<CaptureObjectDefinition>()
                    }
                },
                new CustomCosemProfileGenericModel("1.0.99.2.0.255")
                {
                    ProfileGenericRangeDescriptor = new ProfileGenericRangeDescriptor()
                    {
                        RestrictingObject = new CaptureObjectDefinition()
                            {AttributeIndex = 2, ClassId = 8, DataIndex = 0, LogicalName = "0.0.1.0.0.255"},
                        FromValue = new DlmsDataItem(DataType.OctetString,
                            new CosemClock(DateTime.Now.Subtract(new TimeSpan(0, 0, 0, 0, Period))).GetDateTimeBytes()
                                .ByteToString()),
                        ToValue = new DlmsDataItem(DataType.OctetString,
                            new CosemClock(DateTime.Now).GetDateTimeBytes().ByteToString()),
                        SelectedValues = new List<CaptureObjectDefinition>()
                    }
                }
            };
        }


        public async Task Execute(IJobExecutionContext context)
        {
            foreach (var socket in Client.Socket.SocketClientList)
            {
                foreach (var cosemProfileGenericModel in CosemProfileGenericModels)
                {
                    Client.CurrentSocket = socket;
                    Client.DlmsSettingsViewModel.InterfaceType = InterfaceType.WRAPPER;
                    Client.DlmsSettingsViewModel.CommunicationType = CommunicationType.FrontEndProcess;
                    await Client.InitRequest();
                    await Task.Delay(2000);
                    var response =
                        await Client.GetRequestAndWaitResponseArray(cosemProfileGenericModel
                            .GetBufferAttributeDescriptorWithSelectionByRange());

                    await Task.Delay(2000);
                    await Client.ReleaseRequest(true);
                }
            }
        }
    }

    public class JobCenter
    {
        public JobCenter()
        {
        }

        public IJobDetail CreateJobDetail()
        {
            IJobDetail job = JobBuilder.Create<ProfileGenericJob>().WithIdentity("ProfileGenericJob", "Test").Build();
            return job;
        }

        public ITrigger CreateTrigger()
        {
            ITrigger trigger = TriggerBuilder.Create().WithIdentity("ProfileGenericJobTrigger", "Test")
                .WithSimpleSchedule(x => { x.WithIntervalInMinutes(2).RepeatForever(); })
                .Build();
            return trigger;
        }

     
    }


   
}