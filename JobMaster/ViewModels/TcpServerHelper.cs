using DotNetty.Transport.Channels;
using JobMaster.Helpers;
using MyDlmsStandard.Wrapper;
using NLog;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;


namespace JobMaster.ViewModels
{
    public class MeterIdMatchSocketNew : BindableBase
    {
        public IChannelHandlerContext MySocket
        {
            get => _MySocket;
            set
            {
                _MySocket = value;
                RaisePropertyChanged();
            }
        }

        private IChannelHandlerContext _MySocket;

        public string IpString
        {
            get => _ipString;
            set
            {
                _ipString = value;
                RaisePropertyChanged();
            }
        }

        private string _ipString;

        public string MeterId
        {
            get => _meterId;
            set
            {
                _meterId = value;
                RaisePropertyChanged();
            }
        }

        private string _meterId;

        public bool IsCheck
        {
            get => _isCheck;
            set
            {
                _isCheck = value;
                RaisePropertyChanged();
            }
        }

        private bool _isCheck;
    }

    public class MeterIdMatchSocket : BindableBase
    {
        public Socket MySocket
        {
            get => _MySocket;
            set
            {
                _MySocket = value;
                RaisePropertyChanged();
            }
        }

        private Socket _MySocket;

        public string IpString
        {
            get => _ipString;
            set
            {
                _ipString = value;
                RaisePropertyChanged();
            }
        }

        private string _ipString;

        public string MeterId
        {
            get => _meterId;
            set
            {
                _meterId = value;
                RaisePropertyChanged();
            }
        }

        private string _meterId;

        public bool IsCheck
        {
            get => _isCheck;
            set
            {
                _isCheck = value;
                RaisePropertyChanged();
            }
        }

        private bool _isCheck;
    }

    /// <summary>
    /// 职责：数据的发送和接收。通知socket的各个状态，校验IP等参数后的合理性
    /// </summary>
    public class TcpServerHelper : ValidateModelBase
    {
        public ObservableCollection<MeterIdMatchSocket> MeterIdMatchSockets
        {
            get => _meterIdMatchSockets;
            set
            {
                _meterIdMatchSockets = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<MeterIdMatchSocket> _meterIdMatchSockets;

        public bool IsStarted
        {
            get => _isStarted;
            set
            {
                _isStarted = value;
                RaisePropertyChanged();
            }
        }

        private bool _isStarted;

        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression("^((2(5[0-5]|[0-4]\\d))|[0-1]?\\d{1,2})(\\.((2(5[0-5]|[0-4]\\d))|[0-1]?\\d{1,2})){3}$$",
            ErrorMessage = "请输入正确的IP地址！")]
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

        [Required(ErrorMessage = "不能为空！")]
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

        /// <summary>
        /// 超时时间默认2s
        /// </summary>
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

        /// <summary>
        /// 是否自动响应47心跳帧
        /// </summary>
        public bool IsAutoResponseHeartBeat
        {
            get => _isAutoResponseHeartBeat;
            set
            {
                _isAutoResponseHeartBeat = value;
                RaisePropertyChanged();
            }
        }

        private bool _isAutoResponseHeartBeat;

        /// <summary>
        /// 心跳帧延时响应时间(ms)
        /// </summary>
        public int HeartBeatDelayTime
        {
            get => _heartBeatDelayTime;
            set
            {
                _heartBeatDelayTime = value;
                RaisePropertyChanged();
            }
        }

        private int _heartBeatDelayTime = 1000;

        public readonly List<CancellationTokenSource> SocketClientCancellationTokens =
            new List<CancellationTokenSource>();

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

            CalcTcpServerHelper_ReceiveBytes(clientSocket, bytes);

            //处理Socke和表号匹配更新等
            TcpServerHelper_MatchSocketMeterId(clientSocket, bytes);
        }

        /// <summary>
        /// 计算是否服务47协议 进行拼帧
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="bytes"></param>
        private void CalcTcpServerHelper_ReceiveBytes(Socket clientSocket, byte[] bytes)
        {
            if (bytes == null) return;

            if (!_isNeedContinue)
            {
                if (bytes.Length < 7)
                {
                    var lgLogger = LogManager.GetCurrentClassLogger();
                    lgLogger.Debug("This Is Not 47Message Should Never Enter Here");
                    _listReturnBytes.AddRange(bytes);
                    _returnBytes = _listReturnBytes.ToArray();

                    _listReturnBytes.Clear();
                }
                else
                {
                    if (bytes[7] == bytes.Length - 8)
                    {
                        _listReturnBytes.AddRange(bytes);
                        _returnBytes = _listReturnBytes.ToArray();
                        // Socket_ReceiveBytes_Notify(clientSocket, _returnBytes);
                        TcpServerHelper_ReceiveBytesHandleHeartBeat(clientSocket, _returnBytes);
                        _listReturnBytes.Clear();
                        _isNeedContinue = false;
                    }

                    if (bytes[7] > bytes.Length - 8)
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
                    NeedReceiveLength -= bytes.Length;
                    _listReturnBytes.AddRange(bytes);
                }

                if (bytes.Length >= NeedReceiveLength)
                {
                    NeedReceiveLength = 0;
                    _isNeedContinue = false;

                    _listReturnBytes.AddRange(bytes);
                    _returnBytes = _listReturnBytes.ToArray();
                    TcpServerHelper_ReceiveBytesHandleHeartBeat(clientSocket, _returnBytes);
                    //  Socket_ReceiveBytes_Notify(clientSocket, _returnBytes);
                    _listReturnBytes.Clear();
                }
            }
        }

