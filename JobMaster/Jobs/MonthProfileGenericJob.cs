using JobMaster.Models;
using JobMaster.ViewModels;

namespace JobMaster.Jobs
{
    public class MonthProfileGenericJob : ProfileGenericJobBase
    {
        public MonthProfileGenericJob(NetLoggerViewModel netLoggerViewModel, DlmsClient dlmsClient) : base(netLoggerViewModel, dlmsClient)
        {
            JobName = "月结算曲线任务";
            Period = 60 * 24 * 31;
            CustomCosemProfileGenericModel = new CustomCosemProfileGenericModel("0.0.98.1.0.255")
            {
            };
        }
    }
}