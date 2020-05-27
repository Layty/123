using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;

namespace 三相智慧能源网关调试软件
{
    public delegate void NotifyTcpServerMsgEventHandler(string message);

    public delegate void ReceiveDataFromClientEventHandler(Socket clientSocket, byte[] bytes);

    public delegate void SendDataToClientEventHandler(Socket clientSocket, byte[] bytes);

    public class TcpServerHelper : ViewModelBase
    {
        public int ListenPortNum
        {
            get => _listenPortNum;
            set
            {
                _listenPortNum = value;
                RaisePropertyChanged();
            }
        }

        private int _listenPortNum;


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

        public TcpServerHelper(string listenIpAddress, int listenPortNum)
        {
            ListenIpAddress = listenIpAddress;
            ListenPortNum = listenPortNum;
            SocketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            SocketClientList = new ObservableCollection<Socket>();
        }

        public TcpServerHelper(string listenIpAddress, int listenPortNum, ProtocolType protocolType = ProtocolType.Tcp)
        {
            ListenIpAddress = listenIpAddress;
            ListenPortNum = listenPortNum;
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
            IpEndPoint = new IPEndPoint(IPAddress.Parse(ListenIpAddress), ListenPortNum);
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
                        DispatcherHelper.CheckBeginInvokeOnUI(()=> { SocketClientList.Add(socket1); });
                        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                        SocketClientCancellationTokens.Add(cancellationTokenSource);
                        OnNotifyStatusMsg($"{DateTime.Now}有新的连接{clientSocket.RemoteEndPoint}");
                        var socket = clientSocket;
                        Task.Run(delegate { ClientThread(socket); }, cancellationTokenSource.Token);
                    }
                    catch (Exception ex)
                    {
                        OnNotifyErrorMsg("serverTask" + ex.Message);
                        CloseSever();
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
                    OnNotifyErrorMsg("clientThread" + ex.Message + "\r\n");
                    DispatcherHelper.CheckBeginInvokeOnUI(() => { SocketClientList.Remove(sockClient); });
                   
                    break;
                }

                if (num == 0)
                {
                    OnNotifyStatusMsg($"客户端{sockClient.RemoteEndPoint} 断开了\r\n");
                    DispatcherHelper.CheckBeginInvokeOnUI(() => { SocketClientList.Remove(sockClient); });
                    
                    break;
                }
            }

            sockClient.Close();
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

        public void CloseSever()
        {
            if (SocketServer != null )
            {
               // _sourceServer.Cancel();
                SocketServer.Close();
                SocketServer.Dispose();
                OnNotifyStatusMsg(DateTime.Now + "已关闭监听" + Environment.NewLine);
            }

            if (SocketClientList.Count != 0)
            {
                foreach (Socket socketClient in SocketClientList)
                {
                    socketClient.Shutdown(SocketShutdown.Both);
                    socketClient.Close();
                }

                foreach (CancellationTokenSource socketClientCancellationToken in SocketClientCancellationTokens)
                {
                    socketClientCancellationToken.Cancel();
                }
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