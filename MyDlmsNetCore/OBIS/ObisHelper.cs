using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyDlmsNetCore.OBIS
{
    public static class ObisHelper
    {
        public static byte[] ObisStringToBytes(string obisString)
        {
            string[] strings = obisString.Split('.').ToArray();
            if (strings.Length != 6)
            {
                return null;
            }

            List<byte> obisList = new List<byte>();
            foreach (string str in strings)
            {
                if (!byte.TryParse(str, out byte b))
                {
                    return null;
                }

                obisList.Add(b);
                //将OBIS字符串转成字节数组
            }

            return obisList.ToArray();
        }

        public static string ObisToHexCode(string s)
        {
            string[] array = s.Split(new char[]
            {
                '.'
            });
            if (array.Length != 6)
            {
                return "";
            }

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < 6; i++)
            {
                byte b;
                if (!byte.TryParse(array[i], out b))
                {
                    return "";
                }

                stringBuilder.Append(b.ToString("X2"));
            }

            return stringBuilder.ToString();
        }


        public static string GetObisOriginal(string hexCode)
        {
            if (string.IsNullOrEmpty(hexCode) || hexCode.Length != 12)
            {
                return "";
            }

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < 6; i++)
            {
                string value = hexCode.Substring(i * 2, 2);
                stringBuilder.Append(Convert.ToByte(value, 16).ToString());
                stringBuilder.Append(".");
            }

            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            return stringBuilder.ToString();
        }

        public static bool StringIsObis(string s)
        {
            string[] array = s.Split(new char[]
            {
                '.'
            });
            if (array.Length != 6)
            {
                return false;
            }

            for (int i = 0; i < array.Length; i++)
            {
                byte b;
                if (!byte.TryParse(array[i], out b))
                {
                    return false;
                }
            }

            return true;
        }

        //模糊匹配
        public static bool VagueMatchObis(string matchedObis, string vagueObis)
        {
            string[] array = matchedObis.Split(new char[]
            {
                '.'
            });
            string[] array2 = vagueObis.Split(new char[]
            {
                '.'
            });
            if (array2.Length != array.Length || array2.Length != 6)
            {
                return false;
            }

            for (int i = 0; i < 6; i++)
            {
                if (array[i] != array2[i] && array2[i] != "*")
                {
                    return false;
                }
            }

            return true;
        }
    }
}