using System.IO.Ports;

namespace MySerialPortMaster
{
    /// <summary>
    /// 串口需要备份的参数类
    /// </summary>
    public struct SerialPortConfig
    {
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public string Parity { get; set; }
        public int DataBits { get; set; }
        public string StopBits { get; set; }
        public int DelayTimeOut { get; set; }
        public bool IsOwnThisSerialPort { get; set; }
        public bool IsAutoDataReceived { get; set; }

        public int Interval { get; set; }
        public SerialPortConfig(string portName, int baudRate, StopBits stopBits, Parity parity, int dataBits,
            int delayTimeOut, bool isOwnThisSerialPort, bool isAutoDataReceived,int interval)
        {
            PortName = portName;
            BaudRate = baudRate;
            Parity = parity.ToString();
            DataBits = dataBits;
            StopBits = stopBits.ToString();
            DelayTimeOut = delayTimeOut;
            IsOwnThisSerialPort = isOwnThisSerialPort;
            IsAutoDataReceived = isAutoDataReceived;
            Interval = interval;
        }
    }
}