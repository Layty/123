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