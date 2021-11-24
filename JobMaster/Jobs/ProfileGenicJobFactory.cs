using Quartz;
using Quartz.Spi;
using System;

namespace JobMaster.Jobs
{
    /// <summary>
    /// 使用IJobFactory 实现依赖注入的Job
    /// </summary>
    public class ProfileGenicJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ProfileGenicJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            Type jobType = bundle.JobDetail.JobType;
            return _serviceProvider.GetService(jobType) as IJob;
        }

        public void ReturnJob(IJob job)
        {
        }
    }
}