using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS;
using 三相智慧能源网关调试软件.DLMS._21EMode;
using 三相智慧能源网关调试软件.DLMS.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.CosemObjects;
using 三相智慧能源网关调试软件.DLMS.HDLC;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class DlmsViewModel : ViewModelBase
    {
        #region 属性

        private bool _isUse21E;

        /// <summary>
        /// 是否使用21E模式开启通信
        /// </summary>
        public bool IsUse21E
        {
            get { return _isUse21E; }
            set
            {
                _isUse21E = value;
                RaisePropertyChanged();
            }
        }

        private string _factoryStatus;

        /// <summary>
        /// 工厂状态字
        /// </summary>
        public string FactoryStatus
        {
            get { return _factoryStatus; }
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
            get { return _softVersion; }
            set
            {
                _softVersion = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        public EModeExecutor EModeExecutor { get; set; }

        public Hdlc46Executor Hdlc46Executor { get; set; }
        public HdlcFrameMaker HdlcFrameMaker { get; set; }
        public SerialPortViewModel SerialPortViewModel { get; set; }
        private StartProtocolType _startProtocolType = StartProtocolType.DLMS;

        public StartProtocolType StartProtocolType
        {
            get => _startProtocolType;
            set
            {
                _startProtocolType = value;
                RaisePropertyChanged();
            }
        }

   

        public Array StartProtocolArray => Enum.GetValues(typeof(StartProtocolType));

        public DlmsViewModel()
        {
            SerialPortViewModel = ServiceLocator.Current.GetInstance<SerialPortViewModel>();

            EModeExecutor = new EModeExecutor(SerialPortViewModel.SerialPortMasterModel, ""); //近红外
            //HdlcFrameMaker = new HdlcFrameMaker(new byte[]
            //{
            //    0,
            //    2,
            //    0,
            //    33
            //}, "33333333", 1);
            HdlcFrameMaker = new HdlcFrameMaker(new byte[]
            {
                3
            }, "33333333", 1);
            HdlcFrameMaker.Hdlc46Frame.MaxReceivePduSize = 65535;
            Hdlc46Executor = new Hdlc46Executor(SerialPortViewModel.SerialPortMasterModel, HdlcFrameMaker);

            InitCommand = new RelayCommand(Init);
            DisconnectCommand = new RelayCommand(() => { Hdlc46Executor.ExecuteHdlcDisConnectRequest(); });
            GetSoftVersionCommand = new RelayCommand(async () =>
            {
                var cosem = new DLMSData("1.0.0.2.0.255");
                var value = await cosem.GetValue();
                if (value != null && value.Length != 0)
                {
                    var data = NormalDataParse.GetDataContent(value, 3, out bool result);
                    if (result)
                    {
                        SoftVersion = data.ByteToString();
                    }
                    else
                    {
                        SoftVersion = "";
                    }
                }
            });
            ReadFactoryCommand = new RelayCommand(async () =>
            {
                var cosem = new DLMSData("0.0.96.5.0.255");
                var value = await cosem.GetValue();
                if (value != null && value.Length != 0)
                {
                    var data = NormalDataParse.GetDataFactoryContent(value, 3, out bool result);
                    if (result)
                    {
                        FactoryStatus = BitConverter.ToUInt16(data.Reverse().ToArray(), 0).ToString();
                    }
                    else
                    {
                        FactoryStatus = "";
                    }
                }
            });
            EnterFactorCommand = new RelayCommand(() =>
            {
                var cosem = new DLMSData("0.0.96.5.0.255");
                byte[] inputBytes = BitConverter.GetBytes(short.Parse("8192")).Reverse().ToArray();
                DLMSDataItem dataItem = new DLMSDataItem(DataType.UInt16, inputBytes);
                cosem.SetValue(dataItem);
            });
            QuitFactorCommand = new RelayCommand(() =>
            {
                var cosem = new DLMSData("0.0.96.5.0.255");
                byte[] inputDate = BitConverter.GetBytes(short.Parse("0")).Reverse().ToArray();
                var dateItem = new DLMSDataItem(DataType.UInt16, inputDate);
                cosem.SetValue(dateItem);
            });
            EnterUpgradeModeCommand = new RelayCommand(async () =>
            {
                var msg = HdlcFrameMaker.SetEnterUpGradeMode(256); //写256
                await SerialPortViewModel.SerialPortMasterModel.SendAndReceiveReturnDataAsync(msg);
            });
            SetCapturePeriodCommand = new RelayCommand(() =>
            {
                var cosem = new ProfileGeneric("1.0.99.1.0.255");
                cosem.SetCapturePeriod(60);
            });
            ClearAllCommand = new RelayCommand(() =>
            {
                var cosem = new ScriptTable();
                cosem.ScriptExecute(1);
            });
            OneKeyStartCommand = new RelayCommand(async () =>
            {
                SerialPortViewModel.SerialPortMasterModel.SendAndReceiveDataCollections = "开始搞事情";
                InitCommand.Execute(null);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortMasterModel.SendAndReceiveDataCollections = "读工厂模式";
                ReadFactoryCommand.Execute(null);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortMasterModel.SendAndReceiveDataCollections = "读软件版本";
                GetSoftVersionCommand.Execute(null);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortMasterModel.SendAndReceiveDataCollections = "进入工厂模式";
                EnterFactorCommand.Execute(null);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortMasterModel.SendAndReceiveDataCollections = "读工厂模式";
                ReadFactoryCommand.Execute(null);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortMasterModel.SendAndReceiveDataCollections = "进入升级模式";
                EnterUpgradeModeCommand.Execute(null);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortMasterModel.SendAndReceiveDataCollections = "请开始你的表演";
            });
            OneKeyLeaveCommand = new RelayCommand(async () =>
            {
                SerialPortViewModel.SerialPortMasterModel.SendAndReceiveDataCollections = "开始收拾";
                InitCommand.Execute(null);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortMasterModel.SendAndReceiveDataCollections = "读软件版本";
                GetSoftVersionCommand.Execute(null);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortMasterModel.SendAndReceiveDataCollections = "设置捕获时间60s";
                SetCapturePeriodCommand.Execute(null);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortMasterModel.SendAndReceiveDataCollections = "退出工厂模式";
                QuitFactorCommand.Execute(null);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortMasterModel.SendAndReceiveDataCollections = "读工厂模式";
                ReadFactoryCommand.Execute(null);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortMasterModel.SendAndReceiveDataCollections = "事了拂袖去";
                DisconnectCommand.Execute(null);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortMasterModel.SendAndReceiveDataCollections = "深藏功与名";
            });
        }


        private async void Init()
        {
            try
            {
                if (StartProtocolType == StartProtocolType.IEC)
                {
                    if (await EModeExecutor.Execute())
                    {
                        var t = Hdlc46Executor.ExecuteHdlcSNRMRequest();
                        await t.ContinueWith(
                            t1 =>
                            {
                                if (!t.Result)
                                {
                                    return null;
                                }

                                return Hdlc46Executor.ExecuteHdlcComm(HdlcFrameMaker.AarqRequest);
                            },
                            TaskContinuationOptions.OnlyOnRanToCompletion);
                    }
                }

                else
                {
                    var t = Hdlc46Executor.ExecuteHdlcSNRMRequest();
                    await t.ContinueWith(
                        t1 =>
                        {
                            if (!t.Result)
                            {
                                return null;
                            }

                            return Hdlc46Executor.ExecuteHdlcComm(HdlcFrameMaker.AarqRequest);
                        },
                        TaskContinuationOptions.OnlyOnRanToCompletion);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }


        #region Command

        private RelayCommand _initCommand;

        /// <summary>
        /// 初始化命令，根据是否使用21E,或者使用HDLC46进行初始化通信,包含SNRM,AARQ
        /// </summary>
        public RelayCommand InitCommand
        {
            get { return _initCommand; }
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
            get { return _disconnectCommand; }
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
            get { return _getSoftVersionCommand; }
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
            get { return _readFactoryCommand; }
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
            get { return _enterFactoryCommand; }
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
            get { return _quitFactoryCommand; }
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
            get { return _setCapturePeriodCommand; }
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
            get { return _clearAllCommand; }
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