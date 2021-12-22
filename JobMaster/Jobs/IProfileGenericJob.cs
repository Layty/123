using JobMaster.Models;

namespace JobMaster.Jobs
{
    /// <summary>
    /// 与类7曲线类相关的任务,继承者IBaseJob
    /// </summary>
    public interface IProfileGenericJob : IBaseJob
    {
        /// <summary>
        /// 曲线Cosem对象
        /// </summary>
        CustomCosemProfileGenericModel CustomCosemProfileGenericModel { get; set; }
    }
}