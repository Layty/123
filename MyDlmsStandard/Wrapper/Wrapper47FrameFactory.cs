using MyDlmsStandard.Common;

namespace MyDlmsStandard.Wrapper
{
    //47协议帧工厂
    public static class Wrapper47FrameFactory
    {
        public static HeartBeatFrame CreateHeartBeatFrame(byte[] bytes)
        {
            var pduStringInHex = bytes.ByteToString();
            var heartBeatFrame = new HeartBeatFrame();
            if (heartBeatFrame.PduStringInHexConstructor(ref pduStringInHex))
            {
                return heartBeatFrame;
            }

            return null;
        }
        public static WrapperFrame CreateWrapperFrame(byte[] bytes)
        {
            var pduStringInHex = bytes.ByteToString();
            var wrapperFrame = new WrapperFrame();
            if (wrapperFrame.PduStringInHexConstructor(ref pduStringInHex))
            {
                return wrapperFrame;
            }

            return null;
        }
    }
}