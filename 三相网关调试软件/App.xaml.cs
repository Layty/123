using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using NLog;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using MySerialPortMaster;
using 三相智慧能源网关调试软件.Helpers;
using 三相智慧能源网关调试软件.MyControl;
using 三相智慧能源网关调试软件.Properties;
using 三相智慧能源网关调试软件.ViewModels;
using 三相智慧能源网关调试软件.ViewModels.DlmsViewModels;
using 三相智慧能源网关调试软件.Views.Management;

namespace 三相智慧能源网关调试软件
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static Logger Logger = LogManager.GetCurrentClassLogger();
        public new static App Current => (App)Application.Current;
        public IServiceProvider Services { get; }
        public App()
        {
            Services = ConfigureServices();
        }
        private static IServiceProvider ConfigureServices()
        {

            var services = new ServiceCollection();
          
            //串口配置管理者
            //var serialPortConfigCaretaker =
            //    new SerialPortConfigCaretaker(Settings.Default.SerialPortViewModelConfigFilePath);
            var serialPortConfigCaretaker =
                new SerialPortConfigCaretaker();
            //从管理者中得到串口配置，进行串口相关参数初始化
            var config = serialPortConfigCaretaker.LoadSerialPortConfigFromJsonFile();
            var serialPortMaster = new SerialPortMaster(config);
           
            services.AddSingleton(serialPortMaster);
            services.AddTransient<SerialPortViewModel>(); //RS485串口

            //DLMS各个接口类的配置文件
            ExcelHelper excel = new ExcelHelper("DLMS设备信息.xls");
            services.AddSingleton(excel);
            services.AddSingleton<DlmsClient>();
            services.AddSingleton<DlmsSettingsViewModel>();
            services.AddSingleton<DataViewModel>();
            services.AddSingleton<RegisterViewModel>();
            services.AddSingleton<ProfileGenericViewModel>();
            services.AddSingleton<ClockViewModel>();
            services.AddSingleton<LoadIdentificationViewModel>();

            services.AddSingleton<MainViewModel>(); //主窗体
            services.AddSingleton<MenuViewModel>(); //菜单
            services.AddSingleton<UserLoginViewModel>(); //用户登录
            services.AddSingleton<ColorToolViewModel>(); //程序调色板，皮肤
            services.AddSingleton<SkinViewModel>(); //程序调色板，皮肤，开机直接应用

            services.AddSingleton<SnackbarViewModel>();


            services.AddSingleton<TelnetViewModel>(); //网关调试登录Telnet客户端
            services.AddSingleton<TcpServerViewModel>(); //网关调试登录Telnet客户端
            services.AddSingleton<TftpServerViewModel>();
            services.AddSingleton<TftpClientViewModel>();
            services.AddSingleton<NetLogViewModel>();
            services.AddSingleton<XMLLogViewModel>();

          
            services.AddSingleton<JobCenterViewModel>();

            services.AddSingleton<DlmsBaseMeterViewModel>(); //基表DLMS协议
            services.AddSingleton<FileTransmitViewModel>(); //计量芯升级
            services.AddSingleton<IicDataViewModel>(); //IIC报文解析服务

            services.AddSingleton<UtilityTablesViewModel>(); //泰昂设备

            services.AddSingleton<ENetClientHelper>(); //网关登录使用的ENet客户端
            services.AddSingleton<ENetMessageBuilderViewModel>();

            services.AddSingleton<CosemObjectViewModel>();
            services.AddSingleton<MeterDataViewModel>();
            services.AddSingleton<DialogsViewModel>();

            services.AddSingleton<LocalNetHelper>();
            services.AddSingleton<SSHClientViewModel>();
            services.AddSingleton<SinglePhaseManagementPageViewModel>();

            services.AddTransient<IUserRepository, UserWebApi>();
            return services.BuildServiceProvider();
        }
        protected override void OnStartup(StartupEventArgs e)
        {

            DispatcherHelper.Initialize();

            Services.GetService<SkinViewModel>().ApplyBase();//开机应用主题

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            base.OnStartup(e);
        }


        //仅能捕获 Task 中抛出的未处理异常 事件的触发有延时，依赖垃圾回收
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Logger.Error("TaskError" + e.Exception);
            StrongReferenceMessenger.Default.Send($"TaskError{e.Exception}", "Snackbar");
            e.SetObserved();
        }

        //能捕获 所有线程（Task 除外） 抛出的未处理异常 默认情况无法阻止程序崩溃（可通过 legacyUnhandledExceptionPolicy 配置异常策略 ）
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Error("无法处理的异常" + e.ExceptionObject);
        }

        //能够捕获 UI 线程抛出的未处理异常 可通过事件参数 e.Handled = true 来阻止程序崩溃
        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Error($"Current_DispatcherUnhandledException:{e.Exception}");
            StrongReferenceMessenger.Default.Send($"{e.Exception.Message}", "Snackbar");
            e.Handled = true;
        }
    }
}