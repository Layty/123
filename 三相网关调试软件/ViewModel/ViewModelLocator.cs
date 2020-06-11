using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using 三相智慧能源网关调试软件.DLMS;
using 三相智慧能源网关调试软件.MyControl;

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
               
                #region 主程序界面相关

                SimpleIoc.Default.Register<MainViewModel>(); //主窗体
                SimpleIoc.Default.Register<MenuViewModel>(); //菜单
                SimpleIoc.Default.Register<UserLoginViewModel>(); //用户登录
                SimpleIoc.Default.Register<SkinViewModel>(); //程序调色板，皮肤

                #endregion

                #region 管理芯相关业务

                SimpleIoc.Default.Register<ENetClientHelper>(); //网关登录使用的ENet客户端
                SimpleIoc.Default.Register<TelnetViewModel>(); //网关调试登录Telnet客户端
              
                SimpleIoc.Default.Register<NetLogViewModel>();
                SimpleIoc.Default.Register<TftpServerViewModel>();
                #endregion

                #region 计量芯相关业务

                SimpleIoc.Default.Register<SerialPortViewModel>(); //RS485串口
                SimpleIoc.Default.Register<DlmsBaseMeterViewModel>(); //DLMS协议
                SimpleIoc.Default.Register<UpGradeBaseMeterViewModel>(); //计量芯升级
                #endregion

                #region 智能仪表

                SimpleIoc.Default.Register<TaiAngViewModel>(); //泰昂设备

                #endregion
            }
            else
            {
                SimpleIoc.Default.Register<MyDLMSSettings>();
                SimpleIoc.Default.Register<DLMSClient>();
                SimpleIoc.Default.Register<RegisterViewModel>();
                SimpleIoc.Default.Register<DataViewModel>();
                SimpleIoc.Default.Register<ProfileGenericViewModel>();
                #region 主程序界面相关

                SimpleIoc.Default.Register<MainViewModel>(); //主窗体
                SimpleIoc.Default.Register<MenuViewModel>(); //菜单
                SimpleIoc.Default.Register<UserLoginViewModel>(); //用户登录
                SimpleIoc.Default.Register<SkinViewModel>(); //程序调色板，皮肤

                #endregion

                #region 管理芯相关业务
                SimpleIoc.Default.Register<ENetClientHelper>(); //网关登录使用的ENet客户端
                #endregion

                #region 服务中心相关业务
             
            
                SimpleIoc.Default.Register<TelnetViewModel>(); //网关调试登录Telnet客户端
                SimpleIoc.Default.Register<TcpServerViewModel>(); //网关调试登录Telnet客户端
                SimpleIoc.Default.Register<TftpServerViewModel>();
                SimpleIoc.Default.Register<NetLogViewModel>();
                SimpleIoc.Default.Register<IicDataViewModel>(); //IIC报文解析服务
               
                #endregion
                #region 计量芯相关业务
                SimpleIoc.Default.Register<SerialPortViewModel>(); //RS485串口
                SimpleIoc.Default.Register<DlmsBaseMeterViewModel>(); //DLMS协议
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
        public DLMSClient DlmsClient => ServiceLocator.Current.GetInstance<DLMSClient>();
        public MyDLMSSettings DlmsSettings => ServiceLocator.Current.GetInstance<MyDLMSSettings>();
        public RegisterViewModel RegisterViewModel => ServiceLocator.Current.GetInstance<RegisterViewModel>();
        public DataViewModel DataViewModel => ServiceLocator.Current.GetInstance<DataViewModel>();
        public ProfileGenericViewModel ProfileGenericViewModel => ServiceLocator.Current.GetInstance<ProfileGenericViewModel>();

        public ENetClientHelper ENetClient => ServiceLocator.Current.GetInstance<ENetClientHelper>();
        public TelnetViewModel TcpClientHelper => ServiceLocator.Current.GetInstance<TelnetViewModel>();
       
        public NetLogViewModel Log => ServiceLocator.Current.GetInstance<NetLogViewModel>();
        public TftpServerViewModel TftpServer => ServiceLocator.Current.GetInstance<TftpServerViewModel>();
        public TcpServerViewModel TcpServer => ServiceLocator.Current.GetInstance<TcpServerViewModel>();

   
        #endregion

        #region 计量芯相关业务

        public SerialPortViewModel SerialPortViewModel => ServiceLocator.Current.GetInstance<SerialPortViewModel>();
        public DlmsBaseMeterViewModel DlmsBaseMeterViewModel => ServiceLocator.Current.GetInstance<DlmsBaseMeterViewModel>();

        public TaiAngViewModel TaiAngViewModel =>
            ServiceLocator.Current.GetInstance<TaiAngViewModel>();

        #endregion

        #region 智能仪表

        public UpGradeBaseMeterViewModel UpGradeBaseMeterViewModel =>
            ServiceLocator.Current.GetInstance<UpGradeBaseMeterViewModel>();

        #endregion

        #region IIC
        /// <summary>
        /// IIC数据视图模型
        /// </summary>
        public IicDataViewModel IicDataViewModel =>
            ServiceLocator.Current.GetInstance<IicDataViewModel>();


        #endregion
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}