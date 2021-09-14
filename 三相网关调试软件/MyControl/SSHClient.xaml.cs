using System;
using System.ComponentModel.DataAnnotations;
using System.Windows.Controls;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Renci.SshNet;

namespace 三相智慧能源网关调试软件.MyControl
{
    public class SSHClientViewModel : ObservableObject
    {
        /// <summary>
        /// 服务端IP地址
        /// </summary>
        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression("^((2(5[0-5]|[0-4]\\d))|[0-1]?\\d{1,2})(\\.((2(5[0-5]|[0-4]\\d))|[0-1]?\\d{1,2})){3}$$",
            ErrorMessage = "请输入正确的IP地址！")]
        public string ServerIpAddress
        {
            get => _serverIpAddress;
            set
            {
                _serverIpAddress = value;
                OnPropertyChanged();
            }
        }

        private string _serverIpAddress;

        public SSHClientViewModel()
        {
            ConnectOrDisconnectCommand = new RelayCommand(() =>
            {
                if (sshClient != null && sshClient.IsConnected)
                {
                    sshClient.Disconnect();
                }
                else
                {
                    sshClient = new SshClient(ServerIpAddress, "root", "11223344");
                    sshClient.Connect(); 
                    ConnectResult = sshClient.IsConnected;
                    OnPropertyChanged("ConnectResult");
                }


               
            });
            SendCommand = new RelayCommand<string>((t) =>
            {
                CommandResult = "";
                var cmd = sshClient.RunCommand(t);
                if (cmd.ExitStatus == 0)
                {
                    ResultString = cmd.Result;

                    Console.WriteLine(cmd.Result); //执行结果
                }
                else
                {
                    ResultString = cmd.Error;
                    Console.WriteLine(cmd.Error); //错误信息
                }

                OnPropertyChanged("ResultString");

                CommandResult = cmd.Result;
            });
        }

        public string ResultString { get; set; }
        public string CommandResult { get; set; }
        public RelayCommand<string> SendCommand { get; set; }
        private SshClient sshClient;
        public bool ConnectResult { get; set; }


        public RelayCommand ConnectOrDisconnectCommand
        {
            get => _ConnectOrDisconnectCommand;
            set
            {
                _ConnectOrDisconnectCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _ConnectOrDisconnectCommand;
    }

    /// <summary>
    /// SSHClient.xaml 的交互逻辑
    /// </summary>
    public partial class SSHClient : UserControl
    {
        public SSHClient()
        {
            InitializeComponent();
        }
    }
}