using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            
            if (ViewModelBase.IsInDesignModeStatic)
            {
                //// Create design time view services and models
                //SimpleIoc.Default.Register<IDataService, DesignDataService>();
                //SimpleIoc.Default.Register<MainViewModel>();
                //SimpleIoc.Default.Register<MenuViewModel>();
                //SimpleIoc.Default.Register<UserLoginViewModel>();
                //SimpleIoc.Default.Register<InstantDataViewModel>();
                //SimpleIoc.Default.Register<EnergyDataViewModel>();
                //SimpleIoc.Default.Register<CommandViewModel>();
                //SimpleIoc.Default.Register<LogViewModel>();
                //SimpleIoc.Default.Register<ENetClientHelper>();

                //SimpleIoc.Default.Register<SerialPortViewModel>();
                //SimpleIoc.Default.Register<DlmsViewModel>();
                //SimpleIoc.Default.Register<UpGradeBaseMeterViewModel>();

                //SimpleIoc.Default.Register<SkinViewModel>();
                //SimpleIoc.Default.Register<TcpClientHelper>();
                #region 主程序界面相关

                SimpleIoc.Default.Register<MainViewModel>(); //主窗体
                SimpleIoc.Default.Register<MenuViewModel>(); //菜单
                SimpleIoc.Default.Register<UserLoginViewModel>(); //用户登录
                SimpleIoc.Default.Register<SkinViewModel>(); //程序调色板，皮肤

                #endregion

                #region 管理芯相关业务

                SimpleIoc.Default.Register<ENetClientHelper>(); //网关登录使用的ENet客户端
                SimpleIoc.Default.Register<TcpClientHelper>(); //网关调试登录Telnet客户端
                SimpleIoc.Default.Register<InstantDataViewModel>();
                SimpleIoc.Default.Register<EnergyDataViewModel>();
                SimpleIoc.Default.Register<CommandViewModel>();
                SimpleIoc.Default.Register<LogViewModel>();
                SimpleIoc.Default.Register<TftpServerViewModel>();
                #endregion

                #region 计量芯相关业务

                SimpleIoc.Default.Register<SerialPortViewModel>(); //RS485串口
                SimpleIoc.Default.Register<DlmsViewModel>(); //DLMS协议
                SimpleIoc.Default.Register<UpGradeBaseMeterViewModel>(); //计量芯升级
                #endregion

                #region 智能仪表

                SimpleIoc.Default.Register<TaiAngViewModel>(); //泰昂设备

                #endregion
            }
            else
            {
                #region 主程序界面相关

                SimpleIoc.Default.Register<MainViewModel>(); //主窗体
                SimpleIoc.Default.Register<MenuViewModel>(); //菜单
                SimpleIoc.Default.Register<UserLoginViewModel>(); //用户登录
                SimpleIoc.Default.Register<SkinViewModel>(); //程序调色板，皮肤

                #endregion

                #region 管理芯相关业务

                SimpleIoc.Default.Register<ENetClientHelper>(); //网关登录使用的ENet客户端
                SimpleIoc.Default.Register<TcpClientHelper>(); //网关调试登录Telnet客户端
                SimpleIoc.Default.Register<InstantDataViewModel>();
                SimpleIoc.Default.Register<EnergyDataViewModel>();
                SimpleIoc.Default.Register<CommandViewModel>();
                SimpleIoc.Default.Register<LogViewModel>();
                SimpleIoc.Default.Register<TftpServerViewModel>();
                #endregion

                #region 计量芯相关业务

                SimpleIoc.Default.Register<SerialPortViewModel>(); //RS485串口
                SimpleIoc.Default.Register<DlmsViewModel>(); //DLMS协议
                SimpleIoc.Default.Register<UpGradeBaseMeterViewModel>(); //计量芯升级
                #endregion

                #region 智能仪表

                SimpleIoc.Default.Register<TaiAngViewModel>(); //泰昂设备

                #endregion
            }

        }
        //public INavigationService InitNavigationService()
        //{
        //    NavigationService navigationService = new NavigationService();

        //    navigationService.Configure("Login", typeof(UserLoginPage));
        //    navigationService.Configure("Main", typeof(MainWindow));
        //    return navigationService;
        //}

        #region 主程序界面相关

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
        public MenuViewModel Menu => ServiceLocator.Current.GetInstance<MenuViewModel>();
        public UserLoginViewModel Login => ServiceLocator.Current.GetInstance<UserLoginViewModel>();
        public SkinViewModel Skin => ServiceLocator.Current.GetInstance<SkinViewModel>();

        #endregion

        #region 管理芯相关业务

        public ENetClientHelper ENetClient => ServiceLocator.Current.GetInstance<ENetClientHelper>();
        public TcpClientHelper TcpClientHelper => ServiceLocator.Current.GetInstance<TcpClientHelper>();
        public InstantDataViewModel InstantData => ServiceLocator.Current.GetInstance<InstantDataViewModel>();
        public EnergyDataViewModel EnergyData => ServiceLocator.Current.GetInstance<EnergyDataViewModel>();
        public CommandViewModel Command => ServiceLocator.Current.GetInstance<CommandViewModel>();
        public LogViewModel Log => ServiceLocator.Current.GetInstance<LogViewModel>();
        public TftpServerViewModel TftpServer => ServiceLocator.Current.GetInstance<TftpServerViewModel>();
        #endregion

        #region 计量芯相关业务

        public SerialPortViewModel SerialPortViewModel => ServiceLocator.Current.GetInstance<SerialPortViewModel>();
        public DlmsViewModel DlmsViewModel => ServiceLocator.Current.GetInstance<DlmsViewModel>();

        public TaiAngViewModel TaiAngViewModel =>
            ServiceLocator.Current.GetInstance<TaiAngViewModel>();

        #endregion

        #region 智能仪表

        public UpGradeBaseMeterViewModel UpGradeBaseMeterViewModel =>
            ServiceLocator.Current.GetInstance<UpGradeBaseMeterViewModel>();

        #endregion
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}