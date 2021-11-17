using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quartz;
using Quartz.Impl;
using System.Collections.ObjectModel;
using 三相智慧能源网关调试软件.Model.Jobs;

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
        readonly IScheduler scheduler;

        //调度器工厂
        readonly ISchedulerFactory factory;

        public JobCenter()
        {
            factory = new StdSchedulerFactory();
            //创建任务调度器
            scheduler = factory.GetScheduler().Result;
            // scheduler.Start();
        }





   


        


    }
}