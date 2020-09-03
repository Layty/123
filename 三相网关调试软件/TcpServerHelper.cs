using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using NLog;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects;

namespace 三相智慧能源网关调试软件
{
    public class TcpTranslator : ViewModelBase
    {
       
        public TcpServerHelper TcpListener { get; set; }

        public int LocalPort
        {
            get => _localPort;
            set
            {
                _localPort = value;
                RaisePropertyChanged();
            }
        }

        private int _localPort;

        public string LocalIp
        {
            get => _localIp;
            set
            {
                _localIp = value;
                RaisePropertyChanged();
            }
        }

        private string _localIp;

        public string RemoteIp
        {
            get => _remoteIp;
            set
            {
                _remoteIp = value;
                RaisePropertyChanged();
            }
        }

        private string _remoteIp;

        public int RemotePort
        {
            get => _remotePort;
            set
            {
                _remotePort = value;
                RaisePropertyChanged();
            }
        }

        private int _remotePort;


        public bool IsNeedToConvert12HeartBeatTo8
        {
            get => _isNeedToConvert12HeartBeatTo8;
            set { _isNeedToConvert12HeartBeatTo8 = value; RaisePropertyChanged(); }
        }
        private bool _isNeedToConvert12HeartBeatTo8;
        private byte[] heightBytesOfHeartBeatBytes;
        public TcpTranslator()
        {
            RemoteIp= LocalIp = Properties.Settings.Default.GatewayIpAddress;
            RemotePort= LocalPort = Properties.Settings.Default.GatewayPort;
        }

        public TcpTranslator(string localIp, int localPort, string remoteIp, int remotePort)
        {
            LocalIp = localIp;
            LocalPort = localPort;
            RemoteIp = remoteIp;
            RemotePort = remotePort;
        }
        public void StartListen()
        {
            TcpListener = new TcpServerHelper(LocalIp, LocalPort);
            TcpListener.AcceptNewClient += TcpListener_AcceptNewClient;
            TcpListener.StartListen();
        }

     
        /// <summary>
        /// 表端Socket 为key====>服务端的socket绑定 value
        /// </summary>
        IDictionary<Socket, TcpClientHelper> dictionary = new Dictionary<Socket, TcpClientHelper>();
        private void TcpListener_AcceptNewClient(Socket obj)
        {
            TcpClientHelper tcpClientHelper = new TcpClientHelper(RemoteIp,RemotePort);
            tcpClientHelper.ConnectToServer();
            dictionary[obj] = tcpClientHelper;
            TcpListener.ReceiveBytes += TcpListener_ReceiveBytes;
            tcpClientHelper.ReceiveByte += TcpClientHelper_ReceiveByte;
        }
        private void TcpListener_ReceiveBytes(Socket meterSocket, byte[] arg2)
        {
            foreach (var socket in dictionary)
            {
                if (meterSocket == socket.Key)
                {
                    if (IsNeedToConvert12HeartBeatTo8)
                    {
                        var frame = new HeartBeatFrame();
                        var t = frame.PduBytesToConstructor(arg2);
                        if (t)
                        {
                            var len = BitConverter.ToInt16(frame.LengthBytes.Reverse().ToArray(), 0);
                            if (len == 0x0F)//12位转8位
                            {
                                heightBytesOfHeartBeatBytes = frame.MeterAddressBytes.Take(4).ToArray();//保留高位4地址后续用于补全
                                frame.MeterAddressBytes = frame.MeterAddressBytes.Skip(4).ToArray();
                                arg2 = frame.ToPduBytes();
                            }
                        }
                    }
                    socket.Value?.SendDataToServer( arg2);
                }
            }

        }
        private void TcpClientHelper_ReceiveByte(Socket arg1, byte[] arg2)
        {
            foreach (var socket in dictionary)
            {
                if (arg1== socket.Value.ClientSocket)
                {
                    if (IsNeedToConvert12HeartBeatTo8)
                    {
                        var frame = new HeartBeatFrame();
                        var t = frame.PduBytesToConstructor(arg2);
                        if (t)
                        {
                            var len = BitConverter.ToInt16(frame.LengthBytes.Reverse().ToArray(), 0);
                            if (len == 0x0B)
                            {// 8位转12位
                                var list = new List<byte>();
                                list.AddRange(heightBytesOfHeartBeatBytes);//添加原保留的高位地址
                                list.AddRange(frame.MeterAddressBytes);
                                frame.MeterAddressBytes = list.ToArray();
                                arg2 = frame.ToPduBytes();
                            }
                        }

                    }
                    TcpListener.SendDataToClient(socket.Key, arg2);
                }
            }
         
        }

