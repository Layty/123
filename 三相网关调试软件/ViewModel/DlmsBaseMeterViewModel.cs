using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using 三相智慧能源网关调试软件.DLMS;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.Get;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.Set;
using 三相智慧能源网关调试软件.DLMS.HDLC.Enums;
using 三相智慧能源网关调试软件.DLMS.HDLC.IEC21EMode;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class DlmsBaseMeterViewModel : ViewModelBase
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
                RaisePropertyChanged();
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
                RaisePropertyChanged();
            }
        }

        #endregion

       
        public DLMSClient Client { get; set; }
        public EModeExecutor EModeExecutor { get; set; }

        public SerialPortViewModel SerialPortViewModel { get; set; }

      


        public DlmsBaseMeterViewModel()
        {
            SerialPortViewModel = ServiceLocator.Current.GetInstance<SerialPortViewModel>();

            EModeExecutor = new EModeExecutor(SerialPortViewModel.SerialPortMaster, ""); //近红外

            Client = ServiceLocator.Current.GetInstance<DLMSClient>();

            InitCommand = new RelayCommand(async () => { await Client.InitRequestNew();});
            DisconnectCommand = new RelayCommand(async () => { await Client.DisconnectRequest(true); });
            GetSoftVersionCommand = new RelayCommand(async () =>
            {
                var cosem = new DLMSData("1.0.0.2.0.255");
                GetRequest getRequest=new GetRequest();
                getRequest.GetRequestNormal = new GetRequestNormal(cosem.GetValueAttributeDescriptor());
                var value = await Client.GetRequest(getRequest);
                if (value != null && value.Length != 0)
                {
                    var data = NormalDataParse.ParsePduData(value);
                    SoftVersion = data;
                }
            });
            ReadFactoryCommand = new RelayCommand(async () =>
            {
                var cosem = new DLMSData("0.0.96.5.0.255");
                GetRequest getRequest = new GetRequest();
                getRequest.GetRequestNormal = new GetRequestNormal(cosem.GetValueAttributeDescriptor());
                var value = await Client.GetRequest(getRequest);
                if (value != null && value.Length != 0)
                {
                    FactoryStatus = NormalDataParse.ParsePduData(value);
                }
            });
            EnterFactorCommand = new RelayCommand(async () =>
            {
                var cosem = new DLMSData("0.0.96.5.0.255");
                DLMSDataItem dataItem = new DLMSDataItem(DataType.UInt16, "8192");
                SetRequest setRequest = new SetRequest();
                setRequest.SetRequestNormal = new SetRequestNormal(cosem.GetCosemAttributeDescriptor(2), dataItem);
                await Client.SetRequest(setRequest);
            });
            QuitFactorCommand = new RelayCommand(async () =>
            {
                var cosem = new DLMSData("0.0.96.5.0.255");
                var dataItem = new DLMSDataItem(DataType.UInt16, "0");
                SetRequest setRequest = new SetRequest();
                setRequest.SetRequestNormal = new SetRequestNormal(cosem.GetValueAttributeDescriptor(), dataItem);
                await Client.SetRequest(setRequest);
            });
            EnterUpgradeModeCommand = new RelayCommand(async () =>
            {
                //var msg = Hdlc46Executor.HdlcFrameMaker.SetEnterUpGradeMode(256); //写256
                await Client.SetEnterUpGradeMode();
            });
            SetCapturePeriodCommand = new RelayCommand(async () =>
            {
                var cosem = new ProfileGeneric("1.0.99.1.0.255");
                await Client.SetRequest(cosem.SetCapturePeriod(60));
            });
            ClearAllCommand = new RelayCommand(async () =>
            {
                var cosem = new ScriptTable();
                await Client.ActionRequest(cosem.ScriptExecute(1));
            });
            OneKeyStartCommand = new RelayCommand(async () =>
            {
                SerialPortViewModel.SerialPortMaster.SerialPortLogger.AddInfo("开始搞事情");
                InitCommand.Execute(null);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortMaster.SerialPortLogger.AddInfo("读工厂模式");
                ReadFactoryCommand.Execute(null);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortMaster.SerialPortLogger.AddInfo("读软件版本");
                GetSoftVersionCommand.Execute(null);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortMaster.SerialPortLogger.AddInfo("进入工厂模式");
                EnterFactorCommand.Execute(null);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortMaster.SerialPortLogger.AddInfo("读工厂模式");
                ReadFactoryCommand.Execute(null);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortMaster.SerialPortLogger.AddInfo("进入升级模式");
                EnterUpgradeModeCommand.Execute(null);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortMaster.SerialPortLogger.AddInfo("请开始你的表演");
            });
            OneKeyLeaveCommand = new RelayCommand(async () =>
            {
                SerialPortViewModel.SerialPortMaster.SerialPortLogger.AddInfo("开始收拾");
                InitCommand.Execute(null);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortMaster.SerialPortLogger.AddInfo("读软件版本");
                GetSoftVersionCommand.Execute(null);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortMaster.SerialPortLogger.AddInfo("设置捕获时间60s");
                SetCapturePeriodCommand.Execute(null);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortMaster.SerialPortLogger.AddInfo("退出工厂模式");
                QuitFactorCommand.Execute(null);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortMaster.SerialPortLogger.AddInfo("读工厂模式");
                ReadFactoryCommand.Execute(null);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortMaster.SerialPortLogger.AddInfo("事了拂袖去");
                DisconnectCommand.Execute(null);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortMaster.SerialPortLogger.AddInfo("深藏功与名");
            });
        }


//        private async void Init()
//        {
//            try
//            {
//                if (StartProtocolType == StartProtocolType.IEC21E)
//                {
//                    if (await EModeExecutor.Execute21ENegotiate())
//                    {
////                        var t = Hdlc46Executor.ExecuteHdlcSNRMRequest();
//                        var t = Client.SNRMRequest();
//                        await t.ContinueWith(
//                            t1 =>
//                            {
//                                return Client.AarqRequest();
//                            },
//                            TaskContinuationOptions.OnlyOnRanToCompletion);
//                    }
//                }

//            }
//            catch (Exception exception)
//            {
//                MessageBox.Show(exception.Message);
//            }
//        }


        #region Command

        private RelayCommand _initCommand;

        /// <summary>
        /// 初始化命令，根据是否使用21E,或者使用HDLC46进行初始化通信,包含SNRM,AARQ
        /// </summary>
        public RelayCommand InitCommand
        {
            get => _initCommand;
            set
            {
                _initCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _disconnectCommand;

        /// <summary>
        /// 断开连接命令
        /// </summary>
        public RelayCommand DisconnectCommand
        {
            get => _disconnectCommand;
            set
            {
                _disconnectCommand = value;
                RaisePropertyChanged();
            }
        }

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
                RaisePropertyChanged();
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
                RaisePropertyChanged();
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
                RaisePropertyChanged();
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
                RaisePropertyChanged();
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
                RaisePropertyChanged();
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
                RaisePropertyChanged();
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
                RaisePropertyChanged();
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
                RaisePropertyChanged();
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
                RaisePropertyChanged();
            }
        }

        #endregion
    }
}