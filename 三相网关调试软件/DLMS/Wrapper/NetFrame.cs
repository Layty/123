using System;
using System.Collections.Generic;
using System.Linq;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;

namespace 三相智慧能源网关调试软件.DLMS.Wrapper
{
    public class NetFrame:IToPduBytes
    {
        public byte[] Version { get; set; }
        public byte[] SourceAddress { get; set; }
        public byte[] DestAddress { get; set; }
        public byte[] Length { get;private set; }
        public byte[] DLMSApduDataBytes { get; set; }

        public NetFrame()
        {
            
        }

        public NetFrame(MyDLMSSettings dlmsSettings, byte[] dlmsApduDataBytes)
        {
            Version = new byte[] {0x00, 0x01};
            SourceAddress = BitConverter.GetBytes(dlmsSettings.ClientAddress).Reverse().ToArray();
            DestAddress = BitConverter.GetBytes(dlmsSettings.ServerAddress).Reverse().ToArray();
            Length = BitConverter.GetBytes((ushort) dlmsApduDataBytes.Length).Reverse().ToArray();
            DLMSApduDataBytes = dlmsApduDataBytes;
        }
        public byte[] ToPduBytes()
        {
            List<byte> netFrame = new List<byte>();
            netFrame.AddRange(Version);
            netFrame.AddRange(SourceAddress);
            netFrame.AddRange(DestAddress);
            Length= BitConverter.GetBytes((ushort)DLMSApduDataBytes.Length).Reverse().ToArray();
            netFrame.AddRange(Length);
            netFrame.AddRange(DLMSApduDataBytes);
            return netFrame.ToArray();
        }
    }
}