using CommonServiceLocator;
//using GalaSoft.MvvmLight.Ioc;
using Unity;
using Unity.ServiceLocation;
using 三相智慧能源网关调试软件.MyControl;
using 三相智慧能源网关调试软件.ViewModel.DlmsViewModels;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            UnityContainer container = new UnityContainer();
            UnityServiceLocator unityServiceLocator = new UnityServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => unityServiceLocator);
          //  ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            //注册服务
            {
                #region Dlms相关服务
                
                container.RegisterSingleton<DlmsSettingsViewModel>();
                container.RegisterSingleton<DataViewModel>();
                container.RegisterSingleton<RegisterViewModel>();
                container.RegisterSingleton<ProfileGenericViewModel>();
                container.RegisterSingleton<ClockViewModel>();
                container.RegisterSingleton<LoadIdentificationViewModel>();
                //SimpleIoc.Default.Register<DlmsSettingsViewModel>();
                //SimpleIoc.Default.Register<DataViewModel>();
                //SimpleIoc.Default.Register<RegisterViewModel>();
                //SimpleIoc.Default.Register<ProfileGenericViewModel>();
                //SimpleIoc.Default.Register<ClockViewModel>();
                //SimpleIoc.Default.Register<LoadIdentificationViewModel>();

                #endregion


                #region 主程序界面相关
                 container.RegisterSingleton<MainViewModel>(); //主窗体
                 container.RegisterSingleton<MenuViewModel>(); //菜单
                 container.RegisterSingleton<UserLoginViewModel>(); //用户登录
                 container.RegisterSingleton<ColorToolViewModel>(); //程序调色板，皮肤
                 container.RegisterSingleton<SkinViewModel>(); //程序调色板，皮肤，开机直接应用
                container.RegisterSingleton<SnackbarViewModel>();


                //SimpleIoc.Default.Register<MainViewModel>(); //主窗体
                //SimpleIoc.Default.Register<MenuViewModel>(); //菜单
                //SimpleIoc.Default.Register<UserLoginViewModel>(); //用户登录
                //SimpleIoc.Default.Register<ColorToolViewModel>(); //程序调色板，皮肤
                //SimpleIoc.Default.Register<SkinViewModel>(true); //程序调色板，皮肤，开机直接应用
                //SimpleIoc.Default.Register<SnackbarViewModel>();

                #endregion


                #region 服务中心相关业务
                container.RegisterSingleton<TelnetViewModel>(); //网关调试登录Telnet客户端
                container.RegisterSingleton<TcpServerViewModel>(); //网关调试登录Telnet客户端
                container.RegisterSingleton<TftpServerViewModel>();
                container.RegisterSingleton<TftpClientViewModel>();
                container.RegisterSingleton<NetLogViewModel>();
                container.RegisterSingleton<XMLLogViewModel>();
                container.RegisterSingleton<SerialPortViewModel>(); //RS485串口
                container.RegisterSingleton<DlmsClient>();
                container.RegisterSingleton<JobCenterViewModel>();

                //SimpleIoc.Default.Register<TelnetViewModel>(); //网关调试登录Telnet客户端
                //SimpleIoc.Default.Register<TcpServerViewModel>(); //网关调试登录Telnet客户端
                //SimpleIoc.Default.Register<TftpServerViewModel>();
                //SimpleIoc.Default.Register<TftpClientViewModel>();
                //SimpleIoc.Default.Register<NetLogViewModel>();
                //SimpleIoc.Default.Register<XMLLogViewModel>();
                //SimpleIoc.Default.Register<SerialPortViewModel>(); //RS485串口
                //SimpleIoc.Default.Register<DlmsClient>(true);
                //SimpleIoc.Default.Register<JobCenterViewModel>();

                #endregion

                #region 三相网关特有业务

                #region 三相网关计量芯相关业务

                container.RegisterSingleton<DlmsBaseMeterViewModel>(); //基表DLMS协议
                container.RegisterSingleton<FileTransmitViewModel>(); //计量芯升级
                container.RegisterSingleton<IicDataViewModel>(); //IIC报文解析服务
                //SimpleIoc.Default.Register<DlmsBaseMeterViewModel>(); //基表DLMS协议
                //SimpleIoc.Default.Register<FileTransmitViewModel>(); //计量芯升级
                //SimpleIoc.Default.Register<IicDataViewModel>(); //IIC报文解析服务
                #endregion

                #region 三相网关智能仪表业务

                container.RegisterSingleton<UtilityTablesViewModel>(); //泰昂设备

             //   SimpleIoc.Default.Register<UtilityTablesViewModel>(); //泰昂设备
                #endregion

                #region 三相网关管理芯相关业务

               container.RegisterSingleton<ENetClientHelper>(); //网关登录使用的ENet客户端
                container.RegisterSingleton<ENetMessageBuilderViewModel>();
                //SimpleIoc.Default.Register<ENetClientHelper>(); //网关登录使用的ENet客户端
                //SimpleIoc.Default.Register<ENetMessageBuilderViewModel>();
                #endregion

                #endregion


                container.RegisterSingleton<CosemObjectViewModel>();
                 container.RegisterSingleton<MeterDataViewModel>();
                 container.RegisterSingleton<DialogsViewModel>();
              
                 container.RegisterSingleton<LocalNetHelper>();
                container.RegisterSingleton<SSHClientViewModel>();


                //SimpleIoc.Default.Register<CosemObjectViewModel>();
                //SimpleIoc.Default.Register<MeterDataViewModel>();
                //SimpleIoc.Default.Register<DialogsViewModel>();

                //SimpleIoc.Default.Register<LocalNetHelper>();
                //SimpleIoc.Default.Register<SSHClientViewModel>();

            }
        }

        public SSHClientViewModel SshClientViewModel => ServiceLocator.Current.GetInstance<SSHClientViewModel>();
        public LocalNetHelper LocalNetHelper => ServiceLocator.Current.GetInstance<LocalNetHelper>();

        public DialogsViewModel DialogsViewModel => ServiceLocator.Current.GetInstance<DialogsViewModel>();

        #region 主程序界面相关

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
        public MenuViewModel Menu => ServiceLocator.Current.GetInstance<MenuViewModel>();
        public UserLoginViewModel Login => ServiceLocator.Current.GetInstance<UserLoginViewModel>();
        public ColorToolViewModel ColorToolViewModel => ServiceLocator.Current.GetInstance<ColorToolViewModel>();
        public SkinViewModel Skin => ServiceLocator.Current.GetInstance<SkinViewModel>();

        public ClockViewModel ClockViewModel => ServiceLocator.Current.GetInstance<ClockViewModel>();
        public SnackbarViewModel SnackbarViewModel => ServiceLocator.Current.GetInstance<SnackbarViewModel>();

        #endregion

        #region 管理芯相关业务

        public DlmsClient DlmsClient => ServiceLocator.Current.GetInstance<DlmsClient>();

        public DlmsSettingsViewModel DlmsSettingsViewModel =>
            ServiceLocator.Current.GetInstance<DlmsSettingsViewModel>();

        public RegisterViewModel RegisterViewModel => ServiceLocator.Current.GetInstance<RegisterViewModel>();
        public DataViewModel DataViewModel => ServiceLocator.Current.GetInstance<DataViewModel>();


        public ProfileGenericViewModel ProfileGenericViewModel =>
            ServiceLocator.Current.GetInstance<ProfileGenericViewModel>();

        public LoadIdentificationViewModel LoadIdentificationViewModel =>
            ServiceLocator.Current.GetInstance<LoadIdentificationViewModel>();

        public ENetClientHelper ENetClient => ServiceLocator.Current.GetInstance<ENetClientHelper>();

        public ENetMessageBuilderViewModel ENetMessageMakerViewModel =>
            ServiceLocator.Current.GetInstance<ENetMessageBuilderViewModel>();

        public TelnetViewModel TcpClientHelper => ServiceLocator.Current.GetInstance<TelnetViewModel>();

        public NetLogViewModel Log => ServiceLocator.Current.GetInstance<NetLogViewModel>();

        public XMLLogViewModel XmlLogViewModel => ServiceLocator.Current.GetInstance<XMLLogViewModel>();
        public TftpServerViewModel TftpServer => ServiceLocator.Current.GetInstance<TftpServerViewModel>();
        public TftpClientViewModel TftpClient => ServiceLocator.Current.GetInstance<TftpClientViewModel>();
        public TcpServerViewModel TcpServer => ServiceLocator.Current.GetInstance<TcpServerViewModel>();
        public JobCenterViewModel JobCenterViewModel => ServiceLocator.Current.GetInstance<JobCenterViewModel>();

        #endregion

        #region 计量芯相关业务

        public SerialPortViewModel SerialPortViewModel => ServiceLocator.Current.GetInstance<SerialPortViewModel>();

        public DlmsBaseMeterViewModel DlmsBaseMeterViewModel =>
            ServiceLocator.Current.GetInstance<DlmsBaseMeterViewModel>();

        public UtilityTablesViewModel UtilityTablesViewModel =>
            ServiceLocator.Current.GetInstance<UtilityTablesViewModel>();

        #endregion

        #region 智能仪表

        public FileTransmitViewModel FileTransmitViewModel =>
            ServiceLocator.Current.GetInstance<FileTransmitViewModel>();

        #endregion

        #region IIC

        /// <summary>
        /// IIC数据视图模型
        /// </summary>
        public IicDataViewModel IicDataViewModel =>
            ServiceLocator.Current.GetInstance<IicDataViewModel>();

        public CosemObjectViewModel CosemObjectViewModel => ServiceLocator.Current.GetInstance<CosemObjectViewModel>();
        public MeterDataViewModel MeterDataViewModel => ServiceLocator.Current.GetInstance<MeterDataViewModel>();

        #endregion

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}