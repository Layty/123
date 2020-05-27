using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using 三相智慧能源网关调试软件.Properties;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class TelnetViewModel : ViewModelBase
    {
        #region 网络

        private RelayCommand _connectOrDisconnectCommand;

        public RelayCommand ConnectOrDisconnectCommand
        {
            get => _connectOrDisconnectCommand;
            set
            {
                _connectOrDisconnectCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand<string> _sendMsgCommand;

        public RelayCommand<string> SendMsgCommand
        {
            get => _sendMsgCommand;
            set
            {
                _sendMsgCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _sendMsgControlCCommand;

        public RelayCommand SendMsgControlCCommand
        {
            get => _sendMsgControlCCommand;
            set
            {
                _sendMsgControlCCommand = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region 网关参数配置业务

        private string _afterIp = "192.168.0.145";

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression("^((2(5[0-5]|[0-4]\\d))|[0-1]?\\d{1,2})(\\.((2(5[0-5]|[0-4]\\d))|[0-1]?\\d{1,2})){3}$$",
            ErrorMessage = "请输入正确的IP地址！")]
        public string AfterIp
        {
            get => _afterIp;
            set
            {
                _afterIp = value;
                RaisePropertyChanged();
            }
        }


        private string _afterGateway = "192.168.0.1";

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression("^((2(5[0-5]|[0-4]\\d))|[0-1]?\\d{1,2})(\\.((2(5[0-5]|[0-4]\\d))|[0-1]?\\d{1,2})){3}$$",
            ErrorMessage = "请输入正确的IP地址！")]
        public string AfterGateway
        {
            get => _afterGateway;
            set
            {
                _afterGateway = value;
                RaisePropertyChanged();
            }
        }

        private string _afterHostIp = "172.32.0.3";

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression("^((2(5[0-5]|[0-4]\\d))|[0-1]?\\d{1,2})(\\.((2(5[0-5]|[0-4]\\d))|[0-1]?\\d{1,2})){3}$$",
            ErrorMessage = "请输入正确的IP地址！")]
        public string AfterHostIp
        {
            get => _afterHostIp;
            set
            {
                _afterHostIp = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _tryToLoginCommand;

        public RelayCommand TryToLoginCommand
        {
            get => _tryToLoginCommand;
            set
            {
                _tryToLoginCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _replaceAppParaHostIpCommand;

        public RelayCommand ReplaceAppParaHostIpCommand
        {
            get => _replaceAppParaHostIpCommand;
            set
            {
                _replaceAppParaHostIpCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _replaceAppParaIpAddrCommand;

        public RelayCommand ReplaceAppParaIpAddrCommand
        {
            get => _replaceAppParaIpAddrCommand;
            set
            {
                _replaceAppParaIpAddrCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _replaceAppParaIpGateWayCommand;

        public RelayCommand ReplaceAppParaIpGateWayCommand
        {
            get => _replaceAppParaIpGateWayCommand;
            set
            {
                _replaceAppParaIpGateWayCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _replaceAllParaCommand;

        public RelayCommand ReplaceAllParaCommand
        {
            get => _replaceAllParaCommand;
            set
            {
                _replaceAllParaCommand = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        public TcpClientHelper TcpClientHelper
        {
            get => _tcpClientHelper;
            set
            {
                _tcpClientHelper = value;
                RaisePropertyChanged();
            }
        }

        private TcpClientHelper _tcpClientHelper;

        public TelnetViewModel()
        {
            TcpClientHelper = new TcpClientHelper(Settings.Default.GatewayIpAddress, 23);

            ConnectOrDisconnectCommand = new RelayCommand(() =>
            {
                if (TcpClientHelper.ClientSocket.Connected)
                {
                    TcpClientHelper.Disconnect();
                }
                else
                {
                    TcpClientHelper.ConnectToServer();
                }
            });
            SendMsgCommand = new RelayCommand<string>(TcpClientHelper.SendDataToServerWithNewLine);
            SendMsgControlCCommand = new RelayCommand(() => { TcpClientHelper.SendDataToServer(new byte[] {0x03}); });
            TryToLoginCommand = new RelayCommand((async () =>
            {
                TcpClientHelper.SendDataToServerWithNewLine("root");
                await Task.Delay(500);
                TcpClientHelper.SendDataToServerWithNewLine("11223344");
            }));
            //主站IP
            ReplaceAppParaHostIpCommand = new RelayCommand(() =>
            {
                string data =
                    $"sed -i 's/^\\[HostStationIp:.*/\\[HostStationIp:{AfterHostIp}\\]/' /opt/cfg/AppPara.cfg";
                TcpClientHelper.SendDataToServerWithNewLine(data);
            });
            //网关IP
            ReplaceAppParaIpAddrCommand = new RelayCommand(() =>
            {
                string data = $"sed -i 's/^\\[IpAddr:.*/\\[IpAddr:{AfterIp}\\]/' /opt/cfg/AppPara.cfg";
                TcpClientHelper.SendDataToServerWithNewLine(data);
            });
            //网关默认网关
            ReplaceAppParaIpGateWayCommand = new RelayCommand(() =>
            {
                string data =
                    $"sed -i 's/^\\[Gateway:.*/\\[Gateway:{AfterGateway}\\]/'  /opt/cfg/AppPara.cfg";
                TcpClientHelper.SendDataToServerWithNewLine(data);
            });
            //主站IP/网关IP/网关默认网关
            ReplaceAllParaCommand = new RelayCommand(async () =>
            {
                string dataHostStationIp =
                    $"sed -i 's/^\\[HostStationIp:.*/\\[HostStationIp:{AfterHostIp}\\]/' /opt/cfg/AppPara.cfg";
                TcpClientHelper.SendDataToServerWithNewLine(dataHostStationIp);
                await Task.Delay(500);
                string dataIpAddr = $"sed -i 's/^\\[IpAddr:.*/\\[IpAddr:{AfterIp}\\]/' /opt/cfg/AppPara.cfg";
                TcpClientHelper.SendDataToServerWithNewLine(dataIpAddr);
                await Task.Delay(500);
                string dataGateway =
                    $"sed -i 's/^\\[Gateway:.*/\\[Gateway:{AfterGateway}\\]/'  /opt/cfg/AppPara.cfg";
                TcpClientHelper.SendDataToServerWithNewLine(dataGateway);
            });
        }
    }
}