using JobMaster.Models;

namespace JobMaster.Jobs
{
    public interface IProfileGenericJob : IBaseJob
    {
        /// <summary>
        /// 曲线对象
        /// </summary>
        CustomCosemProfileGenericModel CustomCosemProfileGenericModel { get; set; }
    }
}