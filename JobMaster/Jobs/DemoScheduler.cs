using Quartz;
using Quartz.Impl;
using System.Threading.Tasks;

namespace JobMaster.Jobs
{
    public static class DemoScheduler
    {
        public static async Task<IScheduler> CreatClearBuffer(bool start = true)
        {
            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            //清空分钟曲线任务，通过设置捕获对象进行清空

            IJobDetail ClearEnergyJobDetail = JobBuilder.Create<ClearEnergyProfileGenericBufferJob>().WithIdentity("ClearEnergy", "ClearEnergyProfileGenericJob").WithDescription("1分钟电量曲线清空任务").Build();
            ITrigger ClearEnergyTrigger = TriggerBuilder.Create()
              .WithCronSchedule("10 0/1 * * * ? *", x => x.WithMisfireHandlingInstructionDoNothing())
              .WithIdentity("EnergyProfileGenericJobTrigger", "ClearEnergyProfileGenericJob")
              .WithDescription("分钟曲线每隔1分钟进行一次清空")
              .Build();

            IJobDetail ClearPowerJobDetail = JobBuilder.Create<ClearPowerProfileGenericBufferJob>().WithIdentity("ClearPower", "ClearPowerProfileGenericJob").WithDescription("15分钟功率曲线清空任务").Build();
            ITrigger ClearPowerTrigger = TriggerBuilder.Create()
              .WithCronSchedule("20 0/1 * * * ? *", x => x.WithMisfireHandlingInstructionDoNothing())
              .WithIdentity("EnergyProfileGenericJobTrigger", "ClearPowerProfileGenericJob")
              .WithDescription("功率曲线每隔1分钟进行一次清空")
              .Build();
            IJobDetail ClearDayJobDetail = JobBuilder.Create<ClearDayProfileGenericBufferJob>().WithIdentity("ClearDay", "ClearDayProfileGenericJob").WithDescription("日冻结电量曲线清空任务").Build();
            ITrigger ClearDayTrigger = TriggerBuilder.Create()
              .WithCronSchedule("40 0/1 * * * ? *", x => x.WithMisfireHandlingInstructionDoNothing())
              .WithIdentity("DayProfileGenericJobTrigger", "ClearDayProfileGenericJob")
              .WithDescription("日曲线每隔1分钟进行一次清空")
              .Build();

            await scheduler.ScheduleJob(ClearEnergyJobDetail, ClearEnergyTrigger);
            await scheduler.ScheduleJob(ClearPowerJobDetail, ClearPowerTrigger);
            await scheduler.ScheduleJob(ClearDayJobDetail, ClearDayTrigger);
            if (start)
                await scheduler.Start();
            return scheduler;
        }
        public static async Task<IScheduler> CreateTest(bool start = true)
        {
            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();


            //IJobDetail energyJobDetail = JobBuilder.Create<EnergyProfileGenericJob>()
            //    .WithIdentity("Energy", "EnergyProfileGenericJob").WithDescription("分钟电量曲线任务").Build();

            //ITrigger energyTrigger = TriggerBuilder.Create()
            //    .WithCronSchedule("10 0/1 * * * ? *", x => x.WithMisfireHandlingInstructionDoNothing())
            //    .WithIdentity("EnergyProfileGenericJobTrigger", "EnergyProfileGenericJob").WithDescription("分钟曲线每隔1分钟进行一次采集")
            //    .Build();


            IJobDetail energyJobDetail = JobBuilder.Create<EnergyProfileGenericJobNew>()
                .WithIdentity("Energy", "EnergyProfileGenericJob").WithDescription("分钟电量曲线任务").Build();

            ITrigger energyTrigger = TriggerBuilder.Create()
                .WithCronSchedule("10 0/1 * * * ? *", x => x.WithMisfireHandlingInstructionDoNothing())
                .WithIdentity("EnergyProfileGenericJobTrigger", "EnergyProfileGenericJob")
                .WithDescription("分钟曲线每隔1分钟进行一次采集")
                .Build();

            //功率
            IJobDetail powerJobDetail = JobBuilder.Create<PowerProfileGenericJobNew>()
                .WithIdentity("Power", "PowerProfileGenericJob").Build();

            ITrigger powerTrigger = TriggerBuilder.Create()
                .WithIdentity("PowerProfileGenericJobTrigger", "PowerProfileGenericJob")
                .WithCronSchedule("25 0/1 * * * ? *", x => x.WithMisfireHandlingInstructionDoNothing())
                .Build();

            //日
            IJobDetail dayJobDetail = JobBuilder.Create<DayProfileGenericJobNew>()
                .WithIdentity("Day", "DayProfileGenericJob").StoreDurably().Build();
            ITrigger dayTrigger = TriggerBuilder.Create()
                .WithIdentity("DayProfileGenericJobTrigger", "DayProfileGenericJob")
                .WithCronSchedule("40 0/1 * * * ? *", x => x.WithMisfireHandlingInstructionDoNothing())
                .Build();
            

            //测试任务
            //IJobDetail testJobDetail = JobBuilder.Create<TestJob>().WithIdentity("TestJob").Build();
            //ITrigger testTrigger = TriggerBuilder.Create().WithIdentity("TestJobTrigger").WithSimpleSchedule(builder =>
            //{
            //    builder.WithIntervalInSeconds(10).RepeatForever();
            //}).Build();


            // await scheduler.Standby();
            //await scheduler.ScheduleJob(testJobDetail, testTrigger);
            //await scheduler.PauseTrigger(testTrigger.Key);

            await scheduler.ScheduleJob(energyJobDetail, energyTrigger);
            await scheduler.ScheduleJob(powerJobDetail, powerTrigger);
            await scheduler.ScheduleJob(dayJobDetail, dayTrigger);
            if (start)
                await scheduler.Start();
            return scheduler;
        }

