using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Sockets;
using MyDlmsStandard.Wrapper;


namespace 三相智慧能源网关调试软件
{
    public class TcpTranslator : ValidateModelBase
    {
        public TcpServerHelper TcpListener { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        public int LocalPort
        {
            get => _localPort;
            set
            {
                _localPort = value;
                OnPropertyChanged();
            }
        }

        private int _localPort;
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
        [Required(ErrorMessage = "不能为空！")]
        [RegularExpression("^((2(5[0-5]|[0-4]\\d))|[0-1]?\\d{1,2})(\\.((2(5[0-5]|[0-4]\\d))|[0-1]?\\d{1,2})){3}$$",
            ErrorMessage = "请输入正确的IP地址！")]
        public string RemoteIp
        {
            get => _remoteIp;
            set
            {
                _remoteIp = value;
                OnPropertyChanged();
            }
        }

        private string _remoteIp;
        [Required(ErrorMessage = "不能为空！")]
        public int RemotePort
        {
            get => _remotePort;
            set
            {
                _remotePort = value;
                OnPropertyChanged();
            }
        }

        private int _remotePort;


        public bool IsNeedToConvert12HeartBeatTo8
        {
            get => _isNeedToConvert12HeartBeatTo8;
            set
            {
                _isNeedToConvert12HeartBeatTo8 = value;
                OnPropertyChanged();
            }
        }

        private bool _isNeedToConvert12HeartBeatTo8;

        /// <summary>
        /// 表端Socket 为key====>服务端的socket绑定 value
        /// </summary>
        public IDictionary<Socket, TcpClientHelper> SocketBindingDictionary { get; set; } =
            new Dictionary<Socket, TcpClientHelper>();

        /// <summary>
        /// 需要12位转8位时，存储 12位表地址的高四位地址
        /// </summary>
        private readonly IDictionary<Socket, byte[]> _meterHeight4ByteDictionary = new Dictionary<Socket, byte[]>();

        public TcpTranslator()
        {
            RemoteIp = LocalIp = Properties.Settings.Default.GatewayIpAddress;
            RemotePort = LocalPort = Properties.Settings.Default.GatewayPort;
        }

        public TcpTranslator(string localIp, int localPort, string remoteIp, int remotePort)
        {
            LocalIp = localIp;
            LocalPort = localPort;
            RemoteIp = remoteIp;
            RemotePort = remotePort;
        }

        /// <summary>
        /// 开启转发的监听，当接收到客户端连接，则新建一个socket 连接至远端IP
        /// </summary>
        public void StartListen()
        {
            TcpListener = new TcpServerHelper(LocalIp, LocalPort);
            TcpListener.AcceptNewClient += TcpListener_AcceptNewClient;
            TcpListener.ReceiveBytes += TcpListener_ReceiveBytes;
            TcpListener.StartListen();
        }

        /// <summary>
        /// 当接收到客户端连接，则新建一个客户端socket   连接至远端IP
        /// 是用字典进行关联绑定
        /// </summary>
        /// <param name="obj"></param>
        private void TcpListener_AcceptNewClient(Socket obj)
        {
            TcpClientHelper tcpClientHelper = new TcpClientHelper(RemoteIp, RemotePort);
            tcpClientHelper.ConnectToServer();
            SocketBindingDictionary[obj] = tcpClientHelper;

            tcpClientHelper.ReceiveDataEvent += TcpClientHelperReceiveDataEvent;
        }

        /// <summary>
        /// 本地服务器收到表的数据。
        /// </summary>
        /// <param name="meterSocket">表的socket</param>
        /// <param name="arg2"></param>
        private void TcpListener_ReceiveBytes(Socket meterSocket, byte[] arg2)
        {
            foreach (var socket in SocketBindingDictionary)
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
                            if (len == 0x0F) //12位转8位
                            {
                                _meterHeight4ByteDictionary[meterSocket] =
                                    frame.MeterAddressBytes.Take(4).ToArray(); //保留高位4地址后续用于补全
                                frame.MeterAddressBytes = frame.MeterAddressBytes.Skip(4).ToArray();
                                arg2 = frame.ToPduBytes();
                            }
                        }
                    }

                    socket.Value?.SendDataToServer(arg2);
                }
            }
        }

        private void TcpClientHelperReceiveDataEvent(Socket arg1, byte[] arg2)
        {
            foreach (var socket in SocketBindingDictionary)
            {
                if (arg1 == socket.Value.ClientSocket)
                {
                    if (IsNeedToConvert12HeartBeatTo8)
                    {
                        var frame = new HeartBeatFrame();
                        var t = frame.PduBytesToConstructor(arg2);
                        if (t)
                        {
                            var len = BitConverter.ToInt16(frame.LengthBytes.Reverse().ToArray(), 0);
                            if (len == 0x0B)
                            {
                                // 8位转12位
                                var list = new List<byte>();
                                list.AddRange(_meterHeight4ByteDictionary[socket.Key]); //添加原保留的高位地址
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

            SocketBindingDictionary.Clear();
        }
    }
}