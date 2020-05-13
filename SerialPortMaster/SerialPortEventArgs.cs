using System;

namespace MySerialPortMaster
{
    /// <inheritdoc />
    /// <summary>
    /// 表示包含串口事件的类，用于传递串口事件的相关参数
    /// </summary>
    public class SerialPortEventArgs : EventArgs
    {
        public byte[] DataBytes; //串口的发送或接受数据；
        public Exception Ex; //串口类的异常信息
        public int DelayTime; //串口抄读后最大等待数据响应的时间
        public bool OverTimeMonitor; //用于区分是否为主动读取指令，如果主动读取则关注超时事件
        public string ResponseTime; //抄读后到接收返回数据之间的时间间隔，接收后到发送响应数据之间的时间间隔
        public string ConfigMessage;
    }
}