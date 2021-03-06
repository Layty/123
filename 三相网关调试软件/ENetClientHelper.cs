using ENet;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using 三相智慧能源网关调试软件.Properties;

namespace 三相智慧能源网关调试软件
{
    public class ENetClientHelper : ObservableObject
    {
        private Host _host;
        private Peer _peer;
        private int _serverPortNum;

        public int ServerPortNum
        {
            get => _serverPortNum;
            set
            {
                _serverPortNum = value;
                OnPropertyChanged();
            }
        }

        private string _serverIpAddress;

        public string ServerIpAddress
        {
            get => _serverIpAddress;
            set
            {
                _serverIpAddress = value;
                OnPropertyChanged();
            }
        }

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

        private string _statusString;

        public string StatusString
        {
            get => _statusString;
            set
            {
                _statusString = value;
                OnPropertyChanged();
            }
        }

        private int _timeOut;

        public int TimeOut
        {
            get => _timeOut;
            set
            {
                _timeOut = value;
                OnPropertyChanged();
            }
        }

        private bool _connectResult;

        public bool ConnectResult
        {
            get => _connectResult;
            set
            {
                _connectResult = value;
                OnPropertyChanged();
            }
        }

        public ENetClientHelper()
        {
            TimeOut = Settings.Default.GatewayConnectTimeout;
            ServerIpAddress = Settings.Default.GatewayIpAddress;
            ServerPortNum = Settings.Default.GatewayPort;
            IpEndPoint = new IPEndPoint(IPAddress.Parse(ServerIpAddress), ServerPortNum);
            // Task.Run(Server);//开启本地服务端
        }

        private void SaveGateWayInfo()
        {
            Settings.Default.GatewayConnectTimeout = TimeOut;
            Settings.Default.GatewayIpAddress = ServerIpAddress;
            Settings.Default.GatewayPort = ServerPortNum;
            Settings.Default.Save();
        }


        public void ConnectToServer()
        {
            try
            {
                if (ConnectResult)
                {
                    return;
                }

                _host = new Host();
                _host.InitializeClient(1);
                _peer = _host.Connect(ServerIpAddress, ServerPortNum, 1, 200);
                _host.Service(TimeOut, out var @event);
                if (@event.Type == EventType.Connect)
                {
                    ConnectResult = true;
                    StrongReferenceMessenger.Default.Send($"成功连接至{_peer.GetRemoteAddress()}", "Status");
                    SaveGateWayInfo(); //保存信息
                    Task.Run(ReceiveData);
                }
                else
                {
                    ConnectResult = false;
                    StrongReferenceMessenger.Default.Send("连接不成功", "Status");
                }
            }
            catch (Exception e)
            {
                ConnectResult = false;
                StrongReferenceMessenger.Default.Send("异常" + e.Message, "ENetErrorEvent");
                throw;
            }
        }

        public void SendDataToServer(string inputSendData)
        {
            if (ConnectResult == false)
            {
                return;
            }

            if (string.IsNullOrEmpty(inputSendData))
            {
                return;
            }

            try
            {
                byte[] sendBytes = Encoding.Default.GetBytes(inputSendData);
                _host.CheckEvents(out var @event);
                _peer.Send(@event.ChannelID, sendBytes, PacketFlags.Reliable);
                StrongReferenceMessenger.Default.Send(sendBytes, "SendDataEvent");
            }
            catch (Exception ex)
            {
                StrongReferenceMessenger.Default.Send(ex.Message, "ENetErrorEvent");
            }
        }

