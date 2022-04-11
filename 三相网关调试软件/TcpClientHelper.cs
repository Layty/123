using CommunityToolkit.Mvvm.Messaging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace 三相智慧能源网关调试软件
{
    public class TcpClientHelper : ObservableValidator
    {
        public Socket ClientSocket { get; set; }

        /// <summary>
        /// 最大接收缓存字节数，默认1024
        /// </summary>
        public int MaxBuffer
        {
            get => _maxBuffer;
            set
            {
                _maxBuffer = value;
                OnPropertyChanged();
            }
        }

        private int _maxBuffer = 1024;

        private readonly byte[] _messageByteServer = new byte[1024];

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression("^((2(5[0-5]|[0-4]\\d))|[0-1]?\\d{1,2})(\\.((2(5[0-5]|[0-4]\\d))|[0-1]?\\d{1,2})){3}$$",
            ErrorMessage = "请输入正确的IP地址！")]
        public string LocalIp
        {
            get => _localIp;
            set
            {
                _localIp = value;
                OnPropertyChanged();
            }
        }

        private string _localIp;

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

        /// <summary>
        /// 服务端口号
        /// </summary>
        [Required(ErrorMessage = "不能为空！")]
        public int ServerPortNum
        {
            get => _serverPortNum;
            set
            {
                _serverPortNum = value;
                OnPropertyChanged();
            }
        }

        private int _serverPortNum;
        private IPEndPoint _ipEndPoint;

        public IPEndPoint IpEndPoint
        {
            get => _ipEndPoint;
            set
            {
                _ipEndPoint = value;
                OnPropertyChanged();
            }
        }

        private bool _connectResult;

        public bool ConnectResult
        {
            get => ClientSocket.Connected;
            private set
            {
                _connectResult = value;
                OnPropertyChanged();
            }
        }



        /// <summary>
        /// 成功连接至服务端事件
        /// </summary>
        public event Action<string, string> SucceedConnectToServerEvent;
        public event Action<Socket, byte[]> ReceiveDataEvent;
        public event Action<Socket, byte[]> SendDataToServerEvent;
        public event Action ErrorEvent;
        public event Action DisconnectEvent;
        protected virtual void OnDisconnectEvent()
        {
            DisconnectEvent?.Invoke();
        }
        protected virtual void OnErrorEvent()
        {
            ErrorEvent?.Invoke();
        }

        protected virtual void OnSucceedConnectToServer(string local, string remote)
        {
            StrongReferenceMessenger.Default.Send($"{ClientSocket.LocalEndPoint}成功连接至{ClientSocket.RemoteEndPoint}",
                "ClientStatus");
            //这里不手动负值LocalEndPoint，采用系统分配方式
            ConnectResult = true;
            SucceedConnectToServerEvent?.Invoke(local, remote);
        }

        protected virtual void OnReceiveDataFromServer(Socket serverSocket, byte[] bytes)
        {
            ReceiveDataEvent?.Invoke(serverSocket, bytes);
            StrongReferenceMessenger.Default.Send((serverSocket, bytes).ToTuple(), "ClientReceiveDataEvent");
        }

        protected virtual void OnSendDataToServer(Socket serverSocket, byte[] bytes)
        {
            SendDataToServerEvent?.Invoke(serverSocket, bytes);
            StrongReferenceMessenger.Default.Send((serverSocket, bytes).ToTuple(), "ClientSendDataEvent");
        }

        public TcpClientHelper(string serverIpAddress, int serverPortNum)
        {
            ServerIpAddress = serverIpAddress;
            ServerPortNum = serverPortNum;
            IpEndPoint = new IPEndPoint(IPAddress.Parse(serverIpAddress), serverPortNum);
            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            LocalIp = GetHostIp();
        }


        public static string GetHostIp()
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress[] addressList = hostEntry.AddressList;
            string result = "";
            if (addressList.Length == 0)
            {
                return result;
            }

            IPAddress[] array = addressList;
            foreach (IPAddress iPAddress in array)
            {
                if (iPAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    result = iPAddress.ToString();
                }
            }

            return result;
        }

        public void ConnectToServer()
        {
            try
            {
                Task.Run(() =>
                {
                    try
                    {
                        ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                        StrongReferenceMessenger.Default.Send(
                            $"{ClientSocket.LocalEndPoint}正在尝试连接{ClientSocket.RemoteEndPoint}",
                            "ClientStatus");
                        ClientSocket.Connect(ServerIpAddress, ServerPortNum);
                        OnSucceedConnectToServer(ClientSocket.LocalEndPoint.ToString(),
                            ClientSocket.RemoteEndPoint.ToString());
                        Task.Run(ReceiveData);
                    }
                    catch (Exception e)
                    {
                        ConnectResult = false;
                        StrongReferenceMessenger.Default.Send("连接不成功 from  ConnectToServer()", "ClientStatus");
                        StrongReferenceMessenger.Default.Send("异常" + e.Message + "from  ConnectToServer()",
                            "ClientNetErrorEvent");
                    }
                });
            }
            catch (Exception e)
            {
                ConnectResult = false;
                StrongReferenceMessenger.Default.Send("异常" + e.Message + "from  ConnectToServer()",
                    "ClientNetErrorEvent");
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
                        string str2 = $"{DateTime.Now}  {ClientSocket.RemoteEndPoint} 服务端主动断开了当前链接...\r\n";
                        StrongReferenceMessenger.Default.Send(str2, "ClientStatus");
                        Disconnect();
                        break;
                    }

                    byte[] receiveBytes = _messageByteServer.Take(receiveDataLen).ToArray();
                    OnReceiveDataFromServer(ClientSocket, receiveBytes);
                }
            }
            catch (Exception e)
            {
                StrongReferenceMessenger.Default.Send(e.Message + "from  ReceiveData()", "ClientNetErrorEvent");
                ConnectResult = false;
            }
        }



        public void SendDataToServer(byte[] inputBytesData)
        {
            bool flag = inputBytesData.Length == 0;
            if (flag)
            {
                throw new ArgumentNullException(nameof(inputBytesData));
            }

            try
            {
                ClientSocket.Send(inputBytesData);
                OnSendDataToServer(ClientSocket, inputBytesData);
            }
            catch (Exception ex)
            {
                StrongReferenceMessenger.Default.Send("异常" + ex.Message, "ClientNetErrorEvent");
                OnErrorEvent();
            }
        }

        /// <summary>
        /// 发送数据至服务端，并加上换行符
        /// </summary>
        /// <param name="sendData"></param>
        public void SendDataToServerWithNewLine(string sendData)
        {
            if (ConnectResult == false)
            {
                return;
            }

            if (string.IsNullOrEmpty(sendData))
            {
                throw new ArgumentNullException(nameof(sendData));
            }

            try
            {
                byte[] sendBytes = Encoding.Default.GetBytes(sendData + Environment.NewLine);
                ClientSocket.Send(sendBytes);
                OnSendDataToServer(ClientSocket, sendBytes);
            }
            catch (Exception ex)
            {
                StrongReferenceMessenger.Default.Send(ex.Message, "ClientErrorEvent");
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
            StrongReferenceMessenger.Default.Send("关闭连接成功", "ClientStatus");

        }

        public void CloseAll()
        {
            if (ConnectResult == false)
            {
                return;
            }

            ClientSocket?.Close();
            ConnectResult = false;
            StrongReferenceMessenger.Default.Send("关闭连接成功", "ClientStatus");
        }

        public void Dispose()
        {
            ((IDisposable)ClientSocket).Dispose();
        }



    }
}