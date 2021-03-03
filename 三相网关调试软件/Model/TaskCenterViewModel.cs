using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ObservableCollection<ProfileGenericJobBase> list = new ObservableCollection<ProfileGenericJobBase>();

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
            // scheduler.Start();
        }


     


        private async void LoadingDefaultJob()
        {
            list = new ObservableCollection<ProfileGenericJobBase>()
            {
                new EnergyProfileGenericJob(),
                new PowerProfileGenericJob(),
                new DayProfileGenericJob(),
                new MonthProfileGenericJob()
            };
            //电能
            IJobDetail energyJobDetail = JobBuilder.Create<EnergyProfileGenericJob>()
                .WithIdentity("EnergyProfileGenericJob", "EnergyProfileGenericJob").Build();
            ITrigger energyTrigger = TriggerBuilder.Create().WithCronSchedule("10 0/5 * * * ? *")
                .WithIdentity("EnergyProfileGenericJobTrigger", "EnergyProfileGenericJob").Build();
            //功率
            IJobDetail powerJobDetail = JobBuilder.Create<PowerProfileGenericJob>()
                .WithIdentity("PowerProfileGenericJob", "PowerProfileGenericJob").Build();

            ITrigger powerTrigger = TriggerBuilder.Create()
                .WithIdentity("PowerProfileGenericJobTrigger", "PowerProfileGenericJob")
                .WithCronSchedule("40 5/15 * * * ? *")
                .Build();

            //日
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
        }

        public void Shutdown()
        {
            scheduler?.Shutdown();
          
        }


    }
}