        public void Stop()
        {
            TcpListener.CloseSever();
            
            dictionary.Clear();
        }
    }

    public class TcpServerHelper : ViewModelBase
    {
        public string ListenIpAddress
        {
            get => _listenIpAddress;
            set
            {
                _listenIpAddress = value;
                RaisePropertyChanged();
            }
        }

        private string _listenIpAddress;

        public int ListenPort
        {
            get => _listenPort;
            set
            {
                _listenPort = value;
                RaisePropertyChanged();
            }
        }

        private int _listenPort;
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

        public Socket SocketServer { get; set; }

        public ProtocolType ProtocolType;

        public ObservableCollection<Socket> SocketClientList
        {
            get => _socketClientList;
            set
            {
                _socketClientList = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<Socket> _socketClientList;

        public ObservableCollection<EndPoint> SocketClientListEndPoint
        {
            get => _socketClientListEndPoint;
            set
            {
                _socketClientListEndPoint = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<EndPoint> _socketClientListEndPoint = new ObservableCollection<EndPoint>();

        public int ResponseTimeOut
        {
            get => _responseTimeOut;
            set
            {
                _responseTimeOut = value;
                RaisePropertyChanged();
            }
        }

        private int _responseTimeOut = 2;

        public readonly List<CancellationTokenSource> SocketClientCancellationTokens =
            new List<CancellationTokenSource>();

        private readonly CancellationTokenSource _sourceServer = new CancellationTokenSource();


        public event Action<string> ErrorMsg;
        public event Action<string> StatusMsg;
        public event Action<Socket> AcceptNewClient;
        public event Action<Socket, byte[]> ReceiveBytes;
        public event Action<Socket, byte[]> SendBytesToClient;

        protected virtual void OnNotifyNewClient(Socket clientSocket)
        {
            AcceptNewClient?.Invoke(clientSocket);
        }
        protected virtual void OnReceiveBytes(Socket clientSocket, byte[] bytes)
        {
            ReceiveBytes?.Invoke(clientSocket, bytes);
            Messenger.Default.Send((clientSocket, bytes), "ReceiveDataEvent");
        }

        protected virtual void OnSendBytesToClient(Socket clientSocket, byte[] bytes)
        {
            SendBytesToClient?.Invoke(clientSocket, bytes);
            Messenger.Default.Send((clientSocket, bytes), "SendDataEvent");
        }

        private void OnNotifyErrorMsg(string msg)
        {
            this.ErrorMsg?.Invoke(msg);
            Messenger.Default.Send(msg, "ErrorEvent");
        }

        protected virtual void OnNotifyStatusMsg(string msg)
        {
            Messenger.Default.Send(msg, "Status");
            StatusMsg?.Invoke(msg);
        }

        public TcpServerHelper(string listenIpAddress, int listenPort)
        {
            ListenIpAddress = listenIpAddress;
            ListenPort = listenPort;
            SocketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            SocketClientList = new ObservableCollection<Socket>();
        }

        public TcpServerHelper(string listenIpAddress, int listenPort, ProtocolType protocolType = ProtocolType.Tcp)
        {
            ListenIpAddress = listenIpAddress;
            ListenPort = listenPort;
            ProtocolType = protocolType;

            SocketClientList = new ObservableCollection<Socket>();
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

        public void StartListen()
        {
            SocketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IpEndPoint = new IPEndPoint(IPAddress.Parse(ListenIpAddress), ListenPort);
            SocketServer.Bind(IpEndPoint);
            SocketServer.Listen(5);
            OnNotifyStatusMsg($"监听{IpEndPoint}成功");
            StartListenServerAsync(SocketServer);
        }


        private void StartListenServerAsync(Socket serverSocket)
        {
            Socket clientSocket;
            Task.Run(delegate
            {
                while (true)
                {
                    try
                    {
                        clientSocket = serverSocket.Accept();
                        var socket1 = clientSocket;
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            SocketClientList.Add(socket1);
                            SocketClientListEndPoint.Add(socket1.RemoteEndPoint);
                        });
                        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                        SocketClientCancellationTokens.Add(cancellationTokenSource);
                        OnNotifyStatusMsg($"{DateTime.Now}有新的连接{clientSocket.RemoteEndPoint}");
                        OnNotifyNewClient(clientSocket);
                        var socket = clientSocket;

                        Task.Run(delegate { ClientThread(socket); }, cancellationTokenSource.Token);
                    }
                    catch (Exception ex)
                    {
                        Logger logger = LogManager.GetCurrentClassLogger();
                        logger.Error(ex);
                        OnNotifyStatusMsg("退出服务端监听Task");
                        // CloseSever();
                        break;
                    }
                }
            });
        }


        private void ClientThread(Socket sockClient)
        {
            byte[] array = new byte[1024];

            while (true)
            {
                int num;
                try
                {
                    num = sockClient.Receive(array);
                    byte[] bytes = array.Take(num).ToArray();
                    OnReceiveBytes(sockClient, bytes);
                }
                catch (Exception ex)
                {
                    Logger logger = LogManager.GetCurrentClassLogger();
                    logger.Error(ex);
                    OnNotifyStatusMsg($"退出客户端{sockClient.RemoteEndPoint}Task");
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        if (SocketClientList.Contains(sockClient))
                        {
                            SocketClientList.Remove(sockClient);
                        }
                    });

                    break;
                }

                if (num == 0)
                {
                    OnNotifyStatusMsg($"客户端{sockClient.RemoteEndPoint} 断开了\r\n");
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        if (SocketClientList.Contains(sockClient))
                        {
                            SocketClientList.Remove(sockClient);
                            SocketClientListEndPoint.Remove(sockClient.RemoteEndPoint);
                        }
                    });
                    break;
                }
            }

