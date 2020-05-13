using System;
using System.ComponentModel.DataAnnotations;
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
        /// 指示是否一致占用当前串口号的方式使用串口
        /// </summary>
        public bool IsOwnCurrentSerialPort { get; set; }


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

        #endregion


        #region 串口参数配置、加载、保存

        /// <summary>
        /// 创建当前串口实例的参数配置类实例
        /// </summary>
        public SerialPortConfig CreateMySerialPortConfig =>
            new SerialPortConfig(PortName, BaudRate, StopBits, Parity, DataBits, ResponseTimeOut,
                IsOwnCurrentSerialPort,
                IsAutoDataReceived);

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
            }
            catch (Exception ex)
            {
                OnSerialError(ex);
                // ErrMgs = ex.Message;
            }

            OnSerialPortConfigChanged(this, new SerialPortEventArgs {ConfigMessage = "加载串口参数成功"});
        }

        #endregion

        private readonly object _objLocker = new object(); //发送锁
        private readonly Stopwatch _stopwatch1 = new Stopwatch();
        public CancellationTokenSource ReceiveTokenSource;

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

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] readBuffer = new byte[SerialPort.BytesToRead];
            SerialPort.Read(readBuffer, 0, readBuffer.Length);
            OnDataReceived(readBuffer, null);
            SerialPortLogger.HandlerReceiveData(readBuffer);
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
                   // Messenger.Default.Send("", "PlaySendFlashing");
                    OnDataSend(sendBytes);
                    SerialPortLogger.HandlerSendData(sendBytes);
                }
                catch (Exception ex)
                {
                    ErrMgs = ex.Message;
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
                   // Messenger.Default.Send("", "PlaySendFlashing");
                    OnDataSend(sendBytes);
                    SerialPortLogger.HandlerSendData(sendBytes);
                }
                catch (Exception ex)
                {
                    ErrMgs = ex.Message;
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
                        Thread.Sleep(50);
                        var n1 = SerialPort.BytesToRead;
                        if (n != 0 && n1 == n)
                        {
                            //声明一个临时数组存储当前来的串口数据 
                            tryToReadReceiveData = new byte[SerialPort.BytesToRead];
                           
                            SerialPort.Read(tryToReadReceiveData, 0, SerialPort.BytesToRead);
                            SerialPortLogger.HandlerReceiveData(tryToReadReceiveData);
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
                                SerialPortLogger.HandlerReceiveData(receivedBytes);
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
            var receiveData = await ReceiveDataAsync(new CancellationTokenSource(ResponseTimeOut * 1000).Token);
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


        public event SerialPortMasterEventHandler SerialDataSend; //发送事件
        public event SerialPortMasterEventHandler SerialDataReceived;
        public event SerialPortMasterEventHandler SerialDataReceivedTimeOut; //接收超时事件
        public event SerialPortMasterEventHandler SerialError; //调用串口报错事件        
        public event SerialPortMasterEventHandler MySerialPortConfigChanged;


        private void OnDataSend(byte[] sendBytes)
        {
            Messenger.Default.Send("", "PlaySendFlashing");
            SerialDataSend?.Invoke(this,new SerialPortEventArgs(){DataBytes = sendBytes});

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
        }

        private void OnSerialError(Exception ex)
        {
            SerialError?.Invoke(this, new SerialPortEventArgs {Ex = ex});
        }

        private void OnSerialPortConfigChanged(SerialPortMaster source, SerialPortEventArgs e)
        {
            MySerialPortConfigChanged?.Invoke(source, e);
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