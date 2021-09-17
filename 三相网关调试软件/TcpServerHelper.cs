using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Microsoft.Toolkit.Mvvm.Messaging;
using System.Management;
using System.Text.RegularExpressions;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace 三相智慧能源网关调试软件
{
    /// <summary>
    /// IPProvider 的摘要说明。
    /// </summary>
    public class IPProvider
    {
        public IPProvider()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        /// <summary>
        /// 设置DNS
        /// </summary>
        /// <param name="dns"></param>
        public static void SetDNS(string[] dns)
        {
            SetIPAddress(null, null, null, dns);
        }

        /// <summary>
        /// 设置网关
        /// </summary>
        /// <param name="getway"></param>
        public static void SetGetWay(string getway)
        {
            SetIPAddress(null, null, new string[] {getway}, null);
        }

        /// <summary>
        /// 设置网关
        /// </summary>
        /// <param name="getway"></param>
        public static void SetGetWay(string[] getway)
        {
            SetIPAddress(null, null, getway, null);
        }

        /// <summary>
        /// 设置IP地址和掩码
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="submask"></param>
        public static void SetIPAddress(string ip, string submask)
        {
            SetIPAddress(new string[] {ip}, new string[] {submask}, null, null);
        }

        /// <summary>
        /// 设置IP地址，掩码和网关
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="submask"></param>
        /// <param name="getway"></param>
        public static void SetIPAddress(string ip, string submask, string getway)
        {
            SetIPAddress(new string[] {ip}, new string[] {submask}, new string[] {getway}, null);
        }

        /// <summary>
        /// 设置IP地址，掩码，网关和DNS
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="submask"></param>
        /// <param name="getway"></param>
        /// <param name="dns"></param>
        public static void SetIPAddress(string[] ip, string[] submask, string[] getway, string[] dns)
        {
            ManagementClass wmi = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = wmi.GetInstances();
            ManagementBaseObject inPar = null;
            ManagementBaseObject outPar = null;
            foreach (ManagementObject mo in moc)
            {
                //如果没有启用IP设置的网络设备则跳过
                if (!(bool) mo["IPEnabled"])
                    continue;
                //设置IP地址和掩码
                if (ip != null && submask != null)
                {
                    inPar = mo.GetMethodParameters("EnableStatic");
                    inPar["IPAddress"] = ip;
                    inPar["SubnetMask"] = submask;
                    outPar = mo.InvokeMethod("EnableStatic", inPar, null);
                }

                //设置网关地址
                if (getway != null)
                {
                    inPar = mo.GetMethodParameters("SetGateways");
                    inPar["DefaultIPGateway"] = getway;
                    outPar = mo.InvokeMethod("SetGateways", inPar, null);
                }

                //设置DNS地址
                if (dns != null)
                {
                    inPar = mo.GetMethodParameters("SetDNSServerSearchOrder");
                    inPar["DNSServerSearchOrder"] = dns;
                    outPar = mo.InvokeMethod("SetDNSServerSearchOrder", inPar, null);
                }
            }
        }

        /// <summary>
        /// 启用DHCP服务器
        /// </summary>
        public static void EnableDHCP()
        {
            ManagementClass wmi = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = wmi.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                //如果没有启用IP设置的网络设备则跳过
                if (!(bool) mo["IPEnabled"])
                    continue;
                //重置DNS为空
                mo.InvokeMethod("SetDNSServerSearchOrder", null);
                //开启DHCP
                mo.InvokeMethod("EnableDHCP", null);
            }
        }

        /// <summary>
        /// 判断是否IP地址格式
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIPAddress(string ip)
        {
            string[] arr = ip.Split('.');
            if (arr.Length != 4)
                return false;
            string pattern = @"\d{1,3}";
            for (int i = 0; i < arr.Length; i++)
            {
                string d = arr[i];
                if (i == 0 && d == "0")
                    return false;
                if (!Regex.IsMatch(d, pattern))
                    return false;
                if (d != "0")
                {
                    d = d.TrimStart('0');
                    if (d == "")
                        return false;
                    if (int.Parse(d) > 255)
                        return false;
                }
            }

            return true;
        }
    }
    /// <summary>
    /// 能用，不用，目前多个IP容易改错，后续优化
    /// </summary>
    public class LocalNetHelper : ObservableObject
    {
        public string IPAddress
        {
            get => _iPAddress;
            set
            {
                _iPAddress = value;
                OnPropertyChanged();
            }
        }

        private string _iPAddress="172.32.0.3";

        public string SubnetMask
        {
            get => _SubnetMask;
            set
            {
                _SubnetMask = value;
                OnPropertyChanged();
            }
        }

        private string _SubnetMask="255.255.255.0";


        public string SetGateways
        {
            get => _SetGateways;
            set
            {
                _SetGateways = value;
                OnPropertyChanged();
            }
        }

        private string _SetGateways = "172.32.0.1";


        public RelayCommand SetNetworkCommand
        {
            get => _SetNetworkCommand;
            set
            {
                _SetNetworkCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _SetNetworkCommand;

        public LocalNetHelper()
        {
            SetNetworkCommand = new RelayCommand(SetNetworkAdapter);
        }

        public void SetNetworkAdapter()
        {
            ManagementBaseObject inPar = null;
            ManagementBaseObject outPar = null;
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (!(bool) mo["IPEnabled"])
                    continue;

                //设置ip地址和子网掩码
                inPar = mo.GetMethodParameters("EnableStatic");
                inPar["IPAddress"] = new string[] {IPAddress}; // 1.备用 2.IP
                inPar["SubnetMask"] = new string[] {SubnetMask};
                outPar = mo.InvokeMethod("EnableStatic", inPar, null);
                

                //设置网关地址
                inPar = mo.GetMethodParameters("SetGateways");
                inPar["DefaultIPGateway"] = new string[] {SetGateways}; // 1.网关;2.备用网关
                outPar = mo.InvokeMethod("SetGateways", inPar, null);

//                //设置DNS
//                inPar = mo.GetMethodParameters("SetDNSServerSearchOrder");
//                inPar["DNSServerSearchOrder"] = new string[] {"211.97.168.129", "202.102.152.3"}; // 1.DNS 2.备用DNS
//                outPar = mo.InvokeMethod("SetDNSServerSearchOrder", inPar, null);
                break;
            }
        }
    }

    public class TcpServerHelper : ValidateModelBase
    {
        public bool IsStarted
        {
            get => _isStarted;
            set
            {
                _isStarted = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }

        private int _responseTimeOut = 2;


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
            (Socket clientSocket, byte[] bytes) p = (clientSocket, bytes);
            var t = p.ToTuple();
            StrongReferenceMessenger.Default.Send(t, "ServerReceiveDataEvent");
        }


        protected virtual void OnSendBytesToClient(Socket clientSocket, byte[] bytes)
        {
            SendBytesToClient?.Invoke(clientSocket, bytes);
            (Socket clientSocket, byte[] bytes) p = (clientSocket, bytes);
            var t = p.ToTuple();
            StrongReferenceMessenger.Default.Send(t, "ServerSendDataEvent");
        }

        private void OnNotifyErrorMsg(string msg)
        {
            this.ErrorMsg?.Invoke(msg);

            StrongReferenceMessenger.Default.Send(msg, "ServerErrorEvent");
        }

        protected virtual void OnNotifyStatusMsg(string msg)
        {
            StrongReferenceMessenger.Default.Send(msg, "ServerStatus");
//            Messenger.Default.Send(msg, "ServerStatus");
            StatusMsg?.Invoke(msg);
        }

        public TcpServerHelper(string listenIpAddress, int listenPort)
        {
            ListenIpAddress = listenIpAddress;
            ListenPort = listenPort;
            SocketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IpEndPoint = new IPEndPoint(IPAddress.Parse(ListenIpAddress), ListenPort);
            SocketClientList = new ObservableCollection<Socket>();
        }

        public TcpServerHelper(string listenIpAddress, int listenPort, ProtocolType protocolType = ProtocolType.Tcp)
        {
            ListenIpAddress = listenIpAddress;
            ListenPort = listenPort;
            ProtocolType = protocolType;
            SocketClientList = new ObservableCollection<Socket>();
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
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            SpeechSynthesizer speech = new SpeechSynthesizer();
                            speech.SpeakAsync("有新的连接");
                            SocketClientList.Add(socket1);
                        });
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
        public async Task<byte[]> SendDataToClientAndWaitReceiveData(Socket destinationSocket, byte[] bytes)
        {
            return await Task.Run(async () =>
            {
                _returnBytes = null;
                ReceiveBytes += TcpServerHelper_ReceiveBytes;
                destinationSocket.Send(bytes);
                OnSendBytesToClient(destinationSocket, bytes);
                stopwatch1 = new Stopwatch();
               var nowtick= DateTime.Now.Ticks;
                TimeSpan startTimeSpan = new TimeSpan(DateTime.Now.Ticks);
                stopwatch1.Start();
               
                //占用大量CPU需要优化
                while (true)
                {await Task.Delay(100);
                    TimeSpan elapsed = new TimeSpan(DateTime.Now.Ticks- nowtick);
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

        private void TcpServerHelper_ReceiveBytes(Socket clientSocket, byte[] bytes)
        {
            if (bytes == null)
            {
                return;
            }

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
                    if (uInt16Length == (bytes.Length - 8))
                    {
                        _listReturnBytes.AddRange(bytes);
                        _returnBytes = _listReturnBytes.ToArray();

                        _listReturnBytes.Clear();
                        _isNeedContinue = false;
                    }

                    if (uInt16Length > (bytes.Length - 8))
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

            OnNotifyStatusMsg($"{DateTime.Now}执行关闭{SocketClientList[num].RemoteEndPoint}客户端操作{Environment.NewLine}");
            SocketClientList[num].Shutdown(SocketShutdown.Both);
            SocketClientList[num].Disconnect(reuseSocket: false);
            DispatcherHelper.CheckBeginInvokeOnUI(() => { SocketClientList.RemoveAt(num); });

            SocketClientCancellationTokens[num].Cancel();
            SocketClientCancellationTokens.RemoveAt(num);
        }
    }
}