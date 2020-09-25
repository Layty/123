using System.Linq;
using System.Text;

namespace 三相智慧能源网关调试软件.DLMS.HDLC.IEC21EMode
{
    public class EModeParser
    {
        public static bool CheckServerFrameWisEquals2(byte[] receiveBytes)
        {
            if (receiveBytes.Length == 0)
            {
                return false;
            }
            if (receiveBytes[0] != EModeFrame.StartChar)
            {
                return false;
            }

            if (receiveBytes[receiveBytes.Length - 2] != EModeFrame.CompletCr)
            {
                return false;
            }
            if (receiveBytes[receiveBytes.Length - 1] != EModeFrame.CompletLf)
            {
                return false;
            }
            if (receiveBytes.Contains((byte)92))
            {
                string @string = Encoding.Default.GetString(receiveBytes);
                string[] array = @string.Split('\\');
                EModeFrame.PropMaxBaud = int.Parse(array[0].Substring(array[0].Length - 1, 1));
                if (array[1].Substring(0, 1) != "2")
                {
                    return false;
                }
            }
            return true;
        }

        public static bool CheckServerFrameZisEqualsClient(byte[] receiveBytes)
        {
            if (receiveBytes.Length == 0)
            {
                return false;
            }
            if (receiveBytes[0] != EModeFrame.Ack)
            {
                return false;
            }
            if (receiveBytes[receiveBytes.Length - 2] != EModeFrame.CompletCr)
            {
                return false;
            }
            if (receiveBytes[receiveBytes.Length - 1] != EModeFrame.CompletLf)
            {
                return false;
            }
            return true;
        }
    }
}