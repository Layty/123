using System;

namespace FileTransmit
{
    public delegate void SendToUartEventHandler(object sender, SendToUartEventArgs e);

    public interface ITransmitUart
    {
        event SendToUartEventHandler SendToUartEvent;

        void ReceivedFromUart(byte[] data);
    }

    public class SendToUartEventArgs : EventArgs
    {
        public SendToUartEventArgs(byte[] data)
        {
            Data = data;
        }

        public byte[] Data { get; }
    }
}