using System;

namespace 三相智慧能源网关调试软件.FileTransmit
{
    public delegate void SendToUartEventHandler(object sender,SendToUartEventArgs e);

    interface ITransmitUart
    {
       event SendToUartEventHandler SendToUartEvent;

        void ReceivedFromUart(byte[] data);
    }

    public class SendToUartEventArgs : EventArgs
    {
        private readonly byte[] _Data;

        public SendToUartEventArgs(byte[] data)
        {
            _Data = data;
        }

        public byte[] Data
        {
            get { return _Data; }
        }
    }
}
