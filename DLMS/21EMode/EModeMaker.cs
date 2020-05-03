using System.Collections.Generic;
using System.Text;

namespace 三相智慧能源网关调试软件.DLMS._21EMode
{
    public class EModeMaker
    {
        private readonly EMode _eMode;

        public EModeMaker(int requestBaud, string deviceStr)
        {
            _eMode = new EMode(requestBaud, deviceStr);
        }

        public byte[] GetRequestFrameBytes()
        {
            string s = '/' + "?" + _eMode.Devicestr + "!\r\n";
            return Encoding.Default.GetBytes(s);
        }

        public byte[] GetConfirmFrameBytes()
        {
            List<byte> list = new List<byte>
            {
                6
            };
            string s = "2" + _eMode._baudZ + "2\r\n";
            list.AddRange(Encoding.Default.GetBytes(s));
            return list.ToArray();
        }
    }
}