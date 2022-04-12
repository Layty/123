using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JobMaster.Helpers;
using JobMaster.Jobs;
using Prism.Commands;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace JobMaster.ViewModels
{
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
    public partial class JobCenterViewModel : ObservableObject, ITriggerListener, IJobListener
    {

        [ObservableProperty]
        private bool _isSchedulerStarted;

        [ObservableProperty]
        private IScheduler _scheduler;


        [ObservableProperty]
        private IReadOnlyCollection<IJobExecutionContext> _jobExecutionContexts;


        [ObservableProperty]
        private IReadOnlyCollection<string> _nameList;

        private readonly IServiceProvider serviceProvider;
        private NetLoggerViewModel netLogViewModel;


        public JobCenterViewModel(IServiceProvider serviceProvider, NetLoggerViewModel netLoggerViewModel)
        {
            this.serviceProvider = serviceProvider;
            JobsViewModels = new ObservableCollection<JobsViewModel>();
            netLogViewModel = netLoggerViewModel;

        }

        [ObservableProperty]
        private bool _isTestScheduler = false;

        [ICommand]
        private void StandbyScheduler()
        {
            Scheduler.Standby();
        }
  
        [ObservableProperty]
        private List<IJobDetail> _jobDetails = new List<IJobDetail>();

        [ObservableProperty]
        private List<ITrigger> _triggers = new List<ITrigger>();

        public class TriggersViewModel
        {
            public string Name { get; set; }
            public string Group { get; set; }
            public IScheduler Scheduler { get; set; }
            public TriggerKey TriggerKey { get; set; }
            public DelegateCommand PauseTriggerCommand { get; set; }

            public DelegateCommand ResumeTriggerCommand { get; set; }

            public void Pause(JobsViewModel jobsViewModel)
            {
                var tr = new TriggerKey(jobsViewModel.TriggerName, jobsViewModel.TriggerGroup);
                Scheduler.PauseTrigger(tr);
                //  UpdateJobList();
            }
        }

        public partial class JobsViewModel : ObservableObject
        {

            [ObservableProperty]
            private string _group;



            [ObservableProperty]
            private string _name;


            [ObservableProperty]
            private string _status;



            [ObservableProperty]
            private string _triggerName;



            [ObservableProperty]
            private string _triggerGroup;



            [ObservableProperty]
            private string _nextTriggerTime;


            public string CronExpress { get; set; }
        }


        [ObservableProperty]
        private ObservableCollection<JobsViewModel> _jobsViewModels;


        public SchedulerJobType SchedulerType { get; set; } = SchedulerJobType.NoromalReadProfileBuffer;

        [ICommand]
        public void LoadingJob()
        {
            //每次都清空，保证只有一个
            Scheduler.Clear();
            switch (SchedulerType)
            {
                case SchedulerJobType.ClearBuffer:
                    Scheduler = DemoScheduler.CreatClearBuffer(false).Result;
                    break;
                case SchedulerJobType.NoromalReadProfileBuffer:
                    Scheduler = DemoScheduler.CreateNormal(false).Result;
                    break;
                case SchedulerJobType.TestReadProfileBuffer:
                    Scheduler = DemoScheduler.CreateTest(false).Result;
                    break;
                default:
                    break;
            }
            UpdateJobList();
        }
        [ICommand]
        public async void StartScheduler()
        {
            //if (SchedulerType==SchedulerJobType.ClearBuffer)
            //{
            //    Scheduler = DemoScheduler.CreatClearBuffer(false).Result;
            //}
            //else
            //{
            //    Scheduler = DemoScheduler.CreateNormal(false).Result;
            //}
            Scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await Scheduler.Start();
            Scheduler.ListenerManager.AddJobListener(this);
            Scheduler.JobFactory = new ProfileGenicJobFactory(serviceProvider);
            IsSchedulerStarted = Scheduler.IsStarted;
            UpdateJobList();
        }

        [ICommand]
        private void UpdateJobList()
        {
            JobsViewModels = new ObservableCollection<JobsViewModel>();
            NameList = Scheduler.GetJobGroupNames().GetAwaiter().GetResult();
            var nList = NameList.ToList();

            for (int i = 0; i < NameList.Count; i++)
            {
                var data = new JobsViewModel();

                foreach (var jobKey in Scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(nList[i])).Result)
                {
                    data.Group = jobKey.Group;
                    data.Name = jobKey.Name;
                }

                foreach (TriggerKey triggerKey in Scheduler
                    .GetTriggerKeys(GroupMatcher<TriggerKey>.GroupEquals(nList[i])).Result)
                {
                    data.Status = Scheduler.GetTriggerState(triggerKey).Result.ToString();
                    data.TriggerName = triggerKey.Name;
                    data.TriggerGroup = triggerKey.Group;
                    if (Scheduler.GetTrigger(triggerKey).Result is ICronTrigger cronTrigger)
                    {
                        CronExpression cronExpression = new CronExpression(cronTrigger.CronExpressionString);
                        data.NextTriggerTime = cronExpression.GetNextValidTimeAfter(DateTime.Now)?.LocalDateTime
                            .ToLongTimeString();
                        // data.LastFireTime = cronExpression.GetFinalFireTime()?.LocalDateTime.ToLongTimeString();
                        data.CronExpress = cronExpression.CronExpressionString;
                    }
                    else if (Scheduler.GetTrigger(triggerKey).Result is ISimpleTrigger simple)
                    {
                        data.NextTriggerTime = simple.RepeatInterval.Duration().ToString();
                        //  data.LastFireTime = simple.FinalFireTimeUtc.ToString();
                    }
                }

                DispatcherHelper.CheckBeginInvokeOnUI(() => { JobsViewModels.Add(data); });
            }
        }

        [ICommand]
        public void PauseTrigger(JobsViewModel triggerKey)
        {
            var tr = new TriggerKey(triggerKey.TriggerName, triggerKey.TriggerGroup);
            Scheduler.PauseTrigger(tr);
            UpdateJobList();
        }
        [ICommand]
        public void ResumeTrigger(JobsViewModel triggerKey)
        {
            var tr = new TriggerKey(triggerKey.TriggerName, triggerKey.TriggerGroup);
            Scheduler.ResumeTrigger(tr);
            UpdateJobList();
        }
        [ICommand]
        public void PauseAllScheduler()
        {
            Scheduler.PauseAll();
            UpdateJobList();
        }
        [ICommand]
        public void ShutdownScheduler()
        {
            Scheduler?.Shutdown();
            JobsViewModels.Clear();
            IsSchedulerStarted = false;
        }
        [ICommand]
        public void ResumeAllScheduler()
        {
            Scheduler.ResumeAll();
            UpdateJobList();
        }
        public Task TriggerFired(ITrigger trigger, IJobExecutionContext context,
            CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine(@"TriggerFired");
            return Task.CompletedTask;
        }

        public Task<bool> VetoJobExecution(ITrigger trigger, IJobExecutionContext context,
            CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine(@"VetoJobExecution");
            return Task.FromResult(true);
        }

        public Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine(@"TriggerMisfired");
            return Task.CompletedTask;
        }

        public Task TriggerComplete(ITrigger trigger, IJobExecutionContext context,
            SchedulerInstruction triggerInstructionCode,
            CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine(@"TriggerComplete");
            UpdateJobList();
            return Task.CompletedTask;
        }

        public Task JobToBeExecuted(IJobExecutionContext context,
            CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine(@"JobToBeExecuted");

            JobExecutionContexts = Scheduler.GetCurrentlyExecutingJobs(cancellationToken).Result;
            netLogViewModel.LogTrace($"JobToBeExecuted");
            return Task.CompletedTask;
        }

        public Task JobExecutionVetoed(IJobExecutionContext context,
            CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine(@"JobExecutionVetoed");
            netLogViewModel.LogTrace($"JobExecutionVetoed");

            return Task.CompletedTask;
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException,
            CancellationToken cancellationToken = new CancellationToken())
        {
            netLogViewModel.LogTrace(
                $"Executed {Scheduler.GetMetaData().Result.NumberOfJobsExecuted} Jobs.  RunningSince{Scheduler.GetMetaData().Result.RunningSince.Value.ToLocalTime()}");
            UpdateJobList();
            return Task.CompletedTask;
        }

        public string Name { get; } = "HAHAHAH";

    }
}