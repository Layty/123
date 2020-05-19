using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using NLog;
using 三相智慧能源网关调试软件.Properties;

namespace 三相智慧能源网关调试软件
{
    public class TcpClientHelper : ViewModelBase, IDisposable, IDataErrorInfo
    {
        #region 网络

        private string _sendMsg;

        public string MySendMessage
        {
            get => _sendMsg;
            set
            {
                _sendMsg = value;
                RaisePropertyChanged();
            }
        }

        private string _receiveMsg;

        public string MyReceiveMessage
        {
            get => _receiveMsg;
            set
            {
                _receiveMsg = value;
                RaisePropertyChanged();
            }
        }

        private string _localIp;

        public string LocalIp
        {
            get => _localIp;
            set
            {
                _localIp = value;
                RaisePropertyChanged();
            }
        }


        private string _serverIpAddress;

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression("^((2(5[0-5]|[0-4]\\d))|[0-1]?\\d{1,2})(\\.((2(5[0-5]|[0-4]\\d))|[0-1]?\\d{1,2})){3}$$",
            ErrorMessage = "请输入正确的IP地址！")]
        public string ServerIpAddress
        {
            get => _serverIpAddress;
            set
            {
                _serverIpAddress = value;
                RaisePropertyChanged();
            }
        }

        private int _serverPortNum;

        [Required(ErrorMessage = "不能为空！")]
        public int ServerPortNum
        {
            get => _serverPortNum;
            set
            {
                _serverPortNum = value;
                RaisePropertyChanged();
            }
        }

        private IPEndPoint _ipEndPoint;

        public IPEndPoint IpEndPoint
        {
            get => _ipEndPoint;
            set
            {
                _ipEndPoint = value;
                RaisePropertyChanged();
            }
        }

        private bool _connectResult;

        public bool ConnectResult
        {
            get => ClientSocket.Connected;
            private set
            {
                _connectResult = value;
                RaisePropertyChanged();
            }
        }

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

        public TcpClientHelper()
        {
            ServerIpAddress = Settings.Default.GatewayIpAddress;
            ServerPortNum = 23;
            IpEndPoint = new IPEndPoint(IPAddress.Parse(ServerIpAddress), ServerPortNum);
            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var hostAddresses = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ipa in hostAddresses)
            {
                if (ipa.AddressFamily == AddressFamily.InterNetwork)
                    LocalIp = ipa.ToString();
            }

