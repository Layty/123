using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace MySerialPortMaster
{
    public class SerialPortMaster : ObservableObject
    {
        #region 串口基本参数

        /// <summary>
        /// 指示是否以占用当前串口号的方式使用串口
        /// </summary>
        public bool IsOwnCurrentSerialPort { get; set; }


        /// <summary>
        /// 串口名
        /// 如果上一次的串口号已打开，设置该值后会先关闭先前的串口，然后默认执行打开当前的串口号
        /// </summary>
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
            private set
            {
                _isOpen = value;
                RaisePropertyChanged();
            }
        }


        private int _responseTimeOut = 2;

        public int ResponseTimeOut
        {
            get => _responseTimeOut;
            set
            {
                _responseTimeOut = value;
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

        private string _responseTime;

        /// <summary>
        /// 响应时间，计算发送帧后到接收到下一个帧头的时间
        /// </summary>
        public string ResponseTime
        {
            get => _responseTime;
            set
            {
                _responseTime = value;
                RaisePropertyChanged();
            }
        }


        public int Interval
        {
            get => _interval;
            set
            {
                _interval = value;
                RaisePropertyChanged();
            }
        }

        private int _interval;

        #endregion


        #region 串口参数配置、加载、保存

        /// <summary>
        /// 创建当前串口实例的参数配置类实例
        /// </summary>
        public SerialPortConfig CreateMySerialPortConfig =>
            new SerialPortConfig(PortName, BaudRate, StopBits, Parity, DataBits, ResponseTimeOut,
                IsOwnCurrentSerialPort,
                IsAutoDataReceived, Interval);

        /// <summary>
        /// 导入串口的配置参数
        /// </summary>
        /// <param name="serialPortConfig"></param>
        public void LoadSerialPortConfig(SerialPortConfig serialPortConfig)
        {
            try
            {
                PortName = serialPortConfig.PortName;
                BaudRate = serialPortConfig.BaudRate;
                DataBits = serialPortConfig.DataBits;
                ResponseTimeOut = serialPortConfig.DelayTimeOut;
                if (TryParseParity(serialPortConfig.Parity, out Parity parity)) Parity = parity;
                if (TryParseStopBits(serialPortConfig.StopBits, out StopBits stopBits)) StopBits = stopBits;
                IsOwnCurrentSerialPort = serialPortConfig.IsOwnThisSerialPort;
                IsAutoDataReceived = serialPortConfig.IsAutoDataReceived;
                Interval = serialPortConfig.Interval;
            }
            catch (Exception ex)
            {
                OnSerialError(ex);
            }
        }

        #endregion

        private readonly object _objLocker = new object(); //发送锁
        private readonly Stopwatch _stopwatch1 = new Stopwatch();
        public CancellationTokenSource ReceiveTokenSource { get; set; }

        private SerialPort SerialPort { get; set; }

        public SerialPortLogger SerialPortLogger { get; set; } = new SerialPortLogger();

        public SerialPortMaster()
        {
            SerialPort = new SerialPort();
            IsAutoDataReceived = true;
        }

        public SerialPortMaster(SerialPortConfig serialPortConfig)
        {
            SerialPort = new SerialPort();
            IsAutoDataReceived = true;
            LoadSerialPortConfig(serialPortConfig);
        }

        private async void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            while (SerialPort.BytesToRead != 0)
            {
                var n = SerialPort.BytesToRead;
                await Task.Delay(50);

                var n1 = SerialPort.BytesToRead;
                if (n != 0 && n1 == n)
                {
                    //声明一个临时数组存储当前来的串口数据 
                    var tryToReadReceiveData = new byte[SerialPort.BytesToRead];

                    SerialPort.Read(tryToReadReceiveData, 0, SerialPort.BytesToRead);

                    OnDataReceived(tryToReadReceiveData, null);
                    break;
                }
            }
        }

        #region 串口开启与关闭

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
                OnSerialError(ex);
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
                OnSerialError(ex);
                IsOpen = false;
            }
        }

        #endregion

        public void Send(byte[] sendBytes)
        {
            lock (_objLocker)
            {
                try
                {
                    if (!IsOpen) SerialPort.Open(); //如果串口关闭则打开
                    _stopwatch1.Restart();
                    SerialPort.Write(sendBytes, 0, sendBytes.Length); //发送数据 
                    OnDataSend(sendBytes);
                }
                catch (Exception ex)
                {
                    OnSerialError(ex);
                }
            }
        }

        public void Send(string sendString)
        {
            lock (_objLocker)
            {
                try
                {
                    if (!IsOpen) SerialPort.Open(); //如果串口关闭则打开
                    var sendBytes = Encoding.Default.GetBytes(sendString);
                    SerialPort.Write(sendBytes, 0, sendBytes.Length); //发送数据 
                    OnDataSend(sendBytes);
                }
                catch (Exception ex)
                {
                    OnSerialError(ex);
                }
            }
        }


        /// <summary>
        /// 使用定时器去接收串口数据
        /// </summary>
        /// <returns></returns>
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
                if (timeSpan.Seconds >= ResponseTimeOut)
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
                        Thread.Sleep(100);
                        var n1 = SerialPort.BytesToRead;
                        if (n != 0 && n1 == n)
                        {
                            //声明一个临时数组存储当前来的串口数据 
                            tryToReadReceiveData = new byte[SerialPort.BytesToRead];

                            SerialPort.Read(tryToReadReceiveData, 0, SerialPort.BytesToRead);

                            OnDataReceived(tryToReadReceiveData, null);
                            break;
                        }
                    }

                    break;
                }
            }

            return tryToReadReceiveData;
        }

        #region 自定义的发送并等待接收，加入超时判定，和可取消操作

        #endregion

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
                                SerialPort.Read(receivedBytes, 0, SerialPort.BytesToRead);
                                OnDataReceived(receivedBytes, ResponseTime);
                                break;
                            }
                        }

                        break;
                    }

                    if (token.IsCancellationRequested)
                    {
                        _stopwatch1.Reset();
                        OnDataReceivedTimeOut(ResponseTimeOut);
                        break;
                    }
                }

                return receivedBytes;
            }, token);

            return task;
        }


        public async Task<byte[]> SendAndReceiveReturnDataAsync(byte[] sendData)
        {
            //如果发送前属于自动接收串口数据，发送前先置false,等待接收完成后再将其设置为true
            var keepAutoDataReceived = false;
            if (IsAutoDataReceived)
            {
                keepAutoDataReceived = true;
                IsAutoDataReceived = false;
            }
            Send(sendData);
            ReceiveTokenSource = new CancellationTokenSource(ResponseTimeOut * 1000);
            var receiveData = await ReceiveDataAsync(ReceiveTokenSource.Token);
            if (!IsOwnCurrentSerialPort) //是否强制占有当前串口，不是则关闭串口&&同时在不读后续帧的情况下不关闭串口
            {
                SerialPort.Close();
            }

            if (keepAutoDataReceived)
            {
                IsAutoDataReceived = true;
            }

            return receiveData;
        }


        public static bool TryParseParity(string inputParity, out Parity parity) =>
            Enum.TryParse(inputParity, out parity);

        public static bool TryParseStopBits(string inputStopBits, out StopBits stopBits) =>
            Enum.TryParse(inputStopBits, out stopBits);

        /// <summary>
        /// 串口发送事件
        /// </summary>
        public event SerialPortMasterEventHandler SerialDataSend;

        /// <summary>
        /// 串口接收事件
        /// </summary>
        public event SerialPortMasterEventHandler SerialDataReceived;

        /// <summary>
        /// 接收超时事件
        /// </summary>
        public event SerialPortMasterEventHandler SerialDataReceivedTimeOut;

        /// <summary>
        /// 串口异常事件 
        /// </summary>
        public event SerialPortMasterEventHandler SerialError;


        private void OnDataSend(byte[] sendBytes)
        {
            Messenger.Default.Send("", "PlaySendFlashing");
            SerialDataSend?.Invoke(this, new SerialPortEventArgs() {DataBytes = sendBytes});
            SerialPortLogger.HandlerSendData(sendBytes);
        }

        private void OnDataReceivedTimeOut(int delayTimeOut)
        {
            SerialDataReceivedTimeOut?.Invoke(this, new SerialPortEventArgs {DelayTime = delayTimeOut});
        }


        private void OnDataReceived(byte[] receivedBytes, string responseTime)
        {
            Messenger.Default.Send("", "PlayReceiveFlashing");
            SerialDataReceived?.Invoke(this,
                new SerialPortEventArgs
                    {DataBytes = receivedBytes, ResponseTime = responseTime});
            SerialPortLogger.HandlerReceiveData(receivedBytes);
        }

        private void OnSerialError(Exception ex)
        {
            SerialPortLogger.ErrMgs = ex.Message;
            SerialPortLogger.Logger.Error(ex.Message);
            SerialError?.Invoke(this, new SerialPortEventArgs {Ex = ex});
        }


        public void DiscardBuffer()
        {
            SerialPort.DiscardInBuffer();
            SerialPort.DiscardOutBuffer();
        }

        public override string ToString() =>
            $"{IsOpen}  {SerialPort.PortName}  {SerialPort.BaudRate}  {SerialPort.Parity}  {SerialPort.DataBits}  {SerialPort.StopBits}";
    }
}