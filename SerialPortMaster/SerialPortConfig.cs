using System.IO.Ports;
using Newtonsoft.Json;

namespace MySerialPortMaster
{
    /// <summary>
    /// 串口需要备份的参数类
    /// </summary>
    public class SerialPortConfig
    {
        //不允许被外部修改,由于需要被序列话，加上JsonProperty
        [JsonProperty]
        public string PortName { get; private set; }
        [JsonProperty]
        public int BaudRate { get; private set; }
        [JsonProperty]
        public string Parity { get; private set; }
        [JsonProperty]
        public int DataBits { get; private set; }
        [JsonProperty] public string StopBits { get; private set; }
        [JsonProperty] public int DelayTimeOut { get; private set; }
        [JsonProperty] public bool IsOwnThisSerialPort { get; private set; }
        [JsonProperty] public bool IsAutoDataReceived { get; private set; }

        [JsonProperty] public int Interval { get; private set; }


        //只通过构造传入
        public SerialPortConfig(string portName, int baudRate, StopBits stopBits, Parity parity, int dataBits,
            int delayTimeOut, bool isOwnThisSerialPort, bool isAutoDataReceived, int interval)
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