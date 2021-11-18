using JobMaster.ViewModels;
using JobMaster.Views;
using Microsoft.Extensions.Logging;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Regions;
using System.Windows;

namespace JobMaster
{
    public class Bootstrapper : PrismBootstrapper
    {
        
        protected override DependencyObject CreateShell()
        {

            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //注入Nlog
            var factory = new NLog.Extensions.Logging.NLogLoggerFactory();
            ILogger logger = factory.CreateLogger("");
            containerRegistry.RegisterInstance(logger);

            containerRegistry.RegisterSingleton<DlmsSettingsViewModel>();

            containerRegistry.RegisterSingleton<SerialPortViewModel>();

            containerRegistry.RegisterSingleton<DlmsClient>();

            //  containerRegistry.RegisterSingleton<FrontEndProcessorViewModel>();
            containerRegistry.RegisterSingleton<MainServerViewModel>();

            containerRegistry.RegisterSingleton<JobCenterViewModel>();
            containerRegistry.RegisterSingleton<NetLoggerViewModel>();

            containerRegistry.RegisterSingleton<DataNotificationViewModel>();

            //containerRegistry.RegisterForNavigation<FrontEndProcessorView>();
            //containerRegistry.RegisterForNavigation<JobCenterView>();

            //containerRegistry.RegisterForNavigation<NetLoggerView>();
            containerRegistry.RegisterForNavigation<JobCenterView>();
            containerRegistry.RegisterForNavigation<DataNotificationView>();
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            var _regionManager = Container.Resolve<IRegionManager>();
            //_regionManager.RegisterViewWithRegion("ServerRegion", typeof(FrontEndProcessorView));
            _regionManager.RegisterViewWithRegion("ServerRegion", typeof(MainServerView));
          //  _regionManager.RegisterViewWithRegion("JobRegion", typeof(JobCenterView));
            _regionManager.RegisterViewWithRegion("LogRegion", typeof(NetLoggerView));
            //  _regionManager.RegisterViewWithRegion("NotiRegion", typeof(DataNotificationView));
           
        }
    }
}
