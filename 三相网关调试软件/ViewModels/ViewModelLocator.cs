using Microsoft.Extensions.DependencyInjection;
using 三相智慧能源网关调试软件.MyControl;
using 三相智慧能源网关调试软件.ViewModels.DlmsViewModels;
using 三相智慧能源网关调试软件.Views.Management;

namespace 三相智慧能源网关调试软件.ViewModels
{
    /// <summary>
    /// 为界面绑定提供 便捷的引用
    /// </summary>
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
           
          
            //注册服务
            {


                #region 主程序界面相关

                //container.RegisterSingleton<MainViewModel>(); //主窗体
                //container.RegisterSingleton<MenuViewModel>(); //菜单
                //container.RegisterSingleton<UserLoginViewModel>(); //用户登录
                //container.RegisterSingleton<ColorToolViewModel>(); //程序调色板，皮肤
                // container.RegisterSingleton<SkinViewModel>(); //程序调色板，皮肤，开机直接应用
                //container.RegisterInstance(new SkinViewModel());
                //container.RegisterSingleton<SnackbarViewModel>();


                //SimpleIoc.Default.Register<MainViewModel>(); //主窗体
                //SimpleIoc.Default.Register<MenuViewModel>(); //菜单
                //SimpleIoc.Default.Register<UserLoginViewModel>(); //用户登录
                //SimpleIoc.Default.Register<ColorToolViewModel>(); //程序调色板，皮肤
                //SimpleIoc.Default.Register<SkinViewModel>(true); //程序调色板，皮肤，开机直接应用
                //SimpleIoc.Default.Register<SnackbarViewModel>();

                #endregion


                #region 服务中心相关业务

                //container.RegisterSingleton<TelnetViewModel>(); //网关调试登录Telnet客户端
                //container.RegisterSingleton<TcpServerViewModel>(); //网关调试登录Telnet客户端
                //container.RegisterSingleton<TftpServerViewModel>();
                //container.RegisterSingleton<TftpClientViewModel>();
                //container.RegisterSingleton<NetLogViewModel>();
                //container.RegisterSingleton<XMLLogViewModel>();
                //container.RegisterSingleton<SerialPortViewModel>(); //RS485串口
                //container.RegisterSingleton<DlmsClient>();
                //container.RegisterSingleton<JobCenterViewModel>();

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

                //container.RegisterSingleton<DlmsBaseMeterViewModel>(); //基表DLMS协议
                //container.RegisterSingleton<FileTransmitViewModel>(); //计量芯升级
                //container.RegisterSingleton<IicDataViewModel>(); //IIC报文解析服务
                //SimpleIoc.Default.Register<DlmsBaseMeterViewModel>(); //基表DLMS协议
                //SimpleIoc.Default.Register<FileTransmitViewModel>(); //计量芯升级
                //SimpleIoc.Default.Register<IicDataViewModel>(); //IIC报文解析服务

                #endregion

                #region 三相网关智能仪表业务

                //container.RegisterSingleton<UtilityTablesViewModel>(); //泰昂设备

                //   SimpleIoc.Default.Register<UtilityTablesViewModel>(); //泰昂设备

                #endregion

                #region 三相网关管理芯相关业务

                //container.RegisterSingleton<ENetClientHelper>(); //网关登录使用的ENet客户端
                //container.RegisterSingleton<ENetMessageBuilderViewModel>();
                //SimpleIoc.Default.Register<ENetClientHelper>(); //网关登录使用的ENet客户端
                //SimpleIoc.Default.Register<ENetMessageBuilderViewModel>();

                #endregion

                #endregion


                //container.RegisterSingleton<CosemObjectViewModel>();
                //container.RegisterSingleton<MeterDataViewModel>();
                //container.RegisterSingleton<DialogsViewModel>();

                //container.RegisterSingleton<LocalNetHelper>();
                //container.RegisterSingleton<SSHClientViewModel>();
            }
        }

        public SSHClientViewModel SshClientViewModel => App.Current.Services.GetService<SSHClientViewModel>();
        public LocalNetHelper LocalNetHelper => App.Current.Services.GetService<LocalNetHelper>();

        public DialogsViewModel DialogsViewModel => App.Current.Services.GetService<DialogsViewModel>();

        #region 主程序界面相关

        public MainViewModel MainViewModel => App.Current.Services.GetService<MainViewModel>();
        public MenuViewModel Menu => App.Current.Services.GetService<MenuViewModel>();
        public UserLoginViewModel Login => App.Current.Services.GetService<UserLoginViewModel>();
        public ColorToolViewModel ColorToolViewModel => App.Current.Services.GetService<ColorToolViewModel>();
        public SkinViewModel Skin => App.Current.Services.GetService<SkinViewModel>();

        public ClockViewModel ClockViewModel => App.Current.Services.GetService<ClockViewModel>();
        public SnackbarViewModel SnackbarViewModel => App.Current.Services.GetService<SnackbarViewModel>();

        #endregion

        #region 管理芯相关业务

        public DlmsClient DlmsClient => App.Current.Services.GetService<DlmsClient>();

        public DlmsSettingsViewModel DlmsSettingsViewModel =>
            App.Current.Services.GetService<DlmsSettingsViewModel>();

        public RegisterViewModel RegisterViewModel => App.Current.Services.GetService<RegisterViewModel>();
        public DataViewModel DataViewModel => App.Current.Services.GetService<DataViewModel>();


        public ProfileGenericViewModel ProfileGenericViewModel =>
            App.Current.Services.GetService<ProfileGenericViewModel>();

        public LoadIdentificationViewModel LoadIdentificationViewModel =>
            App.Current.Services.GetService<LoadIdentificationViewModel>();

        public ENetClientHelper ENetClient => App.Current.Services.GetService<ENetClientHelper>();

        public ENetMessageBuilderViewModel ENetMessageMakerViewModel =>
            App.Current.Services.GetService<ENetMessageBuilderViewModel>();

        public TelnetViewModel TcpClientHelper => App.Current.Services.GetService<TelnetViewModel>();

        public NetLogViewModel Log => App.Current.Services.GetService<NetLogViewModel>();

        public XMLLogViewModel XmlLogViewModel => App.Current.Services.GetService<XMLLogViewModel>();
        public TftpServerViewModel TftpServer => App.Current.Services.GetService<TftpServerViewModel>();
        public TftpClientViewModel TftpClient => App.Current.Services.GetService<TftpClientViewModel>();
        public TcpServerViewModel TcpServer => App.Current.Services.GetService<TcpServerViewModel>();
        public JobCenterViewModel JobCenterViewModel => App.Current.Services.GetService<JobCenterViewModel>();

        #endregion

        #region 计量芯相关业务

        public SerialPortViewModel SerialPortViewModel => App.Current.Services.GetService<SerialPortViewModel>();

        public DlmsBaseMeterViewModel DlmsBaseMeterViewModel =>
            App.Current.Services.GetService<DlmsBaseMeterViewModel>();

        public UtilityTablesViewModel UtilityTablesViewModel =>
            App.Current.Services.GetService<UtilityTablesViewModel>();

        #endregion

        #region 智能仪表

        public FileTransmitViewModel FileTransmitViewModel =>
            App.Current.Services.GetService<FileTransmitViewModel>();

        #endregion

        #region IIC

        /// <summary>
        /// IIC数据视图模型
        /// </summary>
        public IicDataViewModel IicDataViewModel =>
            App.Current.Services.GetService<IicDataViewModel>();

        public CosemObjectViewModel CosemObjectViewModel => App.Current.Services.GetService<CosemObjectViewModel>();
        public MeterDataViewModel MeterDataViewModel => App.Current.Services.GetService<MeterDataViewModel>();

        #endregion

        public SinglePhaseManagementPageViewModel SinglePhaseManagementPageViewModel => App.Current.Services.GetService<SinglePhaseManagementPageViewModel>();

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}