        public static async Task<IScheduler> CreateNormal(bool start = true)
        {
            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            IJobDetail energyJobDetail = JobBuilder.Create<EnergyProfileGenericJobNew>()
                .WithIdentity("Energy", "ProfileGenericJob").WithDescription("分钟电量曲线任务").Build();

            ITrigger energyTrigger = TriggerBuilder.Create()
                .WithCronSchedule("10 0/5 * * * ? *", x => x.WithMisfireHandlingInstructionDoNothing())
                .WithIdentity("EnergyProfileGenericJobTrigger", "ProfileGenericJob").WithDescription("分钟曲线每隔5分钟进行一次采集")
                .Build();

            //功率
            IJobDetail powerJobDetail = JobBuilder.Create<PowerProfileGenericJobNew>()
                .WithIdentity("Power", "PowerProfileGenericJob").Build();

            ITrigger powerTrigger = TriggerBuilder.Create()
                .WithIdentity("PowerProfileGenericJobTrigger", "PowerProfileGenericJob")
                .WithCronSchedule("40 5/15 * * * ? *", x => x.WithMisfireHandlingInstructionDoNothing())
                .Build();

            //日
            IJobDetail dayJobDetail = JobBuilder.Create<DayProfileGenericJobNew>()
                .WithIdentity("Day", "DayProfileGenericJob").StoreDurably().Build();
            ITrigger dayTrigger = TriggerBuilder.Create()
                .WithIdentity("DayProfileGenericJobTrigger", "DayProfileGenericJob")
                .WithCronSchedule("0 2 0 * * ? *", x => x.WithMisfireHandlingInstructionDoNothing())
                .Build();
            await scheduler.ScheduleJob(energyJobDetail, energyTrigger);
            await scheduler.ScheduleJob(powerJobDetail, powerTrigger);
            await scheduler.ScheduleJob(dayJobDetail, dayTrigger);
            if (start)
                await scheduler.Start();
            return scheduler;
        }
    }
}