using System;

namespace FileTransmit
{
    public delegate void PacketEventHandler(object sender, PacketEventArgs e);

   public interface IFileTransmit : ITransmitUart
    {
        event EventHandler StartSend;
        event EventHandler StartReceive;
        event EventHandler SendNextPacket;
        event EventHandler ReSendPacket;
        event EventHandler AbortTransmit;
        event EventHandler TransmitTimeOut;
        event EventHandler EndOfTransmit;

        event PacketEventHandler ReceivedPacket;

        void SendPacket(PacketEventArgs packet);
        void Start();
        void Stop();
        void Abort();

        bool IsStart { get; }
    }

    public class PacketEventArgs : EventArgs
    {
        public int PacketNo { get; }

        public int PacketLen { get; }

        public byte[] Packet { get; }

        public PacketEventArgs(int packetNo, byte[] packet)
            : this(packetNo, packet, packet.Length)
        {
        }

        public PacketEventArgs(int packetNo, byte[] packet, int packetLen)
        {
            PacketNo = packetNo;

            if (packet != null)
            {
                if (packet.Length <= packetLen)
                {
                    PacketLen = packetLen;
                }

                Packet = new byte[PacketLen];
                Array.Copy(packet, 0, Packet, 0, PacketLen);
            }
        }
    }
}