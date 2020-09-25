using System;
using System.Collections.Generic;
using System.Linq;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;

namespace 三相智慧能源网关调试软件.DLMS.Wrapper
{
    public class NetFrame : IToPduBytes
    {
        public byte[] Version { get; set; }
        public byte[] SourceAddress { get; set; }
        public byte[] DestAddress { get; set; }
        byte[] Length { get; set; }
        public byte[] DLMSApduDataBytes { get; set; }

        public NetFrame()
        {
        }

        public byte[] ToPduBytes()
        {
            List<byte> netFrame = new List<byte>();
            netFrame.AddRange(Version);
            netFrame.AddRange(SourceAddress);
            netFrame.AddRange(DestAddress);
            netFrame.AddRange(BitConverter.GetBytes((ushort) DLMSApduDataBytes.Length).Reverse().ToArray());
            netFrame.AddRange(DLMSApduDataBytes);
            return netFrame.ToArray();
        }
    }
}