        private void ReceiveData()
        {
            for (; ConnectResult;)
            {
                try
                {
                    _host.CheckEvents(out var @event);
                    _host.Service(TimeOut, out @event);
                    if (@event.Type == EventType.Receive)
                    {
                        byte[] data = @event.Packet.GetBytes();
                        StrongReferenceMessenger.Default.Send(data, "ReceiveDataEvent");
                        StrongReferenceMessenger.Default.Send(data, "ENetReceiveDataEvent");
                        @event.Packet.Dispose();
                    }
                    else if (@event.Type == EventType.Disconnect)
                    {
                        StrongReferenceMessenger.Default.Send("ServerMakeMeDisConnect", "Status");
                        @event.Packet.Dispose();
                        @event.Peer.DisconnectNow(1);
                        ConnectResult = false;
                        break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    break;
                }
            }

            Console.WriteLine(@"I am Outside");
        }

        public void DisconnectToServer()
        {
            if (ConnectResult == false)
            {
                return;
            }

            _peer.DisconnectNow(1);
            StrongReferenceMessenger.Default.Send($"我主动断开与服务端{_peer.GetRemoteAddress()}的连接", "Status");
            ConnectResult = false;
        }


        #region ServerAndClientDemoCode

        public static void Server()
        {
            using (var host = new Host())
            {
                Console.WriteLine(@"Initializing server...");

                host.InitializeServer(8001, 1);

                var peer = new Peer();

                while (true)
                {
                    try
                    {
                        if (host.Service(15000, out var @event))
                        {
                            do
                            {
                                switch (@event.Type)
                                {
                                    case EventType.Connect:
                                        peer = @event.Peer;
                                        StrongReferenceMessenger.Default.Send($"本地服务端收到{peer.GetRemoteAddress()}连接",
                                            "Status");
                                        break;

                                    case EventType.Receive:
                                        byte[] data = @event.Packet.GetBytes();
                                        peer.Send(@event.ChannelID, data, PacketFlags.Reliable);
                                        string @string = Encoding.Default.GetString(data);
                                        byte[] bytes = Convert.FromBase64String(@string);

                                        StrongReferenceMessenger.Default.Send(Encoding.Default.GetString(bytes),
                                            "Status");

                                        @event.Packet.Dispose();
                                        break;
                                    case EventType.Disconnect:
                                        StrongReferenceMessenger.Default.Send($"客户端{peer.GetRemoteAddress()}主动断开",
                                            "Status");
                                        break;
                                }
                            } while (host.CheckEvents(out @event));
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        break;
                    }
                }
            }
        }

        public static void Client()
        {
            using (Host host = new Host())
            {
                Console.WriteLine(@"Initializing client...");
                host.Initialize(null, 1);
                Peer peer = host.Connect("127.0.0.1", 5000, 1234, 200);
                while (true)
                {
                    if (host.Service(15000, out var @event))
                    {
                        do
                        {
                            switch (@event.Type)
                            {
                                case EventType.Connect:
                                    Console.WriteLine(@"Connected to server at IP/port {0}.", peer.GetRemoteAddress());
                                    string sendDate = "{\"cmd_type\":22,\"data\":\"0\"}";
                                    var b = Encoding.Default.GetBytes(sendDate);
                                    peer.Send(@event.ChannelID, b, PacketFlags.Reliable);
                                    @event.Packet.Dispose();
                                    break;

                                case EventType.Receive:
                                    byte[] data = @event.Packet.GetBytes();

                                    Console.WriteLine(@"  Client: Ch={0} Recv={1}", @event.ChannelID,
                                        Encoding.Default.GetString(data));

                                    //ushort value = BitConverter.ToUInt16(data, 0);
                                    //if (value % 1000 == 0)
                                    //{
                                    //    Console.WriteLine(@"  Client: Ch={0} Recv={1}", @event.ChannelID, value);
                                    //}

                                    //value++;
                                    //peer.Send(@event.ChannelID, BitConverter.GetBytes(value),
                                    //    ENet.PacketFlags.Reliable);
                                    var BB = Encoding.Default.GetBytes("helllll");
                                    peer.Send(@event.ChannelID, BB,
                                        PacketFlags.Reliable);
                                    @event.Packet.Dispose();
                                    break;

                                case EventType.None:
                                    break;
                                case EventType.Disconnect:
                                    break;
                                default:
                                    Console.WriteLine(@event.Type);
                                    break;
                            }
                        } while (host.CheckEvents(out @event));
                    }
                }
            }
        }

        #endregion
    }
}