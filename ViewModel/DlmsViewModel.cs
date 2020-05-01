using System;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using 三相智慧能源网关调试软件.DLMS._21EMode;
using 三相智慧能源网关调试软件.DLMS.HDLC;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class DlmsViewModel : ViewModelBase
    {
        private bool _isUse21E;

        public bool IsUse21E
        {
            get { return _isUse21E; }
            set { _isUse21E = value; RaisePropertyChanged(); }
        }

        public Hdlc46Executor DExecutor { get; set; }
        public HdlcFrameMaker HdlcFrameMaker { get; set; }
        public SerialPortViewModel SerialPortViewModel { get; set; }

        public DlmsViewModel()
        {
            SerialPortViewModel = CommonServiceLocator.ServiceLocator.Current.GetInstance<SerialPortViewModel>();
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

            DExecutor = new Hdlc46Executor(SerialPortViewModel.SerialPortModel, HdlcFrameMaker);
            InitCommand=new RelayCommand(Init);
            DisconnectCommand=new RelayCommand(() =>
            {
                DExecutor.ExecuteHdlcDisConnectRequest();
            });
        }

        private RelayCommand _initCommand;

        public RelayCommand InitCommand
        {
            get { return _initCommand; }
            set { _initCommand = value; RaisePropertyChanged(); }
        }
        private RelayCommand _disconnectCommand;

        public RelayCommand DisconnectCommand
        {
            get { return _disconnectCommand; }
            set { _disconnectCommand = value; RaisePropertyChanged(); }
        }

        private async void Init()
        {
            try
            {
                if (IsUse21E)
                {
                    EModeExecutor eModeExecutor = new EModeExecutor(SerialPortViewModel.SerialPortModel, "");
                    if (await eModeExecutor.Execute())
                    {
                        var t = DExecutor.ExecuteHdlcSNRMRequest();
                        await t.ContinueWith(
                            t1 =>
                            {
                                if (!t.Result)
                                {
                                    return null;
                                }

                                return DExecutor.ExecuteHdlcComm(HdlcFrameMaker.AarqRequest);
                            },
                            TaskContinuationOptions.OnlyOnRanToCompletion);
                    }
                }
                else
                {
                    var t = DExecutor.ExecuteHdlcSNRMRequest();

                    //   Task t = DlmsViewModel.DExecutor.ExecuteHdlcSNRMRequest();
                    await t.ContinueWith(
                        t1 =>
                        {
                            if (!t.Result)
                            {
                                return null;
                            }

                            return DExecutor.ExecuteHdlcComm(HdlcFrameMaker.AarqRequest);
                        },
                        TaskContinuationOptions.OnlyOnRanToCompletion);
                }

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

    }
}