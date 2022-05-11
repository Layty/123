using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MySerialPortMaster
{
    public class SerialPortMaster : INotifyPropertyChanged
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

                    SerialPort.PortName = value;
                    SerialPort.Open();

                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsOpen));
                }
                else
                {
                    SerialPort.PortName = value;
                    OnPropertyChanged();
                }
            }
        }

        public int BaudRate
        {
            get => SerialPort.BaudRate;
            set
            {
                SerialPort.BaudRate = value;
                OnPropertyChanged();
            }
        }

        public StopBits StopBits
        {
            get => SerialPort.StopBits;
            set
            {
                SerialPort.StopBits = value;
                OnPropertyChanged();
            }
        }

        public Parity Parity
        {
            get => SerialPort.Parity;
            set
            {
                SerialPort.Parity = value;
                OnPropertyChanged();
            }
        }

        public int DataBits
        {
            get => SerialPort.DataBits;
            set
            {
                SerialPort.DataBits = value;
                OnPropertyChanged();
            }
        }

        public bool IsOpen => SerialPort.IsOpen;

        private int _responseTimeOut = 2;

        public int ResponseTimeOut
        {
            get => _responseTimeOut;
            set
            {
                _responseTimeOut = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }

        public int Interval
        {
            get => _interval;
            set
            {
                _interval = value;
                OnPropertyChanged();
            }
        }

        private int _interval;

        #endregion 串口基本参数

        #region 串口参数配置、加载、保存
        /// <summary>
        /// 创建当前串口实例的参数配置类实例
        /// </summary>
        public SerialPortConfig CreateCurrentSerialPortConfig =>
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
                throw;
            }
        }

        #endregion 串口参数配置、加载、保存

        private readonly object _objLocker = new object(); //发送锁
        private readonly Stopwatch _stopwatch1 = new Stopwatch();

        /// <summary>
        /// 处于长时间接受时，可以被外部取消
        /// </summary>
        public CancellationTokenSource ReceiveTokenSource { get; set; }

        private SerialPort SerialPort { get; set; } = new SerialPort();

        public SerialPortLogger SerialPortLogger { get; set; } = new SerialPortLogger();

        public SerialPortMaster()
        {
            IsAutoDataReceived = true;
            SerialPortConfigCaretaker = new SerialPortConfigCaretaker();
            LoadSerialPortConfig(SerialPortConfigCaretaker.DefaultConfig);
        }

        public SerialPortMaster(SerialPortConfig serialPortConfig)
        {
            if (serialPortConfig == null) throw new ArgumentNullException(nameof(serialPortConfig));
            IsAutoDataReceived = true;
            SerialPortConfigCaretaker = new SerialPortConfigCaretaker();
            LoadSerialPortConfig(serialPortConfig);

        }




        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                //if (SerialPort.BytesToRead != 0)
                {
                    //var n = SerialPort.BytesToRead;
                    //Thread.Sleep(50);//最大帧接受间隔

                    var n1 = SerialPort.BytesToRead;

                    if (n1 != 0)
                    {

                        //声明一个临时数组存储当前来的串口数据
                        var tryToReadReceiveData = new byte[n1];
                        if (IsOpen)
                        {
                            try
                            {
                                SerialPort.Read(tryToReadReceiveData, 0, n1);
                            }
                            catch
                            {
                                // ignored
                            }
                        }

                        OnDataReceived(tryToReadReceiveData, null);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
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
            }
            catch (Exception ex)
            {
                OnSerialError(ex);
                throw;
            }
            finally
            {
                OnPropertyChanged(nameof(IsOpen));
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
            }
            catch (Exception ex)
            {
                OnSerialError(ex);
                throw;
            }
            finally
            {
                OnPropertyChanged(nameof(IsOpen));
            }
        }

        #endregion 串口开启与关闭

        public void Send(byte[] sendBytes)
        {
            lock (_objLocker)
            {
                try
                {
                    if (!IsOpen) SerialPort.Open(); //如果串口关闭则打开
                    DiscardInOutBuffer();
                    _stopwatch1.Restart();//开启秒表计时
                    SerialPort.Write(sendBytes, 0, sendBytes.Length); //发送数据
                    OnDataSend(sendBytes);
                }
                catch (Exception ex)
                {
                    OnSerialError(ex);
                    throw;
                }
            }
        }

        public void Send(string sendString)
        {
            var sendBytes = Encoding.Default.GetBytes(sendString);
            Send(sendBytes);
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

        private Task<byte[]> ReceiveDataAsync(CancellationToken token)
        {
            Task<byte[]> task = Task.Run(async () =>
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
                            await Task.Delay(100);
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

                    await Task.Delay(100);
                    if (token.IsCancellationRequested)
                    {//进入这里代表被手动取消
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
                OnPropertyChanged(nameof(IsOpen));
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
            SerialDataSend?.Invoke(this, new SerialPortEventArgs() { DataBytes = sendBytes });
            SerialPortLogger.HandlerSendData(sendBytes);
        }

        private void OnDataReceivedTimeOut(int delayTimeOut)
        {
            SerialDataReceivedTimeOut?.Invoke(this, new SerialPortEventArgs { DelayTime = delayTimeOut });
            SerialPortLogger.HandlerTimeOut(ResponseTimeOut);
        }

        private void OnDataReceived(byte[] receivedBytes, string responseTime)
        {
            SerialDataReceived?.Invoke(this,
                new SerialPortEventArgs
                { DataBytes = receivedBytes, ResponseTime = responseTime });
            SerialPortLogger.HandlerReceiveData(receivedBytes);
        }

        private void OnSerialError(Exception ex)
        {
            SerialPortLogger.ErrMgs = ex.Message;
            SerialPortLogger.Logger.Error(ex.Message);
            SerialError?.Invoke(this, new SerialPortEventArgs { Ex = ex });
        }

        public void DiscardInOutBuffer()
        {
            //发送前清空buffer,原因因为发送器串口有可能在抖动误触产生杂乱字节数据
            SerialPort.DiscardInBuffer();
            SerialPort.DiscardOutBuffer();
        }

        public override string ToString() =>
            $"{IsOpen}  {SerialPort.PortName}  {SerialPort.BaudRate}  {SerialPort.Parity}  {SerialPort.DataBits}  {SerialPort.StopBits}";

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SerialPortConfigCaretaker SerialPortConfigCaretaker { get; set; }
    }
}