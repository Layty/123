using System.Threading.Tasks;
using CommonServiceLocator;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.Action;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.CosemObjects;
using MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage;
using MyDlmsStandard.Axdr;

namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
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
        public EModeViewModel EModeViewModel { get; set; }

        private SerialPortViewModel SerialPortViewModel { get; set; }


        public DlmsBaseMeterViewModel()
        {
            SerialPortViewModel = ServiceLocator.Current.GetInstance<SerialPortViewModel>();

            EModeViewModel = new EModeViewModel(SerialPortViewModel.SerialPortMaster); //近红外

            Client = ServiceLocator.Current.GetInstance<DlmsClient>();
            EModeViewModel = Client.EModeViewModel;
            InitCommand = new RelayCommand(async () => { await Client.InitRequest(); });
            DisconnectCommand = new RelayCommand(async () => { await Client.ReleaseRequest(); });
            GetSoftVersionCommand = new RelayCommand(async () =>
            {
                var cosem = new CosemData("1.0.0.2.0.255");
                var response = await Client.GetRequestAndWaitResponse(cosem.GetValueAttributeDescriptor());
                if (response != null )
                {
                    SoftVersion = response.GetResponseNormal.Result.Data.ValueString;
                }
            });
            ReadFactoryCommand = new RelayCommand(async () =>
            {
                var cosem = new CosemData("0.0.96.5.0.255");
                var response = await Client.GetRequestAndWaitResponse(cosem.GetValueAttributeDescriptor());
                if (response != null)
                {
                    FactoryStatus = response.GetResponseNormal.Result.Data.ValueString;
                }
            });
            EnterFactorCommand = new RelayCommand(async () =>
            {
                var cosem = new CosemData("0.0.96.5.0.255");
                DlmsDataItem dataItem = new DlmsDataItem(DataType.UInt16, "2000");//8192
                await Client.SetRequestAndWaitResponse(cosem.GetValueAttributeDescriptor(), dataItem);
            });
            QuitFactorCommand = new RelayCommand(async () =>
            {
                var cosem = new CosemData("0.0.96.5.0.255");
                var dataItem = new DlmsDataItem(DataType.UInt16, "0000");
                await Client.SetRequestAndWaitResponse(cosem.GetValueAttributeDescriptor(), dataItem);
            });
            EnterUpgradeModeCommand = new RelayCommand(async () =>
            {
                //var msg = Hdlc46Executor.HdlcFrameMaker.SetEnterUpGradeMode(256); //写256
                await Client.SetEnterUpGradeMode();
            });
            SetCapturePeriodCommand = new RelayCommand(async () =>
            {
                var cosem = new CosemProfileGeneric("1.0.99.1.0.255");
                cosem.CapturePeriod = new AxdrIntegerUnsigned32("00000060");
                var dlmsData = new DlmsDataItem(DataType.UInt32, cosem.CapturePeriod.Value);

                await Client.SetRequestAndWaitResponse(cosem.GetCapturePeriodAttributeDescriptor(), dlmsData);
            });
            ClearAllCommand = new RelayCommand(async () =>
            {
                var cosem = new ScriptTable();
                Client.actionRequest = new ActionRequest()
                {
                    ActionRequestNormal = new ActionRequestNormal(cosem.GetScriptExecuteCosemMethodDescriptor(),
                        new DlmsDataItem(DataType.UInt16, "0001")
                    )
                };
                await Client.ActionRequest(Client.actionRequest);
            });
            OneKeyStartCommand = new RelayCommand(async () =>
            {
                SerialPortViewModel.SerialPortMaster.SerialPortLogger.AddInfo("开始搞事情");
                InitCommand.Execute(null);
                await Task.Delay(800);
                SerialPortViewModel.SerialPortMaster.SerialPortLogger.AddInfo("读工厂模式");
                ReadFactoryCommand.Execute(null);
                await Task.Delay(800);
                SerialPortViewModel.SerialPortMaster.SerialPortLogger.AddInfo("读软件版本");
                GetSoftVersionCommand.Execute(null);
                await Task.Delay(800);
                SerialPortViewModel.SerialPortMaster.SerialPortLogger.AddInfo("进入工厂模式");
                EnterFactorCommand.Execute(null);
                await Task.Delay(800);
                SerialPortViewModel.SerialPortMaster.SerialPortLogger.AddInfo("读工厂模式");
                ReadFactoryCommand.Execute(null);
                await Task.Delay(800);
                SerialPortViewModel.SerialPortMaster.SerialPortLogger.AddInfo("进入升级模式");
                EnterUpgradeModeCommand.Execute(null);
                await Task.Delay(800);
                SerialPortViewModel.SerialPortMaster.SerialPortLogger.AddInfo("请开始你的表演");
            });
            OneKeyLeaveCommand = new RelayCommand(async () =>
            {
                SerialPortViewModel.SerialPortMaster.SerialPortLogger.AddInfo("开始收拾");
                InitCommand.Execute(null);
                await Task.Delay(800);
                SerialPortViewModel.SerialPortMaster.SerialPortLogger.AddInfo("读软件版本");
                GetSoftVersionCommand.Execute(null);
                await Task.Delay(800);
                SerialPortViewModel.SerialPortMaster.SerialPortLogger.AddInfo("设置捕获时间60s");
                SetCapturePeriodCommand.Execute(null);
                await Task.Delay(800);
                SerialPortViewModel.SerialPortMaster.SerialPortLogger.AddInfo("退出工厂模式");
                QuitFactorCommand.Execute(null);
                await Task.Delay(800);
                SerialPortViewModel.SerialPortMaster.SerialPortLogger.AddInfo("读工厂模式");
                ReadFactoryCommand.Execute(null);
                await Task.Delay(800);
                SerialPortViewModel.SerialPortMaster.SerialPortLogger.AddInfo("事了拂袖去");
                DisconnectCommand.Execute(null);
                await Task.Delay(800);
                SerialPortViewModel.SerialPortMaster.SerialPortLogger.AddInfo("深藏功与名");
            });
        }

        #region Command



        /// <summary>
        /// 初始化命令，根据是否使用21E,或者使用HDLC46进行初始化通信,包含SNRM,AARQ
        /// </summary>
        public RelayCommand InitCommand { get; set; }
   


        /// <summary>
        /// 断开连接命令
        /// </summary>
        public RelayCommand DisconnectCommand { get; set; }

        private RelayCommand _getSoftVersionCommand;

        /// <summary>
        /// 读软件版本
        /// </summary>
        public RelayCommand GetSoftVersionCommand
        {
            get => _getSoftVersionCommand;
            set
            {
                _getSoftVersionCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _readFactoryCommand;

        /// <summary>
        /// 读工厂模式
        /// </summary>
        public RelayCommand ReadFactoryCommand
        {
            get => _readFactoryCommand;
            set
            {
                _readFactoryCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _enterFactoryCommand;

        /// <summary>
        /// 进入工厂模式命令
        /// </summary>
        public RelayCommand EnterFactorCommand
        {
            get => _enterFactoryCommand;
            set
            {
                _enterFactoryCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _quitFactoryCommand;

        /// <summary>
        /// 退出工厂模式命令
        /// </summary>
        public RelayCommand QuitFactorCommand
        {
            get => _quitFactoryCommand;
            set
            {
                _quitFactoryCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _setCapturePeriodCommand;

        /// <summary>
        /// 设置捕获时间
        /// </summary>
        public RelayCommand SetCapturePeriodCommand
        {
            get => _setCapturePeriodCommand;
            set
            {
                _setCapturePeriodCommand = value;
                OnPropertyChanged();
            }
        }


        private RelayCommand _clearAllCommand;

        /// <summary>
        /// 总清命令
        /// </summary>
        public RelayCommand ClearAllCommand
        {
            get => _clearAllCommand;
            set
            {
                _clearAllCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _enterUpgradeModeCommand;

        /// <summary>
        /// 进入升级模式命令
        /// </summary>
        public RelayCommand EnterUpgradeModeCommand
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