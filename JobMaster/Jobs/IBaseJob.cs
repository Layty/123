using Quartz;

namespace JobMaster.Jobs
{
    public interface IBaseJob : IJob
    {
        /// <summary>
        /// 任务的名称
        /// </summary>
        string JobName { get; set; }

        /// <summary>
        /// 任务的间隔周期
        /// </summary>
        int Period { get; set; }
    }
}