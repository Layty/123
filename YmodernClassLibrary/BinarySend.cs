using System;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace FileTransmit
{
    public class BinarySend : IFileTransmit, ITransmitUart
    {
        public bool IsStart { get; private set; }
        private int DelayTime = 10;
        public BinarySend(int delayTime)
        {
            DelayTime = delayTime;
        }


        private void SendThreadHandler()
        {
            while (IsStart)
            {
                if (SendNextPacket != null)
                {
                    SendNextPacket(this, null);
                    Thread.Sleep(DelayTime);
                }
            }
        }

        #region IFileTransmit ��Ա

        public event EventHandler StartSend;

        public event EventHandler StartReceive;

        public event EventHandler SendNextPacket;

        public event EventHandler ReSendPacket;

        public event EventHandler AbortTransmit;

        public event EventHandler TransmitTimeOut;

        public event EventHandler EndOfTransmit;

        public event PacketEventHandler ReceivedPacket;

        public void SendPacket(PacketEventArgs packet)
        {
            SendToUartEvent?.Invoke(null, new SendToUartEventArgs(packet.Packet));
        }

        public void Start()
        {
            IsStart = true;
            Task.Run(SendThreadHandler);
        }

        public void Stop()
        {
            IsStart = false;
            EndOfTransmit?.Invoke(this, null);
        }

        public void Abort()
        {
            IsStart = false;
            AbortTransmit?.Invoke(this, null);
        }

        #endregion

        #region ITransmitUart ��Ա

        public event SendToUartEventHandler SendToUartEvent;

        public void ReceivedFromUart(byte[] data)
        {
            Console.WriteLine("�����Ʒ������账�����");
        }

        #endregion


    }
}
