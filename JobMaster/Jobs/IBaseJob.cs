using JobMaster.ViewModels;
using Quartz;

namespace JobMaster.Jobs
{
    public interface IBaseJob : IJob
    {
        DlmsClient Client { get; set; }

        /// <summary>
        /// 曲线任务的名称
        /// </summary>
        string JobName { get; set; }


        int Period { get; set; }
    }
}