using System;
using System.Collections.Generic;
using System.Linq;
using MyDlmsNetCore.ApplicationLay;

namespace MyDlmsNetCore.Wrapper
{
    public class HeartBeatFrame : IToPduBytes, IPduBytesToConstructor
    {
        public byte[] VersionBytes { get; set; }
        public byte[] SourceAddressBytes { get; set; }
        public byte[] DestinationAddressBytes { get; set; }
        public byte[] LengthBytes { get; set; }
        public byte[] HeartBeatFrameType { get; set; }
        public byte[] MeterAddressBytes { get; set; }

        public HeartBeatFrame()
        {
            VersionBytes = new byte[] {0x00, 0x02};
            HeartBeatFrameType = new byte[] {0x00, 0x01, 0x03};
        }

        public void OverturnDestinationSource()
        {
            var tm = DestinationAddressBytes;
            DestinationAddressBytes = SourceAddressBytes;
            SourceAddressBytes = tm;
        }

        public byte[] ToPduBytes()
        {
            List<byte> list = new List<byte>();
            list.AddRange(VersionBytes);
            list.AddRange(SourceAddressBytes);
            list.AddRange(DestinationAddressBytes);
            var len = HeartBeatFrameType.Length + MeterAddressBytes.Length;
            list.AddRange(BitConverter.GetBytes((ushort) len).Reverse().ToArray());
            list.AddRange(HeartBeatFrameType);
            list.AddRange(MeterAddressBytes);
            return list.ToArray();
        }

        public bool PduBytesToConstructor(byte[] bytes)
        {
            if (bytes == null || bytes.Length <= 11)
            {
                return false;
            }

            //比对版本号

            if (!Common.Common.ByteArraysEqual(bytes.Take(2).ToArray(), VersionBytes))
            {
                return false;
            }

            SourceAddressBytes = bytes.Skip(2).Take(2).ToArray();
            DestinationAddressBytes = bytes.Skip(4).Take(2).ToArray();
            LengthBytes = bytes.Skip(6).Take(2).ToArray();
            var length = BitConverter.ToUInt16(LengthBytes.Reverse().ToArray(), 0);
            if (bytes.Skip(8).ToArray().Length != length)
                return false;
            var data = bytes.Skip(8).ToArray();
            if (!Common.Common.ByteArraysEqual(data.Take(3).ToArray(), HeartBeatFrameType))
                return false;
            MeterAddressBytes = data.Skip(3).Take(length - 3).ToArray();

            return true;
        }
    }
}