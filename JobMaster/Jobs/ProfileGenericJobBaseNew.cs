using JobMaster.Models;
using JobMaster.Services;
using JobMaster.ViewModels;
using Quartz;
using System.Threading.Tasks;

namespace JobMaster.Jobs
{
    /// <summary>
    /// 曲线采集的父类，定义实现了基本的采集规则
    /// </summary>
    public abstract class ProfileGenericJobBaseNew : IProfileGenericJob
    {
        protected ProfileGenericJobBaseNew(NetLoggerViewModel netLoggerViewModel, IProtocol protocol, DlmsSettingsViewModel dlmsSettingsViewModel)
        {
            NetLogViewModel = netLoggerViewModel;
            DlmsSettingsViewModel = dlmsSettingsViewModel;
            Protocol = protocol;
        }

        public CustomCosemProfileGenericModel CustomCosemProfileGenericModel { get; set; }
     
        public IProtocol Protocol { get; set; }
        public DlmsSettingsViewModel DlmsSettingsViewModel { get; set; }
        public string JobName { get; set; }
        public int Period { get; set; }
        public NetLoggerViewModel NetLogViewModel { get; set; }

        public virtual Task Execute(IJobExecutionContext context)
        {
            return Task.CompletedTask;
            //throw new NotImplementedException();
        }

       
    }
}