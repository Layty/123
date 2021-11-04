using Quartz;
using Quartz.Impl;
using System.Threading.Tasks;
using 三相智慧能源网关调试软件.Model.Jobs;

namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{
    public static class DemoScheduler
    {
        public static async Task<IScheduler> CreateTest(bool start = true)
        {
            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            IJobDetail energyJobDetail = JobBuilder.Create<EnergyProfileGenericJob>()
                .WithIdentity("Energy", "ProfileGenericJob").WithDescription("分钟电量曲线任务").Build();

            ITrigger energyTrigger = TriggerBuilder.Create()
                .WithCronSchedule("10 0/1 * * * ? *", x => x.WithMisfireHandlingInstructionDoNothing())
                .WithIdentity("EnergyProfileGenericJobTrigger", "ProfileGenericJob").WithDescription("分钟曲线每隔1分钟进行一次采集")
                .Build();

            //功率
            IJobDetail powerJobDetail = JobBuilder.Create<PowerProfileGenericJob>()
                .WithIdentity("PowerProfileGenericJob", "PowerProfileGenericJob").Build();

            ITrigger powerTrigger = TriggerBuilder.Create()
                .WithIdentity("PowerProfileGenericJobTrigger", "PowerProfileGenericJob")
                .WithCronSchedule("40 5/15 * * * ? *", x => x.WithMisfireHandlingInstructionDoNothing())
                .Build();

            //日
            IJobDetail dayJobDetail = JobBuilder.Create<DayProfileGenericJob>()
                .WithIdentity("DayProfileGenericJob", "DayProfileGenericJob").StoreDurably().Build();
            ITrigger dayTrigger = TriggerBuilder.Create()
                .WithIdentity("DayProfileGenericJobTrigger", "DayProfileGenericJob")
                .WithCronSchedule("0 2 0 * * ? *", x => x.WithMisfireHandlingInstructionDoNothing())
                .Build();

            //测试任务
            IJobDetail testJobDetail = JobBuilder.Create<TestJob>().WithIdentity("TestJob").Build();
            ITrigger testTrigger = TriggerBuilder.Create().WithIdentity("TestJobTrigger").WithSimpleSchedule((builder =>
            {
                builder.WithIntervalInSeconds(10).RepeatForever();
            })).Build();
            await scheduler.Standby();
            await scheduler.ScheduleJob(testJobDetail, testTrigger);
            await scheduler.PauseTrigger(testTrigger.Key);

            await scheduler.ScheduleJob(energyJobDetail, energyTrigger);
            await scheduler.ScheduleJob(powerJobDetail, powerTrigger);
            await scheduler.ScheduleJob(dayJobDetail, dayTrigger);
            if (start)
                await scheduler.Start();
            return scheduler;
        }

        public static async Task<IScheduler> Create(bool start = true)
        {
            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            IJobDetail energyJobDetail = JobBuilder.Create<EnergyProfileGenericJob>()
                .WithIdentity("Energy", "ProfileGenericJob").WithDescription("分钟电量曲线任务").Build();

            ITrigger energyTrigger = TriggerBuilder.Create()
                .WithCronSchedule("10 0/5 * * * ? *", x => x.WithMisfireHandlingInstructionDoNothing())
                .WithIdentity("EnergyProfileGenericJobTrigger", "ProfileGenericJob").WithDescription("分钟曲线每隔5分钟进行一次采集")
                .Build();

            //功率
            IJobDetail powerJobDetail = JobBuilder.Create<PowerProfileGenericJob>()
                .WithIdentity("PowerProfileGenericJob", "PowerProfileGenericJob").Build();

            ITrigger powerTrigger = TriggerBuilder.Create()
                .WithIdentity("PowerProfileGenericJobTrigger", "PowerProfileGenericJob")
                .WithCronSchedule("40 5/15 * * * ? *", x => x.WithMisfireHandlingInstructionDoNothing())
                .Build();

            //日
            IJobDetail dayJobDetail = JobBuilder.Create<DayProfileGenericJob>()
                .WithIdentity("DayProfileGenericJob", "DayProfileGenericJob").StoreDurably().Build();
            ITrigger dayTrigger = TriggerBuilder.Create()
                .WithIdentity("DayProfileGenericJobTrigger", "DayProfileGenericJob")
                .WithCronSchedule("0 2 0 * * ? *", x => x.WithMisfireHandlingInstructionDoNothing())
                .Build();

            //测试任务
            IJobDetail testJobDetail = JobBuilder.Create<TestJob>().WithIdentity("TestJob").Build();
            ITrigger testTrigger = TriggerBuilder.Create().WithIdentity("TestJobTrigger").WithSimpleSchedule((builder =>
            {
                builder.WithIntervalInSeconds(10).RepeatForever();
            })).Build();
            await scheduler.Standby();
            await scheduler.ScheduleJob(testJobDetail, testTrigger);
            await scheduler.PauseTrigger(testTrigger.Key);

            await scheduler.ScheduleJob(energyJobDetail, energyTrigger);
            await scheduler.ScheduleJob(powerJobDetail, powerTrigger);
            await scheduler.ScheduleJob(dayJobDetail, dayTrigger);
            if (start)
                await scheduler.Start();
            return scheduler;
        }
    }
}