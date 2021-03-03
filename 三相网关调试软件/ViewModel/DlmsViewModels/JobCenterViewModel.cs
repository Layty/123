using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using 三相智慧能源网关调试软件.Model;

namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
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
    public class JobCenterViewModel : ObservableObject, ITriggerListener, IJobListener
    {
        public RelayCommand StartSchedulerCommand { get; set; }
        public RelayCommand ShutdownSchedulerCommand { get; set; }
        public RelayCommand PauseAllSchedulerCommand { get; set; }
        public RelayCommand ResumeAllSchedulerCommand { get; set; }
        public RelayCommand<JobMessage> PauseTriggerCommand { get; set; }
        public RelayCommand<JobMessage> ResumeTriggerCommand { get; set; }
        public RelayCommand UpdateJobListCommand { get; set; }


        public RelayCommand ActionCloseWarningCommand { get; set; }

        public ObservableCollection<ProfileGenericJobBase> List { get; set; }


        public bool IsSchedulerStarted
        {
            get => _isSchedulerStarted;
            set
            {
                _isSchedulerStarted = value;
                OnPropertyChanged();
            }
        }

        private bool _isSchedulerStarted;


        public void AddJob(ProfileGenericJobBase jobBase)
        {
            List.Add(jobBase);
        }

        public void RemoveJob(ProfileGenericJobBase jobBase)
        {
            List.Remove(jobBase);
        }

        public void UpDate()
        {
        }

        public void CloseWaring()
        {
            IJobDetail normalJobDetail = JobBuilder.Create<CloseWarmingJob>()
                .WithIdentity("CloseWarmingJob", "CloseWarmingJob").Build();
            ITrigger normalTrigger = TriggerBuilder.Create()
                .WithIdentity("CloseWarmingJobTrigger", "CloseWarmingJob")
                .WithSimpleSchedule().StartNow()
                .Build();

            Scheduler.ScheduleJob(normalJobDetail, normalTrigger);
        }

        //调度器工厂
        ISchedulerFactory factory;
        //调度器


        public IScheduler Scheduler
        {
            get => _scheduler;
            set
            {
                _scheduler = value;
                OnPropertyChanged();
            }
        }

        private IScheduler _scheduler;

        public IReadOnlyCollection<IJobExecutionContext> JobExecutionContexts
        {
            get => _jobExecutionContexts;
            set
            {
                _jobExecutionContexts = value;
                OnPropertyChanged();
            }
        }

        private IReadOnlyCollection<IJobExecutionContext> _jobExecutionContexts;


        public IReadOnlyCollection<string> NameList
        {
            get => _nameList;
            set
            {
                _nameList = value;
                OnPropertyChanged();
            }
        }

        private IReadOnlyCollection<string> _nameList;

        private NetLogViewModel netLogViewModel;


        public JobCenterViewModel()
        {
            List = new ObservableCollection<ProfileGenericJobBase>()
            {
                new EnergyProfileGenericJob(),
                new PowerProfileGenericJob(),
                new DayProfileGenericJob(),
                new MonthProfileGenericJob()
            };
            JobMessages = new ObservableCollection<JobMessage>();

            StartSchedulerCommand = new RelayCommand(Start);
            ShutdownSchedulerCommand = new RelayCommand(Shutdown);
            PauseAllSchedulerCommand = new RelayCommand(PauseAll);
            ResumeAllSchedulerCommand = new RelayCommand(() =>
            {
                Scheduler.ResumeAll();
                UpdateJobList();
            });
            PauseTriggerCommand = new RelayCommand<JobMessage>(Pause);
            ResumeTriggerCommand = new RelayCommand<JobMessage>(Resume);
            UpdateJobListCommand = new RelayCommand(UpdateJobList);

            ActionCloseWarningCommand = new RelayCommand(CloseWaring);


            factory = new StdSchedulerFactory();
            //创建任务调度器
            Scheduler = factory.GetScheduler().Result;

            Scheduler.ListenerManager.AddJobListener(this);
            netLogViewModel = SimpleIoc.Default.GetInstance<NetLogViewModel>();
            // Scheduler.ListenerManager.AddTriggerListener(this);
        }

        private async void LoadingDefaultJob()
        {
            IJobDetail energyJobDetail = JobBuilder.Create<EnergyProfileGenericJob>()
                .WithIdentity("EnergyProfileGenericJob", "EnergyProfileGenericJob").Build();
            var t = energyJobDetail.JobType.FullName;

            Console.WriteLine("haha" + energyJobDetail.Description);
            ITrigger energyTrigger = TriggerBuilder.Create()
                .WithCronSchedule("10 0/5 * * * ? *", x => x.WithMisfireHandlingInstructionDoNothing())
                .WithIdentity("EnergyProfileGenericJobTrigger", "EnergyProfileGenericJob").Build();

            //功率
            IJobDetail powerJobDetail = JobBuilder.Create<PowerProfileGenericJob>()
                .WithIdentity("PowerProfileGenericJob", "PowerProfileGenericJob").Build();

            ITrigger powerTrigger = TriggerBuilder.Create()
                .WithIdentity("PowerProfileGenericJobTrigger", "PowerProfileGenericJob")
                .WithCronSchedule("40 5/15 * * * ? *", x => x.WithMisfireHandlingInstructionDoNothing())
                .Build();

            //日
            IJobDetail dayJobDetail = JobBuilder.Create<DayProfileGenericJob>()
                .WithIdentity("DayProfileGenericJob", "DayProfileGenericJob").Build();
            ITrigger dayTrigger = TriggerBuilder.Create()
                .WithIdentity("DayProfileGenericJobTrigger", "DayProfileGenericJob")
                .WithCronSchedule("0 2 0 * * ? *", x => x.WithMisfireHandlingInstructionDoNothing())
                .Build();


            await Scheduler.ScheduleJob(energyJobDetail, energyTrigger);
            await Scheduler.ScheduleJob(powerJobDetail, powerTrigger);
            await Scheduler.ScheduleJob(dayJobDetail, dayTrigger);
        }

        public class JobMessage : ObservableObject
        {
            public string JobGroup
            {
                get => _jobGroup;
                set
                {
                    _jobGroup = value;
                    OnPropertyChanged();
                }
            }

            private string _jobGroup;


            public string JobName
            {
                get => _jobName;
                set
                {
                    _jobName = value;
                    OnPropertyChanged();
                }
            }

            private string _jobName;

            public string Status
            {
                get => _Status;
                set
                {
                    _Status = value;
                    OnPropertyChanged();
                }
            }

            private string _Status;


            public string TriggerName
            {
                get => _triggerName;
                set
                {
                    _triggerName = value;
                    OnPropertyChanged();
                }
            }

            private string _triggerName;


            public string TriggerGroup
            {
                get => _triggerGroup;
                set
                {
                    _triggerGroup = value;
                    OnPropertyChanged();
                }
            }

            private string _triggerGroup;


            public string NextTriggerTime
            {
                get => _nextTriggerTime;
                set
                {
                    _nextTriggerTime = value;
                    OnPropertyChanged();
                }
            }

            private string _nextTriggerTime;
        }

        public ObservableCollection<JobMessage> JobMessages
        {
            get => _jobMessages;
            set
            {
                _jobMessages = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<JobMessage> _jobMessages;


        public void Start()
        {
            factory = new StdSchedulerFactory();
            //创建任务调度器
            Scheduler = factory.GetScheduler().Result;
            Scheduler.ListenerManager.AddJobListener(this);
            LoadingDefaultJob();
            Scheduler.Start();
            IsSchedulerStarted = true;
            UpdateJobList();
        }

        private void UpdateJobList()
        {
            JobMessages = new ObservableCollection<JobMessage>();
            NameList = Scheduler.GetJobGroupNames().GetAwaiter().GetResult();
            var nList = NameList.ToList();

            for (int i = 0; i < NameList.Count; i++)
            {
                var data = new JobMessage();

                foreach (var jobKey in Scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(nList[i])).Result)
                {
                    data.JobGroup = jobKey.Group;
                    data.JobName = jobKey.Name;
                }

                foreach (TriggerKey triggerKey in Scheduler
                    .GetTriggerKeys(GroupMatcher<TriggerKey>.GroupEquals(nList[i])).Result)
                {
                    data.Status = (Scheduler.GetTriggerState(triggerKey).Result).ToString();
                    data.TriggerName = triggerKey.Name;
                    data.TriggerGroup = triggerKey.Group;
                    if (Scheduler.GetTrigger(triggerKey).Result is ICronTrigger)
                    {
                        var cronTrigger = (ICronTrigger)Scheduler.GetTrigger(triggerKey).Result;
                        CronExpression cronExpression = new CronExpression(cronTrigger.CronExpressionString);
                        data.NextTriggerTime = cronExpression.GetNextValidTimeAfter(DateTime.Now).Value.LocalDateTime
                            .ToLongTimeString();
                    }
                    else if (Scheduler.GetTrigger(triggerKey).Result is ISimpleTrigger)
                    {
                        data.NextTriggerTime = "未知";
                    }
                    
                }


                JobMessages.Add(data);
            }
        }

        public void Pause(JobMessage triggerKey)
        {
            var tr = new TriggerKey(triggerKey.TriggerName, triggerKey.TriggerGroup);
            Scheduler.PauseTrigger(tr);
            UpdateJobList();
        }

        public void Resume(JobMessage triggerKey)
        {
            var tr = new TriggerKey(triggerKey.TriggerName, triggerKey.TriggerGroup);
            Scheduler.ResumeTrigger(tr);

            UpdateJobList();
        }

        public void PauseAll()
        {
            Scheduler.PauseAll();
            UpdateJobList();
        }

        public void Shutdown()
        {
            Scheduler?.Shutdown();
            JobMessages.Clear();
            IsSchedulerStarted = false;
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

            JobExecutionContexts = Scheduler.GetCurrentlyExecutingJobs().Result;
            netLogViewModel.MyServerNetLogModel.Log = $"JobToBeExecuted";
            return Task.CompletedTask;
        }

        public Task JobExecutionVetoed(IJobExecutionContext context,
            CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine(@"JobExecutionVetoed");
            netLogViewModel.MyServerNetLogModel.Log = $"JobExecutionVetoed";
            return Task.CompletedTask;
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException,
            CancellationToken cancellationToken = new CancellationToken())
        {
            netLogViewModel.MyServerNetLogModel.Log =
                $"Executed {Scheduler.GetMetaData().Result.NumberOfJobsExecuted} Jobs.  RunningSince{Scheduler.GetMetaData().Result.RunningSince.Value.ToLocalTime()}";
            UpdateJobList();
            return Task.CompletedTask;
        }

        public string Name { get; } = "HAHAHAH";
    }
}