        /// <summary>
        /// 根据是否自动回心跳帧，判断是否为心跳帧类型，模拟主站处理心跳帧功能
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="bytes"></param>
        private async void TcpServerHelper_ReceiveBytesHandleHeartBeat(Socket clientSocket, byte[] bytes)
        {
            if (!IsAutoResponseHeartBeat) return;

            try
            {
                var heartBeatFrame = Wrapper47FrameFactory.CreateHeartBeatFrame(bytes);

                if (heartBeatFrame != null)
                {
                    heartBeatFrame.WrapperHeader.OverturnDestinationSource();
                    await Task.Delay(HeartBeatDelayTime);
                    SendDataToClient(clientSocket, heartBeatFrame.ToPduStringInHex().StringToByte());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// 计算表号和Socket 对MeterIdMatchSockets进行赋值
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="bytes"></param>
        private void TcpServerHelper_MatchSocketMeterId(Socket clientSocket, byte[] bytes)
        {
            var heartBeatFrame = Wrapper47FrameFactory.CreateHeartBeatFrame(bytes);
            if (heartBeatFrame != null)
            {
                var strAdd = heartBeatFrame.GetMeterAddressString();
                if (MeterIdMatchSockets.Count == 0)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        MeterIdMatchSockets.Add(new MeterIdMatchSocket()
                        {
                            MySocket = clientSocket,
                            IpString = clientSocket.RemoteEndPoint.ToString(),
                            MeterId = strAdd,
                            IsCheck = false
                        });
                    });
                }
                else
                {
                    var boo = false;
                    foreach (var myClass in MeterIdMatchSockets)
                    {
                        if (myClass.MeterId.Contains(strAdd))
                        {
                            boo = false;
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                //socket链接可能会变，但表号唯一,此处进行更新最新的Socket链接
                                myClass.IpString = clientSocket.RemoteEndPoint.ToString();
                                myClass.MySocket = clientSocket;
                            });
                            break;
                        }
                        else
                        {
                            boo = true;
                        }
                    }

                    if (boo)
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            MeterIdMatchSockets.Add(new MeterIdMatchSocket()
                            {
                                MySocket = clientSocket,
                                IpString = clientSocket.RemoteEndPoint.ToString(),
                                MeterId = strAdd,
                                IsCheck = false
                            });
                        });
                    }
                }
            }
        }

        protected virtual void OnSendBytesToClient(Socket clientSocket, byte[] bytes)
        {
            SendBytesToClient?.Invoke(clientSocket, bytes);
        }

        private void OnNotifyErrorMsg(string msg)
        {
            ErrorMsg?.Invoke(msg);
        }

        protected virtual void OnNotifyStatusMsg(string msg)
        {
            StatusMsg?.Invoke(msg);
        }

        public TcpServerHelper(string listenIpAddress, int listenPort)
        {
            ListenIpAddress = listenIpAddress;
            ListenPort = listenPort;
            SocketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IpEndPoint = new IPEndPoint(IPAddress.Parse(ListenIpAddress), ListenPort);
            SocketClientList = new ObservableCollection<Socket>();
            MeterIdMatchSockets = new ObservableCollection<MeterIdMatchSocket>();
            IsAutoResponseHeartBeat = true;
        }

        public TcpServerHelper(string listenIpAddress, int listenPort, ProtocolType protocolType = ProtocolType.Tcp)
        {
            ListenIpAddress = listenIpAddress;
            ListenPort = listenPort;
            ProtocolType = protocolType;
            SocketClientList = new ObservableCollection<Socket>();
            MeterIdMatchSockets = new ObservableCollection<MeterIdMatchSocket>();
            IsAutoResponseHeartBeat = true;
        }

        /// <summary>
        /// 获取当前计算机的主机IPV4地址
        /// </summary>
        /// <returns></returns>
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

        public List<string> HostIPlList => GetHostIpList();

        /// <summary>
        /// 获取当前计算机的主机IPV4地址
        /// </summary>
        /// <returns></returns>
        public static List<string> GetHostIpList()
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress[] addressList = hostEntry.AddressList;

            List<string> stList = new List<string>();
            if (addressList.Length == 0)
            {
                return stList;
            }

            IPAddress[] array = addressList;
            foreach (IPAddress iPAddress in array)
            {
                if (iPAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    stList.Add(iPAddress.ToString());
                }
            }

            return stList;
        }

        public void StartListen()
        {
            try
            {
                if (IsStarted)
                {
                    return;
                }

                SocketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IpEndPoint = new IPEndPoint(IPAddress.Parse(ListenIpAddress), ListenPort);
                SocketServer.Bind(IpEndPoint);
                SocketServer.Listen(500);
                OnNotifyStatusMsg($"监听{IpEndPoint}成功");
                IsStarted = true;
                StartListenServerAsync(SocketServer);
            }
            catch (Exception exception)
            {
                OnNotifyStatusMsg(exception.Message);
            }
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

                        OnNotifyStatusMsg($"{DateTime.Now}有新的连接{clientSocket.RemoteEndPoint}");

                        var socket1 = clientSocket;
                        DispatcherHelper.CheckBeginInvokeOnUI(() => { SocketClientList.Add(socket1); });
                        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                        SocketClientCancellationTokens.Add(cancellationTokenSource);
                        OnNotifyNewClient(clientSocket);
                        var socket = clientSocket;

                        Task.Run(delegate { ClientThread(socket); }, cancellationTokenSource.Token);
                    }
                    catch (Exception ex)
                    {
                        Logger logger = LogManager.GetCurrentClassLogger();
                        logger.Error(ex);
                        IsStarted = false;
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
            string remoteEndPoint = sockClient.RemoteEndPoint.ToString();
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
                    OnNotifyStatusMsg($"退出客户端{remoteEndPoint}Task");
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

        Stopwatch stopwatch1;

        public async Task<byte[]> SendDataToClientAndWaitReceiveDataAsync(Socket destinationSocket, byte[] bytes)
        {
            return await Task.Run(async () =>
            {
                _returnBytes = null;
                ReceiveBytes += TcpServerHelper_ReceiveBytes;
                destinationSocket.Send(bytes);
                OnSendBytesToClient(destinationSocket, bytes);
                stopwatch1 = new Stopwatch();
                var nowtick = DateTime.Now.Ticks;
                TimeSpan startTimeSpan = new TimeSpan(DateTime.Now.Ticks);
                stopwatch1.Start();

                //占用大量CPU需要优化
                while (true)
                {
                    await Task.Delay(100); //进行等待延时，进行优化
                    TimeSpan elapsed = new TimeSpan(DateTime.Now.Ticks - nowtick);
                    // TimeSpan timeSpan = stopTimeSpan.Subtract(startTimeSpan).Duration();
                    if (elapsed.TotalSeconds >= ResponseTimeOut)
                    {
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

                ReceiveBytes -= TcpServerHelper_ReceiveBytes;
                return _returnBytes;
            });
        }

        public string ResponseTime { get; set; }

        private byte[] _returnBytes;
        private readonly List<byte> _listReturnBytes = new List<byte>();
        private bool _isNeedContinue;
        private int TotalLength { get; set; }
        private int NeedReceiveLength { get; set; }

        //处理拼帧 必然不需要拼帧
        private void TcpServerHelper_ReceiveBytes(Socket clientSocket, byte[] bytes)
        {
            if (bytes == null)
            {
                return;
            }

            //必然不需要拼帧
            if (!_isNeedContinue)
            {
                if (bytes.Length < 8)
                {
                    Logger lgLogger = LogManager.GetCurrentClassLogger();
                    lgLogger.Debug("This Is Not 47Message Should Never Enter Here");
                    _listReturnBytes.AddRange(bytes);
                    _returnBytes = _listReturnBytes.ToArray();

                    _listReturnBytes.Clear();
                }
                else
                {
                    var uInt16Length = BitConverter.ToUInt16(bytes.Skip(6).Take(2).Reverse().ToArray(), 0);
                    if (uInt16Length == bytes.Length - 8)
                    {
                        _listReturnBytes.AddRange(bytes);
                        _returnBytes = _listReturnBytes.ToArray();

                        _listReturnBytes.Clear();
                        _isNeedContinue = false;
                    }

                    if (uInt16Length > bytes.Length - 8)
                    {
                        TotalLength = uInt16Length;
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
                    NeedReceiveLength -= bytes.Length;
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
                SocketServer.Close();
                OnNotifyStatusMsg(DateTime.Now + "已关闭监听" + Environment.NewLine);
            }
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
            // var ss = SocketClientList.FirstOrDefault((t) => t.RemoteEndPoint.ToString() == strRemoteEndPoint);
            //  if (ss == null) return;

            OnNotifyStatusMsg($"{DateTime.Now}执行关闭{SocketClientList[num].RemoteEndPoint}客户端操作{Environment.NewLine}");
            //OnNotifyStatusMsg($"{DateTime.Now}执行关闭{ss.RemoteEndPoint}客户端操作{Environment.NewLine}");
            SocketClientList[num].Shutdown(SocketShutdown.Both);
            //   ss.Shutdown(SocketShutdown.Both);
            SocketClientList[num].Disconnect(reuseSocket: false);
            //ss.Disconnect(false);
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                SocketClientList.RemoveAt(num);
                //SocketClientList.Remove(ss);
            });

            SocketClientCancellationTokens[num].Cancel();
            SocketClientCancellationTokens.RemoveAt(num);
        }
    }
}