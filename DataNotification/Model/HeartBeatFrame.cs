using System.Collections.Generic;
using System.Linq;
using MyDlmsStandard.Axdr;
using MyDlmsStandard.Wrapper;

namespace DataNotification.Model
{
    /// <summary>
    /// 网关心跳帧，继承自WrapperFrame
    /// </summary>
    public class HeartBeatFrame : WrapperFrame
    {
        /// <summary>
        /// 定义了必须为0x00, 0x01, 0x03
        /// </summary>
        public readonly byte[] HeartBeatFrameFlag = {0x00, 0x01, 0x03};

        public byte[] MeterAddressBytes { get; set; }

        protected HeartBeatFrame()
        {
            this.WrapperHeader = new WrapperHeader()
            {
                Version = new AxdrIntegerUnsigned16("0002"), DestAddress = new AxdrIntegerUnsigned16("0016"),
                SourceAddress = new AxdrIntegerUnsigned16("0002"),Length = new AxdrIntegerUnsigned16("000F")
            };
        }

        /// <summary>
        /// 重写父类对WrapperBody的赋值
        /// </summary>
        /// <returns></returns>
        public override string ToPduStringInHex()
        {
            //将WrapperBody用_heartBeatFrameType+MeterAddressBytes替代
            List<byte> d = new List<byte>();
            d.AddRange(HeartBeatFrameFlag);
            d.AddRange(MeterAddressBytes);
            WrapperBody.DataBytes = d.ToArray();
            return base.ToPduStringInHex();
        }

        /// <summary>
        /// 重写对父类解析判断方法
        /// </summary>
        /// <param name="pduStringInHex"></param>
        /// <returns></returns>
        public override bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            //使用父类的判定方法
            if (!base.PduStringInHexConstructor(ref pduStringInHex))
            {
                return false;
            }

            // 子类的自定义要求， 要求版本必须为0002
            if (WrapperHeader.Version.Value != "0002")
            {
                return false;
            }

            //要求{0x00, 0x01, 0x03}
            if (!MyDlmsStandard.Common.Common.ByteArraysEqual(WrapperBody.DataBytes.Take(3).ToArray(),
                HeartBeatFrameFlag))
            {
                return false;
            }

            MeterAddressBytes = WrapperBody.DataBytes.Skip(3).Take(WrapperHeader.Length.GetEntityValue() - 3).ToArray();

            return true;
        }

        public static HeartBeatFrame ParseHeartBeatFrame(ref string pduStringInHex)
        {
            var heartBeatFrame = new HeartBeatFrame();

            if (heartBeatFrame.PduStringInHexConstructor(ref pduStringInHex))
            {
                return heartBeatFrame;
            }

            return null;
        }
    }
}