            //  sockClient.Close();
        }

        public void SendDataToAllClients(byte[] bytes)
        {
            foreach (Socket socketClient in SocketClientList)
            {
                socketClient.Send(bytes);
                OnSendBytesToClient(socketClient, bytes);
            }
        }

        public void SendDataToClient(Socket destinationSocket, byte[] bytes)
        {
            destinationSocket.Send(bytes);
            OnSendBytesToClient(destinationSocket, bytes);
        }


        public async Task<byte[]> SendDataToClientAndWaitReceiveData(Socket destinationSocket, byte[] bytes)
        {
            _returnBytes = null;
            ReceiveBytes += TcpServerHelper_ReceiveBytes;
            destinationSocket.Send(bytes);
            OnSendBytesToClient(destinationSocket, bytes);
            await Task.Run(() =>
            {
                Stopwatch stopwatch1 = new Stopwatch();
                TimeSpan startTimeSpan = new TimeSpan(DateTime.Now.Ticks);
                stopwatch1.Start();
                while (true)
                {
                    TimeSpan stopTimeSpan = new TimeSpan(DateTime.Now.Ticks);
                    TimeSpan timeSpan = stopTimeSpan.Subtract(startTimeSpan).Duration();
                    if (timeSpan.Seconds >= ResponseTimeOut)
                    {
                        ResponseTime = timeSpan.Seconds.ToString();
                        stopwatch1.Reset();
                        OnNotifyStatusMsg($"超时{ResponseTimeOut}秒未响应");
                        break;
                    }

                    if (_returnBytes != null)
                    {
                        stopwatch1.Stop();
                        ResponseTime = stopwatch1.ElapsedMilliseconds.ToString();
                        stopwatch1.Reset();
                        break;
                    }
                }
            });
            //  await Task.Delay(2000);
            ReceiveBytes -= TcpServerHelper_ReceiveBytes;
            return _returnBytes;
        }

        public string ResponseTime { get; set; }

        private byte[] _returnBytes;
        private readonly List<byte> _listReturnBytes = new List<byte>();
        private bool _isNeedContinue = false;
        private int TotalLength { get; set; }
        private int NeedReceiveLength { get; set; }

