using MyDlmsStandard.Axdr;
using MyDlmsStandard.Common;

namespace MyDlmsStandard.Wrapper
{
    public interface IWrapper47Builder
    {
        WrapperFrame SetDestAddress(AxdrIntegerUnsigned16 destAddress);
        WrapperFrame SetSourceAddress(AxdrIntegerUnsigned16 sourceAddress);
        WrapperFrame SetVersion(AxdrIntegerUnsigned16 version);

    } 
    public class HeartBeatFrameBuilder: IWrapper47Builder
    {
        private HeartBeatFrame heartBeatFrame=new HeartBeatFrame();
        public WrapperFrame SetDestAddress(AxdrIntegerUnsigned16 destAddress)
        {
            heartBeatFrame.WrapperHeader.DestAddress = destAddress;
            return heartBeatFrame;
        }

        public WrapperFrame SetSourceAddress(AxdrIntegerUnsigned16 sourceAddress)
        {
            heartBeatFrame.WrapperHeader.SourceAddress = sourceAddress;
            return heartBeatFrame;
        }

        public WrapperFrame SetVersion(AxdrIntegerUnsigned16 version)
        {
            heartBeatFrame.WrapperHeader.Version = version;
            return heartBeatFrame;
        }
    }
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