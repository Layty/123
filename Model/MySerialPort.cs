using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using 三相智慧能源网关调试软件.Commom;

namespace 三相智慧能源网关调试软件.Model
{
    public class MySerialPort : ObservableObject
    {
        #region 串口基本参数

        /// <summary>
        /// 指示是否一致占用当前串口号
        /// </summary>
        public bool IsOwnCurrentSerialPort
        {
            get => _isOwnCurrentSerialPort;
            set
            {
                _isOwnCurrentSerialPort = value;
                RaisePropertyChanged();
            }
        }

        private bool _isOwnCurrentSerialPort;

        public SerialPort SerialPort { get; set; }

        /// <summary>
        /// 串口名
        /// 如果上一次的串口号已打开，设置该值后会先关闭先前的串口，然后默认执行打开当前的串口号
        /// </summary>
        [Required]
        public string PortName
        {
            get => SerialPort.PortName;
            set
            {
                if (SerialPort.IsOpen)
                {
                    SerialPort.Close();
                    IsOpen = false;
                    SerialPort.PortName = value;
                    SerialPort.Open();
                    IsOpen = true;
                    RaisePropertyChanged();
                }
                else
                {
                    SerialPort.PortName = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int BaudRate
        {
            get => SerialPort.BaudRate;
            set
            {
                SerialPort.BaudRate = value;
                RaisePropertyChanged();
            }
        }

        public StopBits StopBits
        {
            get => SerialPort.StopBits;
            set
            {
                SerialPort.StopBits = value;
                RaisePropertyChanged();
            }
        }

        public Parity Parity
        {
            get => SerialPort.Parity;
            set
            {
                SerialPort.Parity = value;
                RaisePropertyChanged();
            }
        }

        public int DataBits
        {
            get => SerialPort.DataBits;
            set
            {
                SerialPort.DataBits = value;
                RaisePropertyChanged();
            }
        }


        private bool _isOpen;

        public bool IsOpen
        {
            get => SerialPort.IsOpen;
            set
            {
                _isOpen = value;
                RaisePropertyChanged();
            }
        }

        private int _channelNum = 1;

        public int ChannelNum
        {
            get => _channelNum;
            set
            {
                _channelNum = value;
                RaisePropertyChanged();
            }
        }

        private int _delayTimeOut = 2;

        public int DelayTimeOut
        {
            get => _delayTimeOut;
            set
            {
                _delayTimeOut = value;
                RaisePropertyChanged();
            }
        }

        private bool _isAutoDataReceived;

        public bool IsAutoDataReceived
        {
            get => _isAutoDataReceived;
            set
            {
                if (_isAutoDataReceived == value)
                    return;
                if (value)
                {
                    SerialPort.DataReceived += SerialPort_DataReceived;
                }
                else
                {
                    SerialPort.DataReceived -= SerialPort_DataReceived;
                }

                _isAutoDataReceived = value;
                RaisePropertyChanged();
            }
        }


        private string _errMgs;

        public string ErrMgs
        {
            get => _errMgs;
            set
            {
                _errMgs = value;
                RaisePropertyChanged();
            }
        }

        private string ResponseTime { get; set; } //响应时间，发送帧后到接收到下一个帧头的时间

        #endregion

        #region 串口收发数据参数

        private bool _isSendFormat16 = true;

        /// <summary>
        /// 发送区是否16进制显示
        /// </summary>
        public bool IsSendFormat16
        {
            get => _isSendFormat16;
            set
            {
                _isSendFormat16 = value;
                RaisePropertyChanged();
            }
        }

        private bool _isSendDataForShowString;

        public bool IsSendDataShowString
        {
            get => _isSendDataForShowString;
            set
            {
                _isSendDataForShowString = value;
                RaisePropertyChanged();
            }
        }

        private byte[] _sendBytes;

        public byte[] SendBytes
        {
            get => _sendBytes;
            set
            {
                _sendBytes = value;
                RaisePropertyChanged();
            }
        }


        public string SendStringDataForShow
        {
            get
            {
                if (IsSendFormat16)
                {
                    return SendBytes.ByteToString();
                }

                return Encoding.ASCII.GetString(SendBytes);
            }
            set
            {
                if (IsSendFormat16)
                {
                    SendBytes = value.StringToByte();
                }
                else
                {
                    SendBytes = Encoding.ASCII.GetBytes(value);
                }

                RaisePropertyChanged();
            }
        }


        private int _sendFrameCount;

        /// <summary>
        /// 发送帧数
        /// </summary>
        public int SendFrameCount
        {
            get => _sendFrameCount;
            set
            {
                _sendFrameCount = value;
                RaisePropertyChanged();
            }
        }

        private int _sendBytesCount;

        /// <summary>
        /// 发送字节数
        /// </summary>
        public int SendBytesCount
        {
            get => _sendBytesCount;
            set
            {
                _sendBytesCount = value;
                RaisePropertyChanged();
            }
        }

        private int _keepMaxSendAndReceiveDataLength = 5000;

        /// <summary>
        /// 最大支持的串口历史记录帧长度
        /// </summary>
        public int KeepMaxSendAndReceiveDataLength
        {
            get => _keepMaxSendAndReceiveDataLength;
            set
            {
                _keepMaxSendAndReceiveDataLength = value;
                RaisePropertyChanged();
            }
        }

        public StringBuilder SendAndReceiveDataStringBuilderCollections = new StringBuilder();

        public string SendAndReceiveDataCollections
        {
            get => SendAndReceiveDataStringBuilderCollections.ToString() + Environment.NewLine;
            set
            {
                if (SendAndReceiveDataStringBuilderCollections.Length > _keepMaxSendAndReceiveDataLength)
                {
                    SendAndReceiveDataStringBuilderCollections.Clear();
                }

                SendAndReceiveDataStringBuilderCollections.Append(value);
                RaisePropertyChanged();
            }
        }


        private int _receiveFrameCount;

        /// <summary>
        /// 接收帧数
        /// </summary>
        public int ReceiveFrameCount
        {
            get => _receiveFrameCount;
            set
            {
                _receiveFrameCount = value;
                RaisePropertyChanged();
            }
        }

        private int _receiveBytesCount;

        /// <summary>
        /// 接收字节数
        /// </summary>
        public int ReceiveBytesCount
        {
            get => _receiveBytesCount;
            set
            {
                _receiveBytesCount = value;
                RaisePropertyChanged();
            }
        }


        private bool _isReceiveFormat16 = true;

        /// <summary>
        /// 接收区16进制
        /// </summary>
        public bool IsReceiveFormat16
        {
            get => _isReceiveFormat16;
            set
            {
                _isReceiveFormat16 = value;
                RaisePropertyChanged();
            }
        }


        private string _dataReceiveShowBytes;

        /// <summary>
        /// 接收区数据
        /// </summary>
        public string DataReceiveForShow
        {
            get => _dataReceiveShowBytes;
            set
            {
                _dataReceiveShowBytes = value;
                RaisePropertyChanged();
            }
        }


        private byte[] _dataBytes;

        public byte[] DataReceiveBytes
        {
            get => _dataBytes;
            set
            {
                if (IsReceiveFormat16)
                {
                }


                _dataBytes = value;
                RaisePropertyChanged();
            }
        }

        #endregion


        #region 串口参数配置、加载、保存

        /// <summary>
        /// 创建当前串口实例的参数配置类实例
        /// </summary>
        public SerialPortConfig CreateMySerialPortConfig =>
            new SerialPortConfig(SerialPort.PortName, SerialPort.BaudRate, SerialPort.StopBits, SerialPort.Parity,
                SerialPort.DataBits,
                DelayTimeOut, IsOwnCurrentSerialPort, IsAutoDataReceived);

        /// <summary>
        /// 导入串口的配置参数
        /// </summary>
        /// <param name="serialPortConfig"></param>
        public void LoadSerialPortConfig(SerialPortConfig serialPortConfig)
        {
            try
            {
                SerialPort.PortName = serialPortConfig.PortName;
                SerialPort.BaudRate = serialPortConfig.BaudRate;
                SerialPort.DataBits = serialPortConfig.DataBits;
                DelayTimeOut = serialPortConfig.DelayTimeOut;
                if (TryParseParity(serialPortConfig.Parity, out Parity parity)) SerialPort.Parity = parity;
                if (Enum.TryParse(serialPortConfig.StopBits, out StopBits stopBits))
                {
                    SerialPort.StopBits = stopBits;
                }

                SerialPort.DataBits = serialPortConfig.DataBits;
                DelayTimeOut = serialPortConfig.DelayTimeOut;
                IsOwnCurrentSerialPort = serialPortConfig.IsOwnThisSerialPort;
                IsAutoDataReceived = serialPortConfig.IsAutoDataReceived;
            }
            catch (Exception ex)
            {
                ErrMgs = ex.Message;
            }

            OnSerialPortConfigChanged(this, new MySerialEventArgs {ConfigMessage = "加载串口参数成功"});
        }

        #endregion


        private readonly object _objLocker = new object(); //发送锁
        private readonly Stopwatch _stopwatch1 = new Stopwatch();
        public bool IsNeedToResponse { get; set; } = true;
        public CancellationTokenSource ReceiveTokenSource;


        public MySerialPort()
        {
            SerialPort = new SerialPort();
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //ReceiveData();
            byte[] readBuffer = new byte[SerialPort.BytesToRead];
            SerialPort.Read(readBuffer, 0, readBuffer.Length);
            ReceiveFrameCount++;
            ReceiveBytesCount += readBuffer.Length;
            SendAndReceiveDataCollections = $"{DateTime.Now} <= {(readBuffer.ByteToString())}{Environment.NewLine}";
            if (IsReceiveFormat16)
            {
                //不做增量
                DataReceiveForShow = readBuffer.ByteToString();
            }
            else
            {
                DataReceiveForShow = Encoding.ASCII.GetString(readBuffer);
            }
        }

        /// <summary>
        /// 打开串口
        /// </summary>
        public void Open()
        {
            try
            {
                if (!SerialPort.IsOpen) SerialPort.Open();
                IsOpen = SerialPort.IsOpen;
            }
            catch (Exception ex)
            {
                ErrMgs = ex.Message;
                IsOpen = false;
                throw;
            }
        }

        /// <summary>
        /// 关闭串口
        /// </summary>
        /// <returns></returns>
        public void Close()
        {
            try
            {
                if (SerialPort.IsOpen)
                {
                    SerialPort.Close();
                }

                IsOpen = SerialPort.IsOpen;
            }
            catch (Exception ex)
            {
                ErrMgs = ex.Message;
                IsOpen = false;
            }
        }


        public void SendDataTaskMVVM()
        {
            lock (_objLocker)
            {
                try
                {
                    if (!IsOpen) SerialPort.Open(); //如果串口关闭则打开
                    SerialPort.Write(SendBytes, 0, SendBytes.Length); //发送数据 
                    SendFrameCount++;
                    SendBytesCount += SendBytes.Length;
                    Messenger.Default.Send("", "PlaySendFlashing");
                    SendAndReceiveDataCollections =
                        $"{DateTime.Now} => {(SendBytes.ByteToString())}{Environment.NewLine}";
                    _stopwatch1.Start();
                }
                catch (Exception ex)
                {
                    ErrMgs = ex.Message;
                    _stopwatch1?.Reset();
                    ReceiveTokenSource?.Cancel();
                }
            }
        }


        public override string ToString() =>
            $"{IsOpen}  {SerialPort.PortName}  {SerialPort.BaudRate}  {SerialPort.Parity}  {SerialPort.DataBits}  {SerialPort.StopBits}";


        public byte[] ReceiveData()
        {
            while (true)
            {
                Thread.Sleep(10);
                var n4 = SerialPort.BytesToRead;
                if (n4 != 0)
                {
                    while (true)
                    {
                        var n = SerialPort.BytesToRead;
                        Thread.Sleep(20);
                        var n1 = SerialPort.BytesToRead;
                        if (n != 0 && n1 == n)
                        {
                            //声明一个临时数组存储当前来的串口数据 
                            var receivedBytes = new byte[SerialPort.BytesToRead];
                            DataReceiveBytes = receivedBytes;
                            Messenger.Default.Send("", "PlayReceiveFlashing");
                            SerialPort.Read(receivedBytes, 0,
                                SerialPort.BytesToRead);
                            OnDataReceived(receivedBytes, null);
                            return receivedBytes;
                        }
                    }
                }

                break;
            }

            return null;
        }


        public byte[] TryToReadReceiveData()
        {
            byte[] tryToReadReceiveData = { };
            Stopwatch stopwatch1 = new Stopwatch();
            TimeSpan startTimeSpan = new TimeSpan(DateTime.Now.Ticks);
            stopwatch1.Start();
            while (true)
            {
                TimeSpan stopTimeSpan = new TimeSpan(DateTime.Now.Ticks);
                TimeSpan timeSpan = stopTimeSpan.Subtract(startTimeSpan).Duration();
                if (timeSpan.Seconds >= DelayTimeOut)
                {
                    ResponseTime = timeSpan.Seconds.ToString();
                    stopwatch1.Reset();
                    break;
                }

                if (SerialPort.BytesToRead != 0)
                {
                    stopwatch1.Stop();
                    ResponseTime = stopwatch1.ElapsedMilliseconds.ToString();
                    stopwatch1.Reset();

                    while (true)
                    {
                        var n = SerialPort.BytesToRead;
                        Thread.Sleep(50);
                        var n1 = SerialPort.BytesToRead;
                        if (n != 0 && n1 == n)
                        {
                            //声明一个临时数组存储当前来的串口数据 
                            tryToReadReceiveData = new byte[SerialPort.BytesToRead];
                            Messenger.Default.Send("", "PlayReceiveFlashing");
                            SerialPort.Read(tryToReadReceiveData, 0, SerialPort.BytesToRead);
                            DataReceiveForShow = tryToReadReceiveData.ByteToString();
                            SendAndReceiveDataCollections =
                                $"{DateTime.Now} <= {(tryToReadReceiveData.ByteToString())}{Environment.NewLine}";
                            OnDataReceived(tryToReadReceiveData, null);
                            break;
                        }
                    }

                    break;
                }
            }

            return tryToReadReceiveData;
        }

        private Task<byte[]> ReceiveDataAsync(CancellationToken token)
        {
            Task<byte[]> task = Task.Run(() =>
            {
                byte[] receivedBytes = { };
                while (true)
                {
                    if (SerialPort.BytesToRead != 0)
                    {
                        _stopwatch1.Stop();
                        ResponseTime = _stopwatch1.ElapsedMilliseconds.ToString();
                        _stopwatch1.Reset();
                        while (true)
                        {
                            var n = SerialPort.BytesToRead;
                            Thread.Sleep(50);
                            var n1 = SerialPort.BytesToRead;
                            if (n != 0 && n1 == n)
                            {
                                //声明一个临时数组存储当前来的串口数据 
                                receivedBytes = new byte[SerialPort.BytesToRead];
                                Messenger.Default.Send("", "PlayReceiveFlashing");
                                SerialPort.Read(receivedBytes, 0, SerialPort.BytesToRead);
                                DataReceiveForShow = receivedBytes.ByteToString();
                                SendAndReceiveDataCollections =
                                    $"{DateTime.Now} <= {(receivedBytes.ByteToString())}{Environment.NewLine}";
                                OnDataReceived(receivedBytes, ResponseTime);
                                break;
                            }
                        }

                        break;
                    }

                    if (token.IsCancellationRequested)
                    {
                        _stopwatch1.Reset();
                        OnDataReceivedTimeOut(DelayTimeOut);
                        break;
                    }
                }

                return receivedBytes;
            }, token);

            return task;
        }


        public async Task<byte[]> SendAndReceiveReturnData(byte[] sendData, bool cycle = false,
            bool overTimeMonitor = true)
        {
            var keep = false;
            if (IsAutoDataReceived == true)
            {
                keep = true;
                IsAutoDataReceived = false;
            }

            ReceiveTokenSource = new CancellationTokenSource(DelayTimeOut * 1000);
            SendDataTask(sendData);
            var receiveData = await ReceiveDataAsync(ReceiveTokenSource.Token);
            if (!IsOwnCurrentSerialPort) //是否强制占有当前串口，不是则关闭串口&&同时在不读后续帧的情况下不关闭串口
            {
                SerialPort.Close();
            }

            if (keep == true)
            {
                IsAutoDataReceived = true;
            }

            return receiveData;
        }


        public void SendDataWithLocker(byte[] sendData)
        {
            lock (_objLocker)
            {
                if (!IsOpen) SerialPort.Open();
                SerialPort.Write(sendData, 0, sendData.Length);
                Messenger.Default.Send("", "PlaySendFlashing");
                SendStringDataForShow = sendData.ByteToString();
                SendAndReceiveDataCollections =
                    $"{DateTime.Now} => {(sendData.ByteToString())}{Environment.NewLine}";
            }
        }

        public void SendDataTask(byte[] sendData)
        {
            lock (_objLocker)
            {
                try
                {
                    if (!IsOpen) SerialPort.Open(); //如果串口关闭则打开
                    Messenger.Default.Send("", "PlaySendFlashing");
                    SerialPort.Write(sendData, 0, sendData.Length); //发送数据 
                    _stopwatch1.Start();
                    SendStringDataForShow = sendData.ByteToString();
                    SendAndReceiveDataCollections =
                        $"{DateTime.Now} => {(sendData.ByteToString())}{Environment.NewLine}";
                
                }
                catch (Exception ex)
                {
                    ErrMgs = ex.Message;
                    _stopwatch1?.Reset();
                    ReceiveTokenSource?.Cancel();
                }
            }
        }


        public static bool TryParseParity(string inputParity, out Parity parity) =>
            Enum.TryParse(inputParity, out parity);

        public void StopBitsCheck(string inputStopBits)
        {
            try
            {
                if (Enum.TryParse(inputStopBits, out StopBits stopBits))
                {
                    SerialPort.StopBits = stopBits;
                }
            }
            catch (Exception ex)
            {
                ErrMgs = ex.Message;
            }
        }


        //public event MySerialEventHandler SendDate; //发送事件
        public event MySerialEventHandler MySerialDataReceived;
        public event MySerialEventHandler MySerialDataReceivedTimeOut; //接收超时事件
        public event MySerialEventHandler SerialError; //调用串口报错事件        
        public event MySerialEventHandler MySerialPortConfigChanged;


        private void OnDataReceivedTimeOut(int delayTimeOut)
        {
            MySerialDataReceivedTimeOut?.Invoke(this, new MySerialEventArgs {DelayTime = delayTimeOut});
        }


        private void OnDataReceived(byte[] receivedBytes, string responseTime)
        {
            MySerialDataReceived?.Invoke(this,
                new MySerialEventArgs
                    {DataBytes = receivedBytes, ResponseTime = responseTime, IsStringData = IsSendDataShowString});
        }

        private void OnSerialError(Exception ex)
        {
            SerialError?.Invoke(this, new MySerialEventArgs {Ex = ex});
        }

        private void OnSerialPortConfigChanged(MySerialPort source, MySerialEventArgs e)
        {
            MySerialPortConfigChanged?.Invoke(source, e);
        }
    }
}