        private void TcpServerHelper_ReceiveBytes(Socket clientSocket, byte[] bytes)
        {
            if (bytes == null)
            {
                return;
            }

            if (!_isNeedContinue)
            {
                if (bytes.Length < 7)
                {
                    Logger lgLogger = LogManager.GetCurrentClassLogger();
                    lgLogger.Debug("This Is Not 47Message Should Never Enter Here");
                    _listReturnBytes.AddRange(bytes);
                    _returnBytes = _listReturnBytes.ToArray();
                    _listReturnBytes.Clear();
                }
                else
                {
                    if (bytes[7] == (bytes.Length - 8))
                    {
                        _listReturnBytes.AddRange(bytes);
                        _returnBytes = _listReturnBytes.ToArray();
                        _listReturnBytes.Clear();
                        _isNeedContinue = false;
                    }

                    if (bytes[7] > (bytes.Length - 8))
                    {
                        TotalLength = bytes[7];
                        NeedReceiveLength = TotalLength - (bytes.Length - 8);
                        _listReturnBytes.AddRange(bytes);
                        _isNeedContinue = true;
                    }
                }
            }
            else
            {
                if (bytes.Length < NeedReceiveLength)
                {
                    NeedReceiveLength = NeedReceiveLength - bytes.Length;
                    _listReturnBytes.AddRange(bytes);
                }

                if (bytes.Length >= NeedReceiveLength)
                {
                    NeedReceiveLength = 0;
                    _isNeedContinue = false;
                    _listReturnBytes.AddRange(bytes);
                    _returnBytes = _listReturnBytes.ToArray();
                    _listReturnBytes.Clear();
                }
            }
        }

        public void CloseSever()
        {
            if (SocketClientList.Count != 0)
            {
                foreach (Socket socketClient in SocketClientList)
                {
                    if (socketClient != null)
                    {
                        socketClient.Shutdown(SocketShutdown.Both);
                        socketClient.Close();
                    }
                }

                foreach (CancellationTokenSource socketClientCancellationToken in SocketClientCancellationTokens)
                {
                    socketClientCancellationToken.Cancel();
                }

                SocketClientList.Clear();
            }

            if (SocketServer != null)
            {
                // _sourceServer.Cancel();
                SocketServer.Close();
                SocketServer.Dispose();
                OnNotifyStatusMsg(DateTime.Now + "已关闭监听" + Environment.NewLine);
            }
        }


        public void SendFile()
        {
            SocketClientList[0].SendFile("txt.txt");
        }

        public void DisConnectClient(string strRemoteEndPoint)
        {
            int count = SocketClientList.Count;
            int num = 0;
            while (true)
            {
                if (num < count)
                {
                    if (strRemoteEndPoint == SocketClientList[num].RemoteEndPoint.ToString())
                    {
                        break;
                    }

                    num++;
                    continue;
                }

                return;
            }

            OnNotifyStatusMsg($"{DateTime.Now}执行关闭{SocketClientList[num].RemoteEndPoint}客户端操作{Environment.NewLine}");
            SocketClientList[num].Shutdown(SocketShutdown.Both);
            SocketClientList[num].Disconnect(reuseSocket: false);
            DispatcherHelper.CheckBeginInvokeOnUI(() => { SocketClientList.RemoveAt(num); });
            //  SocketClientList.RemoveAt(num);
            SocketClientCancellationTokens[num].Cancel();
            SocketClientCancellationTokens.RemoveAt(num);
        }

        TcpClient clientForTran = new TcpClient();
        IDictionary<Socket, TcpClient> liiii = new Dictionary<Socket, TcpClient>();

