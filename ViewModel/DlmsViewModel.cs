using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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
        private bool _isUse21E;

        public bool IsUse21E
        {
            get { return _isUse21E; }
            set
            {
                _isUse21E = value;
                RaisePropertyChanged();
            }
        }

        public EModeExecutor EModeExecutor { get; set; }

        public Hdlc46Executor Hdlc46Executor { get; set; }
        public HdlcFrameMaker HdlcFrameMaker { get; set; }
        public SerialPortViewModel SerialPortViewModel { get; set; }

        public DlmsViewModel()
        {
            SerialPortViewModel = CommonServiceLocator.ServiceLocator.Current.GetInstance<SerialPortViewModel>();

            EModeExecutor = new EModeExecutor(SerialPortViewModel.SerialPortModel, ""); //近红外
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
            Hdlc46Executor = new Hdlc46Executor(SerialPortViewModel.SerialPortModel, HdlcFrameMaker);

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
                if (value != null&&value.Length!=0)
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
                await SerialPortViewModel.SerialPortModel.SendAndReceiveReturnDataAsync(msg);
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
                SerialPortViewModel.SerialPortModel.SendAndReceiveDataCollections = "开始搞事情";
                //ButtonInit_OnClick(sender, e);
                InitCommand.Execute(null);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortModel.SendAndReceiveDataCollections = "读工厂模式";
                ReadFactoryCommand.Execute(null);
                //    ButtonReadFactory_OnClick(sender, e);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortModel.SendAndReceiveDataCollections = "读软件版本";
                //   ButtonGetSoftVersion_OnClick(sender, e);
                GetSoftVersionCommand.Execute(null);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortModel.SendAndReceiveDataCollections = "进入工厂模式";
                // ButtonEnterFactory_OnClick(sender, e);
                EnterFactorCommand.Execute(null);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortModel.SendAndReceiveDataCollections = "读工厂模式";

                ReadFactoryCommand.Execute(null);
                //ButtonReadFactory_OnClick(sender, e);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortModel.SendAndReceiveDataCollections = "进入升级模式";
                // ButtonEnterUpgradeMode_OnClick(sender, e);
                EnterUpgradeModeCommand.Execute(null);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortModel.SendAndReceiveDataCollections = "请开始你的表演";
            });
            OneKeyLeaveCommand = new RelayCommand(async () =>
            {
                SerialPortViewModel.SerialPortModel.SendAndReceiveDataCollections = "开始收拾";
                InitCommand.Execute(null);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortModel.SendAndReceiveDataCollections = "读软件版本";
                GetSoftVersionCommand.Execute(null);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortModel.SendAndReceiveDataCollections = "设置捕获时间60s";
                SetCapturePeriodCommand.Execute(null);
                //  ButtonSetCapturePeriod_OnClick(sender, e);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortModel.SendAndReceiveDataCollections = "退出工厂模式";
                QuitFactorCommand.Execute(null);
                //  ButtonQuitFactory_OnClick(sender, e);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortModel.SendAndReceiveDataCollections = "读工厂模式";
                ReadFactoryCommand.Execute(null);
                // ButtonReadFactory_OnClick(sender, e);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortModel.SendAndReceiveDataCollections = "事了拂袖去";
                //  ButtonDisConnect_OnClick(sender, e);
                DisconnectCommand.Execute(null);
                await Task.Delay(500);
                SerialPortViewModel.SerialPortModel.SendAndReceiveDataCollections = "深藏功与名";
            });
        }

        private RelayCommand _initCommand;

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

        public RelayCommand DisconnectCommand
        {
            get { return _disconnectCommand; }
            set
            {
                _disconnectCommand = value;
                RaisePropertyChanged();
            }
        }

        private async void Init()
        {
            try
            {
                if (IsUse21E)
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

        private RelayCommand _oneKeyStartCommand;

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

        public RelayCommand OneKeyLeaveCommand
        {
            get => _oneKeyLeaveCommand;
            set
            {
                _oneKeyLeaveCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _enterUpgradeModeCommand;

        public RelayCommand EnterUpgradeModeCommand
        {
            get => _enterUpgradeModeCommand;
            set
            {
                _enterUpgradeModeCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _enterFactoryCommand;

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

        public RelayCommand QuitFactorCommand
        {
            get { return _quitFactoryCommand; }
            set
            {
                _quitFactoryCommand = value;
                RaisePropertyChanged();
            }
        }

        private string _factoryStatus;

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

        public string SoftVersion
        {
            get { return _softVersion; }
            set
            {
                _softVersion = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _getSoftVersionCommand;

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

        public RelayCommand ReadFactoryCommand
        {
            get { return _readFactoryCommand; }
            set
            {
                _readFactoryCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _setCapturePeriodCommand;

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

        public RelayCommand ClearAllCommand
        {
            get { return _clearAllCommand; }
            set
            {
                _clearAllCommand = value;
                RaisePropertyChanged();
            }
        }
    }
}