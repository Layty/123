using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using 三相智慧能源网关调试软件.Helpers;
using 三相智慧能源网关调试软件.ViewModels;

namespace 三相智慧能源网关调试软件.Views.Management
{
    public class SinglePhaseManagementPageViewModel : ObservableObject
    {
        public string SoftVersion
        {
            get => _softVersion;
            set => SetProperty(ref _softVersion, value);
        }

        private string _softVersion;

        public RelayCommand GetSoftVersionCommand
        {
            get => _getSoftVersionCommand;
            set => SetProperty(ref _getSoftVersionCommand, value);
        }

        private RelayCommand _getSoftVersionCommand;

        private RelayCommand _openV114Command;

        public RelayCommand OpenV114Command
        {
            get => _openV114Command;
            set => SetProperty(ref _openV114Command, value);
        }

        private RelayCommand _openV1021Command;

        public RelayCommand OpenV1021Command
        {
            get => _openV1021Command;
            set => SetProperty(ref _openV1021Command, value);
        }
        public SinglePhaseManagementPageViewModel(SerialPortViewModel serialPort)
        {
            GetSoftVersionCommand = new RelayCommand(async () =>
            {
                SoftVersion = "";
                var connect = SinglePhaseGatewayManagementHelper.GetConnect();
                var returnConnect = await serialPort.SerialPortMaster.SendAndReceiveReturnDataAsync(connect);
                if (SinglePhaseGatewayManagementHelper.ParseConnectResult(returnConnect))
                {
                    var getVersion = SinglePhaseGatewayManagementHelper.GetVersion();
                    var returnVersion = await serialPort.SerialPortMaster.SendAndReceiveReturnDataAsync(getVersion);


                    SoftVersion = SinglePhaseGatewayManagementHelper.ParseVersion(returnVersion);
                }
            });
            OpenV114Command = new RelayCommand(() =>
            {
                Process.Start("智慧能源网关升级工具_V1.1.4.exe");
            });
            OpenV1021Command = new RelayCommand(() =>
            {
                Process.Start("智慧能源网关升级工具_V1.0.21.exe");
            });
        }
    }
}