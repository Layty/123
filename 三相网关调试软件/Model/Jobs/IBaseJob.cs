using Quartz;
using 三相智慧能源网关调试软件.ViewModel.DlmsViewModels;

namespace 三相智慧能源网关调试软件.Model.Jobs
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