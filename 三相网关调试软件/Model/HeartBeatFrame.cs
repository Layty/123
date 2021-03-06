using MyDlmsStandard.Axdr;
using MyDlmsStandard.Wrapper;
using System.Collections.Generic;
using System.Linq;

namespace 三相智慧能源网关调试软件.Model
{


    /// <summary>
    /// 网关心跳帧，继承自WrapperFrame
    /// </summary>
    public class HeartBeatFrame : WrapperFrame
    {
        public readonly byte[] HeartBeatFrameType = { 0x00, 0x01, 0x03 };
        public byte[] MeterAddressBytes { get; set; }

        public HeartBeatFrame(byte[] meterAddressBytes) : this()
        {
            MeterAddressBytes = meterAddressBytes;
        }

        public HeartBeatFrame()
        {
            this.WrapperHeader = new WrapperHeader()
            {
                DestAddress = new AxdrIntegerUnsigned16("0001"),
                SourceAddress = new AxdrIntegerUnsigned16("0001"),
                Version = new AxdrIntegerUnsigned16("0002")
            };
        }

        public override string ToPduStringInHex()
        {
            List<byte> list = new List<byte>();
            list.AddRange(HeartBeatFrameType);
            list.AddRange(MeterAddressBytes);
            WrapperBody.DataBytes = list.ToArray();
            return base.ToPduStringInHex();
        }


        public override bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            if (!base.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            if (WrapperHeader.Version.Value != "0002")
            {
                return false;
            }

            if (!MyDlmsStandard.Common.Common.ByteArraysEqual(WrapperBody.DataBytes.Take(3).ToArray(),
                HeartBeatFrameType))
            {
                return false;
            }

            MeterAddressBytes = WrapperBody.DataBytes.Skip(3).Take(WrapperHeader.Length.GetEntityValue() - 3).ToArray();

            return true;
        }
    }
}