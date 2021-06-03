namespace 三相智慧能源网关调试软件.Model.Jobs
{
    public class MonthProfileGenericJob : ProfileGenericJobBase
    {
        public MonthProfileGenericJob()
        {
            JobName = "月结算曲线任务";
            Period = 60 * 24 * 31;
            CustomCosemProfileGenericModel = new CustomCosemProfileGenericModel("0.0.98.1.0.255")
            {
            };
        }
    }
}