            ConnectOrDisconnectCommand = new RelayCommand(() =>
            {
                if (ClientSocket.Connected)
                {
                    Disconnect();
                }
                else
                {
                    ConnectToServer();
                }
            });
            SendMsgCommand = new RelayCommand<string>(SendDataToServerWithNewLine);
            SendMsgControlCCommand=new RelayCommand(() =>
            {
                SendDataToServer(new byte[]{0x03});
            });
            TryToLoginCommand = new RelayCommand((async () =>
            {
                SendDataToServerWithNewLine("root");
                await Task.Delay(500);
                SendDataToServerWithNewLine("11223344");
            }));
            //主站IP
            ReplaceAppParaHostIpCommand = new RelayCommand(() =>
            {
                string data =
                    $"sed -i 's/^\\[HostStationIp:.*/\\[HostStationIp:{AfterHostIp}\\]/' /opt/cfg/AppPara.cfg";
                SendDataToServerWithNewLine(data);
            });
            //网关IP
            ReplaceAppParaIpAddrCommand = new RelayCommand(() =>
            {
                string data = $"sed -i 's/^\\[IpAddr:.*/\\[IpAddr:{AfterIp}\\]/' /opt/cfg/AppPara.cfg";
                SendDataToServerWithNewLine(data);
            });
            //网关默认网关
            ReplaceAppParaIpGateWayCommand = new RelayCommand(() =>
            {
                string data =
                    $"sed -i 's/^\\[Gateway:.*/\\[Gateway:{AfterGateway}\\]/'  /opt/cfg/AppPara.cfg";
                SendDataToServerWithNewLine(data);
            });
            //主站IP/网关IP/网关默认网关
            ReplaceAllParaCommand = new RelayCommand(async () =>
            {
                string dataHostStationIp =
                    $"sed -i 's/^\\[HostStationIp:.*/\\[HostStationIp:{AfterHostIp}\\]/' /opt/cfg/AppPara.cfg";
                SendDataToServerWithNewLine(dataHostStationIp);
                await Task.Delay(500);
                string dataIpAddr = $"sed -i 's/^\\[IpAddr:.*/\\[IpAddr:{AfterIp}\\]/' /opt/cfg/AppPara.cfg";
                SendDataToServerWithNewLine(dataIpAddr);
                await Task.Delay(500);
                string dataGateway =
                    $"sed -i 's/^\\[Gateway:.*/\\[Gateway:{AfterGateway}\\]/'  /opt/cfg/AppPara.cfg";
                SendDataToServerWithNewLine(dataGateway);
            });
        }

        public void ConnectToServer()
        {
            try
            {
                Task.Run(delegate
                {
                    try
                    {
                        ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        Messenger.Default.Send($"{ClientSocket.LocalEndPoint}正在尝试连接{ClientSocket.RemoteEndPoint}",
                            "Status");
                        ClientSocket.Connect(ServerIpAddress, ServerPortNum);
                        ConnectResult = true;
                        Messenger.Default.Send($"成功连接至{ClientSocket.RemoteEndPoint}", "Status");
                    }
                    catch (Exception e)
                    {
                        ConnectResult = false;
                        Messenger.Default.Send("连接不成功", "Status");
                        Messenger.Default.Send("异常" + e.Message, "TelNetErrorEvent");
                        throw;
                    }

                    Task.Run(ReceiveData);
                });
            }
            catch (Exception e)
            {
                ConnectResult = false;
                Messenger.Default.Send("异常" + e.Message, "TelNetErrorEvent");
                throw;
            }
        }

        private void ReceiveData()
        {
            try
            {
                for (; ClientSocket.Connected;)
                {
                    int receiveDataLen = ClientSocket.Receive(_messageByteServer);
                    bool flag2 = receiveDataLen == 0;
                    if (flag2)
                    {
                        string str2 = $"{DateTime.Now}  {ClientSocket.RemoteEndPoint} 服务端主动断开了当前链接..." + "\r\n";
                        Messenger.Default.Send(str2, "Status");
                        break;
                    }

                    byte[] receiveBytes = _messageByteServer.Take(receiveDataLen).ToArray();
                  
                    Messenger.Default.Send(receiveBytes, "ReceiveDataEvent");
                }
            }
            catch (Exception e)
            {
                Messenger.Default.Send(e.Message, "TelNetErrorEvent");
            }
            finally
            {
                ClientSocket.Disconnect(false);
                ConnectResult = false;
            }
        }

      
    
        /// <summary>
        /// 发送数据至服务端，并加上换行符
        /// </summary>
        /// <param name="inputSendData"></param>
        public void SendDataToServerWithNewLine(string inputSendData)
        {
            if (ConnectResult == false)
            {
                return;
            }

            bool flag = inputSendData == null;
            if (flag)
            {
                throw new ArgumentNullException("inputSendData");
            }

            try
            {
                byte[] sendBytes = Encoding.Default.GetBytes(inputSendData + Environment.NewLine);
                ClientSocket.Send(sendBytes);
               
                MySendMessage += inputSendData + Environment.NewLine;
                Messenger.Default.Send(sendBytes, "SendDataEvent");
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(ex.Message, "ENetErrorEvent");
            }
        }

        public void SendDataToServer(string inputSendData)
        {
            if (ConnectResult == false)
            {
                return;
            }

            bool flag = inputSendData == null;
            if (flag)
            {
                throw new ArgumentNullException("inputSendData");
            }

            try
            {
                byte[] sendBytes = Encoding.Default.GetBytes(inputSendData);
                ClientSocket.Send(sendBytes);
                MySendMessage += (inputSendData + Environment.NewLine);
                Messenger.Default.Send(sendBytes, "SendDataEvent");
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(ex.Message, "TelNetErrorEvent");
            }
        }

        public void SendDataToServer(byte[] inputBytesData)
        {
            bool flag = inputBytesData.Length == 0;
            if (flag)
            {
                throw new ArgumentNullException("inputBytesData");
            }

            try
            {
                ClientSocket.Send(inputBytesData);
                Messenger.Default.Send(inputBytesData, "SendDataEvent");
            }
            catch (Exception ex)
            {
                Messenger.Default.Send("异常" + ex.Message, "TelNetErrorEvent");
            }
        }


        public void Disconnect()
        {
            if (ConnectResult == false)
            {
                return;
            }

            ClientSocket.Disconnect(false);
            ConnectResult = false;
            Messenger.Default.Send("关闭连接成功", "Status");
        }

        public void CloseAll()
        {
            if (ConnectResult == false)
            {
                return;
            }

            ClientSocket?.Close();
            ConnectResult = false;
            Messenger.Default.Send("关闭连接成功", "Status");
        }


        public void Dispose()
        {
            ((IDisposable) ClientSocket).Dispose();
        }


        public Socket ClientSocket { get; set; }


        private readonly byte[] _messageByteServer = new byte[1024];


        public string this[string columnName]
        {
            get
            {
                ValidationContext vc = new ValidationContext(this, null, null);
                vc.MemberName = columnName;
                var res = new List<ValidationResult>();
                var result = Validator.TryValidateProperty(this.GetType().GetProperty(columnName).GetValue(this, null),
                    vc, res);
                if (res.Count > 0)
                {
                    AddDic(dataErrors, vc.MemberName);
                    return string.Join(Environment.NewLine, res.Select(r => r.ErrorMessage).ToArray());
                }

                RemoveDic(dataErrors, vc.MemberName);
                return null;
            }
        }


        /// <summary>
        /// 表当验证错误集合
        /// </summary>
        private Dictionary<String, String> dataErrors = new Dictionary<String, String>();

        public string Error { get; }

        /// <summary>
        /// 移除字典
        /// </summary>
        /// <param name="dics"></param>
        /// <param name="dicKey"></param>
        private void RemoveDic(Dictionary<string, string> dics, string dicKey)
        {
            dics.Remove(dicKey);
        }

        /// <summary>
        /// 添加字典
        /// </summary>
        /// <param name="dics"></param>
        /// <param name="dicKey"></param>
        private void AddDic(Dictionary<string, string> dics, string dicKey)
        {
            if (!dics.ContainsKey(dicKey)) dics.Add(dicKey, "");
        }
    }
}