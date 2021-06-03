namespace 三相智慧能源网关调试软件.Model.Jobs
{
    public interface IProfileGenericJob : IBaseJob
    {
        /// <summary>
        /// 曲线对象
        /// </summary>
        CustomCosemProfileGenericModel CustomCosemProfileGenericModel { get; set; }
    }
}