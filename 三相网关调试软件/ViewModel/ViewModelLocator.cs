using System;
using Autofac;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Extensions.DependencyInjection;
using 三相智慧能源网关调试软件.ViewModel.DlmsViewModels;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class ViewModelLocator
    {
//        protected void ConfigureServices(ServiceCollection services)
//        {
//            services.AddSingleton<DataViewModel>();
//            services.AddSingleton<DlmsSettingsViewModel>();
//            services.AddSingleton<DlmsClient>();
//        }

        public ViewModelLocator()
        {
//            //Microsoft
//            var serviceCollection = new ServiceCollection();
//            ConfigureServices(serviceCollection);
//            var ser = serviceCollection.BuildServiceProvider();

      
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            //注册服务
            {
                #region Dlms相关服务

                SimpleIoc.Default.Register<DlmsSettingsViewModel>();
                SimpleIoc.Default.Register<DlmsClient>(true);
                SimpleIoc.Default.Register<DataViewModel>();
                SimpleIoc.Default.Register<RegisterViewModel>();
                SimpleIoc.Default.Register<ProfileGenericViewModel>();
                SimpleIoc.Default.Register<ClockViewModel>();
                SimpleIoc.Default.Register<LoadIdentificationViewModel>();

                #endregion


                #region 主程序界面相关

                SimpleIoc.Default.Register<MainViewModel>(); //主窗体
                SimpleIoc.Default.Register<MenuViewModel>(); //菜单
                SimpleIoc.Default.Register<UserLoginViewModel>(); //用户登录
                SimpleIoc.Default.Register<ColorToolViewModel>(); //程序调色板，皮肤
                SimpleIoc.Default.Register<SkinViewModel>(true); //程序调色板，皮肤，开机直接应用
                SimpleIoc.Default.Register<SnackbarViewModel>();

                #endregion


                #region 服务中心相关业务

                SimpleIoc.Default.Register<TelnetViewModel>(); //网关调试登录Telnet客户端
                SimpleIoc.Default.Register<TcpServerViewModel>(); //网关调试登录Telnet客户端
                SimpleIoc.Default.Register<TftpServerViewModel>();
                SimpleIoc.Default.Register<TftpClientViewModel>();
                SimpleIoc.Default.Register<NetLogViewModel>();
                SimpleIoc.Default.Register<XMLLogViewModel>();
                SimpleIoc.Default.Register<SerialPortViewModel>(); //RS485串口
             
                #endregion

                #region 三相网关特有业务

                #region 三相网关计量芯相关业务

                SimpleIoc.Default.Register<DlmsBaseMeterViewModel>(); //基表DLMS协议
                SimpleIoc.Default.Register<FileTransmitViewModel>(); //计量芯升级
                SimpleIoc.Default.Register<IicDataViewModel>(); //IIC报文解析服务

                #endregion

                #region 三相网关智能仪表业务

                SimpleIoc.Default.Register<UtilityTablesViewModel>(); //泰昂设备

                #endregion

                #region 三相网关管理芯相关业务

                SimpleIoc.Default.Register<ENetClientHelper>(); //网关登录使用的ENet客户端
                SimpleIoc.Default.Register<ENetMessageBuilderViewModel>();

                #endregion

                #endregion


                SimpleIoc.Default.Register<CosemObjectViewModel>();

                SimpleIoc.Default.Register<DialogsViewModel>();
            }
        }


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

        #endregion

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}