﻿using System;
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
using NLog.LayoutRenderers.Wrappers;

namespace 三相智慧能源网关调试软件
{
    public delegate void NotifyTcpServerMsgEventHandler(string message);

    public delegate void ReceiveDataFromClientEventHandler(Socket clientSocket, byte[] bytes);

    public delegate void SendDataToClientEventHandler(Socket clientSocket, byte[] bytes);

    public class TcpServerHelper : ViewModelBase
    {
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


        public event NotifyTcpServerMsgEventHandler ErrorMsg;
        public event NotifyTcpServerMsgEventHandler StatusMsg;
        public event ReceiveDataFromClientEventHandler ReceiveBytes;
        public event SendDataToClientEventHandler SendBytesToClient;

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
            returnBytes = null;
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

                    if (returnBytes != null)
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
            return returnBytes;
        }

        public string ResponseTime { get; set; }

        private byte[] returnBytes;
        private List<byte> listReturnByteses = new List<byte>();
        private bool isNeedContinue = false;
        private int totalLength { get; set; }
        private int needReceiveLength { get; set; }

        private void TcpServerHelper_ReceiveBytes(Socket clientSocket, byte[] bytes)
        {
            if (bytes == null)
            {
                return;
            }

            if (!isNeedContinue)
            {
                if (bytes.Length < 7)
                {
                    Logger lgLogger = LogManager.GetCurrentClassLogger();
                    lgLogger.Debug("This Is Not 47Message Should Never Enter Here");
                    listReturnByteses.AddRange(bytes);
                    returnBytes = listReturnByteses.ToArray();
                    listReturnByteses.Clear();
                }
                else
                {
                    if (bytes[7] == (bytes.Length - 8))
                    {
                        listReturnByteses.AddRange(bytes);
                        returnBytes = listReturnByteses.ToArray();
                        listReturnByteses.Clear();
                        isNeedContinue = false;
                    }

                    if (bytes[7] > (bytes.Length - 8))
                    {
                        totalLength = bytes[7];
                        needReceiveLength = totalLength - (bytes.Length - 8);
                        listReturnByteses.AddRange(bytes);
                        isNeedContinue = true;
                    }
                }
            }
            else
            {
                if (bytes.Length < needReceiveLength)
                {
                    needReceiveLength = needReceiveLength - bytes.Length;
                    listReturnByteses.AddRange(bytes);
                }

                if (bytes.Length >= needReceiveLength)
                {
                    needReceiveLength = 0;
                    isNeedContinue = false;
                    listReturnByteses.AddRange(bytes);
                    returnBytes = listReturnByteses.ToArray();
                    listReturnByteses.Clear();
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
    }
}