using System.Text;

namespace 三相智慧能源网关调试软件.DLMS.HDLC.IEC21EMode
{
    public class EModeFrameMaker
    {
        private readonly EModeFrame _eModeFrame;

        public EModeFrameMaker(int requestBaud, string deviceStr)
        {
            _eModeFrame = new EModeFrame(requestBaud, deviceStr);
        }

        public byte[] GetRequestFrameBytes()
        {
            string s = EModeFrame.StartChar.ToString() + EModeFrame.RequestChar + _eModeFrame.DeviceAddress + EModeFrame.EndChar +
                       EModeFrame.CompletCr + EModeFrame.CompletLf;
            return Encoding.Default.GetBytes(s);
        }

        public byte[] GetConfirmFrameBytes()
        {
            string s = EModeFrame.Ack + "2" + _eModeFrame._baudZ + "2" + EModeFrame.CompletCr + EModeFrame.CompletLf;
            return Encoding.Default.GetBytes(s);
        }
    }
}