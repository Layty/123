using System.Linq;
using MyDlmsStandard.Common;

namespace 三相智慧能源网关调试软件.Helpers
{
    public static class SinglePhaseGatewayManagementHelper
    {
        public static byte[] GetConnect()
        {
            return "55 22 11 22 0D 00 01 31 32 33 34 35 36 37 38 AD E4".StringToByte();
        }
        public static bool ParseConnectResult(byte[] dataBytes)
        {
            //55 44 33 44 0C 00 01FF 00 00 00 00 01 01 BE AB
            //= 0x00：密码错误
            //    = 0x01：其他通道正在连接
            //    = 0xFF：连接成功
            var p = new byte[] { 0x55, 0x44, 0x33, 0x44, 0x0C, 0x00, 0x01,0xFF };
            for (int i = 0; i <= 7; i++)
            {
                if (dataBytes[i]!= p[i])
                {
                    if (dataBytes[7]==0xff)
                    {
                        return true;
                    }
                    return false;
                }
            }

            return true;
          

        }

        public static byte[] GetVersion()
        {
            return "55 22 11 22 06 00 0B 00 07 9C".StringToByte();
        }
        /// <summary>
        /// 这里只取软件版本号
        /// </summary>
        /// <param name="dataBytes"></param>
        /// <returns></returns>
        public static string ParseVersion(byte[] dataBytes)
        {
            // "55 44 33 44 0A 00 0B 01 3E 01 00 00 A0 E7",, 01 3E==代表 1,62
            var mainVersion = dataBytes.Skip(7).Take(1).ToArray()[0];
            var subVersion = dataBytes.Skip(8).Take(1).ToArray()[0];
            return $"{mainVersion}.{subVersion}";
        }
    }
}
