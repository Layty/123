using System.Collections.Generic;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quartz;
using Quartz.Impl;

namespace 三相智慧能源网关调试软件.Model
{
    /// <summary>
    /// 任务中心，职责进行增删改任务功能
    /// </summary>
    public class JobCenter : ObservableObject
    {
        private List<ProfileGenericJobBase> list = new List<ProfileGenericJobBase>();

        public void AddJob(ProfileGenericJobBase jobBase)
        {
            list.Add(jobBase);
        }

        public void RemoveJob(ProfileGenericJobBase jobBase)
        {
            list.Remove(jobBase);
        }

        public void UpDate()
        {
        }

        //调度器
        IScheduler scheduler;

        //调度器工厂
        ISchedulerFactory factory;

        public JobCenter()
        {
            factory = new StdSchedulerFactory();
            //创建任务调度器
            scheduler = factory.GetScheduler().Result;
            scheduler.Start();
        }

        public int IntervalInMinutes { get; set; } = 5;
        public bool IsRepeatForever { get; set; } = true;
        public int RepeatCount { get; set; } = 1;


        public bool IsJobStart
        {
            get => _IsJobStart;
            set
            {
                _IsJobStart = value;
                OnPropertyChanged();
            }
        }

        private bool _IsJobStart;


        private async void LoadingDefaultJob()
        {
            list = new List<ProfileGenericJobBase>()
            {
                new EnergyProfileGenericJob(),
                new PowerProfileGenericJob(),
                new DayProfileGenericJob(),
                new MonthProfileGenericJob()
            };
            IJobDetail energyJobDetail = JobBuilder.Create<EnergyProfileGenericJob>()
                .WithIdentity("EnergyProfileGenericJob", "EnergyProfileGenericJob").Build();
            ITrigger energyTrigger = TriggerBuilder.Create().WithCronSchedule("10 0/5 * * * ? *")
                .WithIdentity("EnergyProfileGenericJobTrigger", "EnergyProfileGenericJob").Build();

            IJobDetail powerJobDetail = JobBuilder.Create<PowerProfileGenericJob>()
                .WithIdentity("PowerProfileGenericJob", "PowerProfileGenericJob").Build();

            ITrigger powerTrigger = TriggerBuilder.Create()
                .WithIdentity("PowerProfileGenericJobTrigger", "PowerProfileGenericJob")
                .WithCronSchedule("40 5/15 * * * ? *")
                .Build();
            IJobDetail dayJobDetail = JobBuilder.Create<DayProfileGenericJob>()
                .WithIdentity("DayProfileGenericJob", "DayProfileGenericJob").Build();
            ITrigger dayTrigger = TriggerBuilder.Create()
                .WithIdentity("DayProfileGenericJobTrigger", "DayProfileGenericJob")
                .WithCronSchedule("0 2 0 * * ? *")
                .Build();
            await scheduler.ScheduleJob(energyJobDetail, energyTrigger);
            await scheduler.ScheduleJob(powerJobDetail, powerTrigger);
            await scheduler.ScheduleJob(dayJobDetail, dayTrigger);
        }


        public void Start()
        {
            LoadingDefaultJob();
            scheduler.Start();
            IsJobStart = true;
        }

        public void Shutdown()
        {
            scheduler?.Shutdown();
            IsJobStart = false;
        }

        /*
由7段构成：秒 分 时 日 月 星期 年（可选）

"-" ：表示范围  MON-WED表示星期一到星期三
"," ：表示列举 MON,WEB表示星期一和星期三
"*" ：表是“每”，每月，每天，每周，每年等
"/" :表示增量：0/15（处于分钟段里面） 每15分钟，在0分以后开始，3/20 每20分钟，从3分钟以后开始
"?" :只能出现在日，星期段里面，表示不指定具体的值
"L" :只能出现在日，星期段里面，是Last的缩写，一个月的最后一天，一个星期的最后一天（星期六）
"W" :表示工作日，距离给定值最近的工作日
"#" :表示一个月的第几个星期几，例如："6#3"表示每个月的第三个星期五（1=SUN...6=FRI,7=SAT）

如果Minutes的数值是 '0/15' ，表示从0开始每15分钟执行

如果Minutes的数值是 '3/20' ，表示从3开始每20分钟执行，也就是‘3/23/43’
*/
    }
}