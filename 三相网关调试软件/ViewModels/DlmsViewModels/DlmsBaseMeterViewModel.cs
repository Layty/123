using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.CosemObjects;
using MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage;
using MyDlmsStandard.Axdr;
using MySerialPortMaster;

namespace 三相智慧能源网关调试软件.ViewModels.DlmsViewModels
{
    public class DlmsBaseMeterViewModel : ObservableObject
    {
        #region 属性

        private string _factoryStatus;

        /// <summary>
        /// 工厂状态字
        /// </summary>
        public string FactoryStatus
        {
            get => _factoryStatus;
            set
            {
                _factoryStatus = value;
                OnPropertyChanged();
            }
        }

        private string _softVersion;

        /// <summary>
        /// 软件版本
        /// </summary>
        public string SoftVersion
        {
            get => _softVersion;
            set
            {
                _softVersion = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public DlmsClient Client { get; set; }


        public DlmsBaseMeterViewModel(SerialPortMaster serialPortMaster)
        {
            //这里只用来打日志？
            var serialPortMaster1 = serialPortMaster;
            Client = App.Current.Services.GetService<DlmsClient>();



            InitCommand = new AsyncRelayCommand(async () => { await Client.InitRequest(); });
            DisconnectCommand = new AsyncRelayCommand(async () => { await Client.ReleaseRequest(); });
            GetSoftVersionCommand = new AsyncRelayCommand(async () =>
            {
                var cosem = new CosemData("1.0.0.2.0.255");
                var response = await Client.GetRequestAndWaitResponse(cosem.ValueAttributeDescriptor);
                if (response?.GetResponseNormal.Result.Data != null)
                {
                    SoftVersion = response.GetResponseNormal.Result.Data.ValueString;
                }
            });
            ReadFactoryCommand = new AsyncRelayCommand(async () =>
            {
                var cosem = new CosemData("0.0.96.5.0.255");
                var response = await Client.GetRequestAndWaitResponse(cosem.ValueAttributeDescriptor);
                if (response?.GetResponseNormal.Result.Data != null)
                {
                    FactoryStatus = response.GetResponseNormal.Result.Data.ValueString;
                }
            });
            EnterFactorCommand = new AsyncRelayCommand(async () =>
            {
                var cosem = new CosemData("0.0.96.5.0.255");
                DlmsDataItem dataItem = new DlmsDataItem(DataType.UInt16, "2000"); //8192
                await Client.SetRequestAndWaitResponse(cosem.ValueAttributeDescriptor, dataItem);
            });
            QuitFactorCommand = new AsyncRelayCommand(async () =>
            {
                var cosem = new CosemData("0.0.96.5.0.255");
                var dataItem = new DlmsDataItem(DataType.UInt16, "0000");
                await Client.SetRequestAndWaitResponse(cosem.ValueAttributeDescriptor, dataItem);
            });
            EnterUpgradeModeCommand = new AsyncRelayCommand(async () =>
            {
                //  var msg = HdlcFrameMaker.SetEnterUpGradeMode(256); //写256
                await Client.SetEnterUpGradeMode();
            });
            SetCapturePeriodCommand = new AsyncRelayCommand(async () =>
            {
                var cosem = new CosemProfileGeneric("1.0.99.1.0.255")
                {
                    CapturePeriod = new AxdrIntegerUnsigned32("0000003C")//60s
                };
                var dlmsData = new DlmsDataItem(DataType.UInt32, cosem.CapturePeriod.Value);

                await Client.SetRequestAndWaitResponse(cosem.CapturePeriodAttributeDescriptor, dlmsData);
            });
            ClearAllCommand = new AsyncRelayCommand(async () =>
            {
                var cosem = new ScriptTable();
                var value = new DlmsDataItem(DataType.UInt16, "0001");
                await Client.ActionRequestAndWaitResponse(cosem.GetScriptExecuteCosemMethodDescriptor(),
                    value);
            });
            OneKeyStartCommand = new RelayCommand(async () =>
            {
                serialPortMaster1.SerialPortLogger.AddInfo("开始初始化");
                InitCommand.Execute(null);
                await Task.Delay(800);
                serialPortMaster1.SerialPortLogger.AddInfo("读工厂模式");
                ReadFactoryCommand.Execute(null);
                await Task.Delay(800);
                serialPortMaster1.SerialPortLogger.AddInfo("读软件版本");
                GetSoftVersionCommand.Execute(null);
                await Task.Delay(800);
                serialPortMaster1.SerialPortLogger.AddInfo("进入工厂模式");
                EnterFactorCommand.Execute(null);
                await Task.Delay(800);
                serialPortMaster1.SerialPortLogger.AddInfo("读工厂模式");
                ReadFactoryCommand.Execute(null);
                await Task.Delay(800);
                serialPortMaster1.SerialPortLogger.AddInfo("进入升级模式");
                EnterUpgradeModeCommand.Execute(null);
                await Task.Delay(800);
                serialPortMaster1.SerialPortLogger.AddInfo("请开始升级");
            });
            OneKeyLeaveCommand = new RelayCommand(async () =>
            {
                serialPortMaster1.SerialPortLogger.AddInfo("开始初始化");
                InitCommand.Execute(null);
                await Task.Delay(800);
                serialPortMaster1.SerialPortLogger.AddInfo("读软件版本");
                GetSoftVersionCommand.Execute(null);
                await Task.Delay(800);
                serialPortMaster1.SerialPortLogger.AddInfo("设置捕获时间60s");
                SetCapturePeriodCommand.Execute(null);
                await Task.Delay(800);
                serialPortMaster1.SerialPortLogger.AddInfo("退出工厂模式");
                QuitFactorCommand.Execute(null);
                await Task.Delay(800);
                serialPortMaster1.SerialPortLogger.AddInfo("读工厂模式");
                ReadFactoryCommand.Execute(null);
                await Task.Delay(800);
                serialPortMaster1.SerialPortLogger.AddInfo("断开连接");
                DisconnectCommand.Execute(null);
                await Task.Delay(800);
                serialPortMaster1.SerialPortLogger.AddInfo("完成");
            });
        }

        #region Command

        /// <summary>
        /// 初始化命令，根据是否使用21E,或者使用HDLC46进行初始化通信,包含SNRM,AARQ
        /// </summary>
        public AsyncRelayCommand InitCommand { get; set; }


        /// <summary>
        /// 断开连接命令
        /// </summary>
        public AsyncRelayCommand DisconnectCommand { get; set; }

        private AsyncRelayCommand _getSoftVersionCommand;

        /// <summary>
        /// 读软件版本
        /// </summary>
        public AsyncRelayCommand GetSoftVersionCommand
        {
            get => _getSoftVersionCommand;
            set
            {
                _getSoftVersionCommand = value;
                OnPropertyChanged();
            }
        }

        private AsyncRelayCommand _readFactoryCommand;

        /// <summary>
        /// 读工厂模式
        /// </summary>
        public AsyncRelayCommand ReadFactoryCommand
        {
            get => _readFactoryCommand;
            set
            {
                _readFactoryCommand = value;
                OnPropertyChanged();
            }
        }

        private AsyncRelayCommand _enterFactoryCommand;

        /// <summary>
        /// 进入工厂模式命令
        /// </summary>
        public AsyncRelayCommand EnterFactorCommand
        {
            get => _enterFactoryCommand;
            set
            {
                _enterFactoryCommand = value;
                OnPropertyChanged();
            }
        }

        private AsyncRelayCommand _quitFactoryCommand;

        /// <summary>
        /// 退出工厂模式命令
        /// </summary>
        public AsyncRelayCommand QuitFactorCommand
        {
            get => _quitFactoryCommand;
            set
            {
                _quitFactoryCommand = value;
                OnPropertyChanged();
            }
        }

        private AsyncRelayCommand _setCapturePeriodCommand;

        /// <summary>
        /// 设置捕获时间
        /// </summary>
        public AsyncRelayCommand SetCapturePeriodCommand
        {
            get => _setCapturePeriodCommand;
            set
            {
                _setCapturePeriodCommand = value;
                OnPropertyChanged();
            }
        }


        private AsyncRelayCommand _clearAllCommand;

        /// <summary>
        /// 总清命令
        /// </summary>
        public AsyncRelayCommand ClearAllCommand
        {
            get => _clearAllCommand;
            set
            {
                _clearAllCommand = value;
                OnPropertyChanged();
            }
        }

        private AsyncRelayCommand _enterUpgradeModeCommand;

        /// <summary>
        /// 进入升级模式命令
        /// </summary>
        public AsyncRelayCommand EnterUpgradeModeCommand
        {
            get => _enterUpgradeModeCommand;
            set
            {
                _enterUpgradeModeCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _oneKeyStartCommand;

        /// <summary>
        /// 自定义的一键执行指令
        /// </summary>
        public RelayCommand OneKeyStartCommand
        {
            get => _oneKeyStartCommand;
            set
            {
                _oneKeyStartCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _oneKeyLeaveCommand;

        /// <summary>
        /// 自定义的一键执行指令
        /// </summary>
        public RelayCommand OneKeyLeaveCommand
        {
            get => _oneKeyLeaveCommand;
            set
            {
                _oneKeyLeaveCommand = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}