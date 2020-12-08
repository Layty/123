using System.Collections.Generic;
using System.Linq;
using MyDlmsStandard.Wrapper;

namespace DataNotification.Model
{
    /// <summary>
    /// 网关心跳帧，继承自WrapperFrame
    /// </summary>
    public class HeartBeatFrame : WrapperFrame
    {
        private readonly byte[] _heartBeatFrameType = { 0x00, 0x01, 0x03 };
        public byte[] MeterAddressBytes { get; set; }

        public HeartBeatFrame()
        {
        }

        public new string ToPduStringInHex()
        {
            List<byte> d = new List<byte>();
            d.AddRange(_heartBeatFrameType);
            d.AddRange(MeterAddressBytes);
            WrapperData = d.ToArray();
            return base.ToPduStringInHex();
        }


        public new bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            if (!base.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            if (WrapperHeader.Version.Value != "0002")
            {
                return false;
            }

            if (!MyDlmsStandard.Common.Common.ByteArraysEqual(WrapperData.Take(3).ToArray(), _heartBeatFrameType))
            {
                return false;
            }

            MeterAddressBytes = WrapperData.Skip(3).Take(WrapperHeader.Length.GetEntityValue() - 3).ToArray();

            return true;
        }
    }
}