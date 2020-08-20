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
using GalaSoft.MvvmLight.Messaging;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace 三相智慧能源网关调试软件
{
    public class TcpClientHelper : ObservableObject, IDataErrorInfo
    {
        public Socket ClientSocket { get; set; }
        private readonly byte[] _messageByteServer = new byte[1024];
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
                        Messenger.Default.Send($"{ClientSocket.LocalEndPoint}正在尝试连接{ClientSocket.RemoteEndPoint}",
                            "Status");
                        ClientSocket.Connect(ServerIpAddress, ServerPortNum);

                        ConnectResult = true;
                        Messenger.Default.Send($"成功连接至{ClientSocket.RemoteEndPoint}", "Status");
                        Task.Run(ReceiveData);
                    }
                    catch (Exception e)
                    {
                        ConnectResult = false;
                        Messenger.Default.Send("连接不成功 from  ConnectToServer()", "Status");
                        Messenger.Default.Send("异常" + e.Message + "from  ConnectToServer()", "TelNetErrorEvent");
                    }
                });
            }
            catch (Exception e)
            {
                ConnectResult = false;
                Messenger.Default.Send("异常" + e.Message + "from  ConnectToServer()", "TelNetErrorEvent");
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
                Messenger.Default.Send(e.Message + "from  ReceiveData()", "TelNetErrorEvent");
            }
            finally
            {
                ClientSocket?.Disconnect(false);
                ConnectResult = false;
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
        private Dictionary<string, string> dataErrors = new Dictionary<string, string>();

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