        public void StartListenServerAsyncNew(Socket tcpListenerSocketServer,string remoteIp,int remotePort, bool isNeedToConvert12HeartBeatTo8)
        {
            Socket clientSocket;
            Task.Run(delegate
            {
                while (true)
                {
                    try
                    {
                        clientSocket = tcpListenerSocketServer.Accept();
                        var socket1 = clientSocket;
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            SocketClientList.Add(socket1);
                            SocketClientListEndPoint.Add(socket1.RemoteEndPoint);
                        });
                        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                        SocketClientCancellationTokens.Add(cancellationTokenSource);
                        OnNotifyStatusMsg($"{DateTime.Now}有新的转发连接{clientSocket.RemoteEndPoint}");
                        clientForTran = new TcpClient();
                        clientForTran.Connect(remoteIp, remotePort);
                        OnNotifyStatusMsg($"已连接至{remoteIp}{remotePort}");
                        liiii[socket1] = clientForTran;//绑定
                        var socket = clientSocket;

                        Task.Run(delegate { ClientThreadNew(socket, clientForTran, isNeedToConvert12HeartBeatTo8); }, cancellationTokenSource.Token);
                        Task.Run(() => { ClientThreadFromHostNew(socket, clientForTran, isNeedToConvert12HeartBeatTo8); }, cancellationTokenSource.Token);
                    }
                    catch (Exception ex)
                    {
                        Logger logger = LogManager.GetCurrentClassLogger();
                        logger.Error(ex);
                        OnNotifyStatusMsg("退出服务端监听Task");
                        // CloseSever();
                        break;
                    }
                }
            });
        }

        private byte[] heightBytesOfHeartBeatBytes;
        /// <summary>
        /// 接收主站的数据转发至表端
        /// </summary>
        /// <param name="sockClient"></param>
        /// <param name="client"></param>
        private void ClientThreadFromHostNew(Socket sockClient, TcpClient client,bool isNeedToConver12HeartBeatTo8)
        {
            byte[] array = new byte[1024];

            while (true)
            {
                int num;
                try
                {
                    num = client.Client.Receive(array);

                    byte[] bytes = array.Take(num).ToArray();
                   // OnReceiveBytes(client.Client, bytes);
                    if (isNeedToConver12HeartBeatTo8)
                    {
                        var frame = new HeartBeatFrame();
                        var t = frame.PduBytesToConstructor(bytes);
                        if (t)
                        {
                            var len = BitConverter.ToInt16(frame.LengthBytes.Reverse().ToArray(), 0);
                            if (len == 0x0B)
                            {// 8位转12位
                                var list = new List<byte>();
                                list.AddRange(heightBytesOfHeartBeatBytes);//添加原保留的高位地址
                                list.AddRange(frame.MeterAddressBytes);
                                frame.MeterAddressBytes = list.ToArray();
                                bytes = frame.ToPduBytes();
                            }
                        }

                    }


                    sockClient.Send(bytes);
                    OnSendBytesToClient(sockClient, bytes);
                }
                catch (Exception ex)
                {
                    Logger logger = LogManager.GetCurrentClassLogger();
                    logger.Error(ex);
                    OnNotifyStatusMsg($"退出客户端{sockClient.RemoteEndPoint}Task");
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        if (SocketClientList.Contains(sockClient))
                        {
                            SocketClientList.Remove(sockClient);
                        }
                    });

                    break;
                }

                if (num == 0)
                {
                    OnNotifyStatusMsg($"客户端{sockClient.RemoteEndPoint} 断开了\r\n");
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        if (SocketClientList.Contains(sockClient))
                        {
                            SocketClientList.Remove(sockClient);
                            SocketClientListEndPoint.Remove(sockClient.RemoteEndPoint);
                        }
                    });
                    break;
                }
            }
        }

        //表端发送数据，后 转发至 主站
        private void ClientThreadNew(Socket sockClient, TcpClient client,bool isNeedToConvert12HeartBeatTo8)
        {
            byte[] array = new byte[1024];

            while (true)
            {
                int num;
                try
                {
                    num = sockClient.Receive(array);

                    byte[] bytes = array.Take(num).ToArray();
                  //  OnReceiveBytes(sockClient, bytes);
                    if (isNeedToConvert12HeartBeatTo8)
                    {
                        var frame = new HeartBeatFrame();
                        var t = frame.PduBytesToConstructor(bytes);
                        if (t)
                        {
                            var len = BitConverter.ToInt16(frame.LengthBytes.Reverse().ToArray(), 0);
                            if (len == 0x0F)//12位转8位
                            {
                                heightBytesOfHeartBeatBytes = frame.MeterAddressBytes.Take(4).ToArray();//保留高位4地址后续用于补全
                                frame.MeterAddressBytes = frame.MeterAddressBytes.Skip(4).ToArray();
                                bytes = frame.ToPduBytes();
                            }
                        }
                    }
                 

                    try
                    {
                        client.Client.Send(bytes);
                        OnSendBytesToClient(client.Client, bytes);
                    }
                    catch (Exception e)
                    {
                        Logger logger = LogManager.GetCurrentClassLogger();
                        logger.Error(e);
                        OnNotifyStatusMsg($"转发至主站 - 线程出现异常{sockClient.RemoteEndPoint}Task");
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    Logger logger = LogManager.GetCurrentClassLogger();
                    logger.Error(ex);
                    OnNotifyStatusMsg($"退出客户端{sockClient.RemoteEndPoint}Task");
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        if (SocketClientList.Contains(sockClient))
                        {
                            SocketClientList.Remove(sockClient);
                        }
                    });

                    break;
                }

                if (num == 0)
                {
                    OnNotifyStatusMsg($"客户端{sockClient.RemoteEndPoint} 断开了\r\n");
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        if (SocketClientList.Contains(sockClient))
                        {
                            SocketClientList.Remove(sockClient);
                            SocketClientListEndPoint.Remove(sockClient.RemoteEndPoint);
                        }
                    });
                    break;
                }
            }

            //  sockClient.Close();
        }
    }
}