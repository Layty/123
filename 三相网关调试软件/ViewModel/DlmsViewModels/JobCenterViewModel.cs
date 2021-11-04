using CommonServiceLocator;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Quartz;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using 三相智慧能源网关调试软件.Helpers;
using 三相智慧能源网关调试软件.Model.Jobs;

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
        public RelayCommand StandbySchedulerCommand { get; set; }
        public RelayCommand<JobsViewModel> PauseTriggerCommand { get; set; }
        public RelayCommand<JobsViewModel> ResumeTriggerCommand { get; set; }
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
            JobsViewModels = new ObservableCollection<JobsViewModel>();

            StartSchedulerCommand = new RelayCommand(Start);

            ShutdownSchedulerCommand = new RelayCommand(Shutdown);
            PauseAllSchedulerCommand = new RelayCommand(PauseAll);
            ResumeAllSchedulerCommand = new RelayCommand(() =>
            {
                Scheduler.ResumeAll();
                UpdateJobList();
            });
            PauseTriggerCommand = new RelayCommand<JobsViewModel>(Pause);
            ResumeTriggerCommand = new RelayCommand<JobsViewModel>(Resume);

            StandbySchedulerCommand = new RelayCommand(Standby);
            UpdateJobListCommand = new RelayCommand(UpdateJobList);

            ActionCloseWarningCommand = new RelayCommand(CloseWaring);



          //  Scheduler = DemoScheduler.Create(false).Result;
            Scheduler = DemoScheduler.CreateTest(false).Result;
            Scheduler.ListenerManager.AddJobListener(this);
            netLogViewModel = ServiceLocator.Current.GetInstance<NetLogViewModel>();
            // Scheduler.ListenerManager.AddTriggerListener(this);
        }

        private void Standby()
        {
            Scheduler.Standby();
        }


        public List<IJobDetail> JobDetails
        {
            get => _jobDetails;
            set
            {
                _jobDetails = value;
                OnPropertyChanged();
            }
        }

        private List<IJobDetail> _jobDetails = new List<IJobDetail>();


        public List<ITrigger> Triggers
        {
            get => _triggers;
            set
            {
                _triggers = value;
                OnPropertyChanged();
            }
        }

        private List<ITrigger> _triggers = new List<ITrigger>();

        public class TriggersViewModel
        {
            public string Name { get; set; }
            public string Group { get; set; }
            public IScheduler Scheduler { get; set; }
            public TriggerKey TriggerKey { get; set; }
            public RelayCommand PauseTriggerCommand { get; set; }

            public RelayCommand ResumeTriggerCommand { get; set; }
            public void Pause(JobsViewModel jobsViewModel)
            {
                var tr = new TriggerKey(jobsViewModel.TriggerName, jobsViewModel.TriggerGroup);
                Scheduler.PauseTrigger(tr);
                //UpdateJobList();
            }
        }
        public class JobsViewModel : ObservableObject
        {

            public string Group
            {
                get => _group;
                set
                {
                    _group = value;
                    OnPropertyChanged();
                }
            }

            private string _group;


            public string Name
            {
                get => _name;
                set
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }

            private string _name;

            public string Status
            {
                get => _status;
                set
                {
                    _status = value;
                    OnPropertyChanged();
                }
            }

            private string _status;


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


            public string LastFireTime
            {
                get => _lastFireTime;
                set
                {
                    _lastFireTime = value;
                    OnPropertyChanged();
                }
            }

            private string _lastFireTime;

            public string CronExpress { get; set; }


        }

        public ObservableCollection<JobsViewModel> JobsViewModels
        {
            get => _jobsViewModels;
            set
            {
                _jobsViewModels = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<JobsViewModel> _jobsViewModels;


        public async void Start()
        {
            Scheduler.ListenerManager.AddJobListener(this);
            await Scheduler.Start();
            IsSchedulerStarted = Scheduler.IsStarted;
            UpdateJobList();
        }


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
                        data.LastFireTime = cronExpression.GetFinalFireTime()?.LocalDateTime.ToLongTimeString();
                        data.CronExpress = cronExpression.CronExpressionString;

                    }
                    else if (Scheduler.GetTrigger(triggerKey).Result is ISimpleTrigger simple)
                    {
                        data.NextTriggerTime = simple.RepeatInterval.Duration().ToString();
                        data.LastFireTime = simple.FinalFireTimeUtc.ToString();

                    }
                }

                DispatcherHelper.CheckBeginInvokeOnUI(() => { JobsViewModels.Add(data); });
            }
        }

        public void Pause(JobsViewModel triggerKey)
        {
            var tr = new TriggerKey(triggerKey.TriggerName, triggerKey.TriggerGroup);
            Scheduler.PauseTrigger(tr);
            UpdateJobList();
        }

        public void Resume(JobsViewModel triggerKey)
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
            JobsViewModels.Clear();
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

            JobExecutionContexts = Scheduler.GetCurrentlyExecutingJobs(cancellationToken).Result;
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