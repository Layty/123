namespace MySerialPortMaster
{
    public delegate void SerialPortMasterEventHandler(SerialPortMaster source, SerialPortEventArgs e);
    public delegate void SerialPortMasterSendDataEventHandler(SerialPortMaster source, SerialPortEventArgs e);
    public delegate void SerialPortMasterReceiveDataEventHandler(SerialPortMaster source, SerialPortEventArgs e);
    public delegate void SerialPortMasterErrorEventHandler(SerialPortMaster source, SerialPortEventArgs e);
}