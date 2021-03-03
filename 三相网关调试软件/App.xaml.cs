using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Lierda.WPFHelper;
using NLog;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using 三相智慧能源网关调试软件.ViewModel;

namespace 三相智慧能源网关调试软件
{
    public class AutofacLocator
    {
        IContainer container;

        public TInterface Get<TInterface>(string typeName)
        {
            return container.ResolveNamed<TInterface>(typeName);
        }

        public TInterface Get<TInterface>()
        {
            return container.Resolve<TInterface>();
        }

        public void Register()
        {
            var Container = new ContainerBuilder();

            Container.RegisterType<UserLoginViewModel>();
            container = Container.Build();
        }
    }

    public class ServiceProvider
    {
        public static AutofacLocator Instance { get; private set; }

        public static void RegisterServiceLocator(AutofacLocator s)
        {
            Instance = s;
        }
    }

    public class BootStrapper
    {
        /// <summary>
        /// 注册方法
        /// </summary>
        public static void Initialize(AutofacLocator autoFacLocator)
        {
            ServiceProvider.RegisterServiceLocator(autoFacLocator);
            ServiceProvider.Instance.Register();
        }
    }

    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly LierdaCracker _cracker = new LierdaCracker();

        /// <summary>
        /// 启动项注册
        /// </summary>
        protected void Configure(ContainerBuilder builder)
        {
            builder.RegisterType<UserLoginViewModel>();
        }

        protected void ConfigureServices()
        {
            AutofacLocator autofacLocator = new AutofacLocator(); //创建IOC容器
            autofacLocator.Register(); //注册服务
           BootStrapper.Initialize(autofacLocator);
        }

      
        protected override void OnStartup(StartupEventArgs e)
        {
            _cracker.Cracker();
            DispatcherHelper.Initialize();
           ConfigureServices();

      



            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            base.OnStartup(e);
        }


        //仅能捕获 Task 中抛出的未处理异常 事件的触发有延时，依赖垃圾回收
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Logger.Error("TaskError" + e.Exception);
            StrongReferenceMessenger.Default.Send("TaskError", "Snackbar");
            e.SetObserved();
        }

        //能捕获 所有线程（Task 除外） 抛出的未处理异常 默认情况无法阻止程序崩溃（可通过 legacyUnhandledExceptionPolicy 配置异常策略 ）
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Error("无法处理的异常啊" + e.ExceptionObject);
        }

        //能够捕获 UI 线程抛出的未处理异常 可通过事件参数 e.Handled = true 来阻止程序崩溃
        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Error($"Current_DispatcherUnhandledException:{e.Exception}");
            StrongReferenceMessenger.Default.Send("Current_DispatcherUnhandledException", "Snackbar");
            e.Handled = true;
        }
    }
}