using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 三相智慧能源网关调试软件.DLMS.Common
{
    public class MyConvert
    {
        public static string ByteArrayToOctetString(byte[] bArray)
        {
            if (bArray == null || bArray.Length == 0)
            {
                return "";
            }

            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in bArray)
            {
                stringBuilder.Append(b.ToString("X2"));
            }

            return stringBuilder.ToString();
        }

        public static string ByteArrayToOctetString(byte[] bArray, char sp)
        {
            if (bArray == null || bArray.Length == 0)
            {
                return "";
            }

            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in bArray)
            {
                stringBuilder.Append(b.ToString("X2"));
                stringBuilder.Append(sp);
            }

            return stringBuilder.ToString(0, stringBuilder.Length - 1);
        }

        public static string ByteArrayToOctetString(byte[] bArray, int offset, int count, char sp)
        {
            if (bArray == null || bArray.Length == 0)
            {
                return "";
            }

            if (offset + count > bArray.Length)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = offset; i < count + offset; i++)
            {
                stringBuilder.Append(bArray[i].ToString("X2"));
                stringBuilder.Append(sp);
            }

            return stringBuilder.ToString(0, stringBuilder.Length - 1);
        }

        public static byte[] OctetStringToByteArray(string s)
        {
            int num = s.Length / 2;
            byte[] array = new byte[num];
            for (int i = 0; i < num; i++)
            {
                array[i] = Convert.ToByte(s.Substring(i * 2, 2), 16);
            }

            return array;
        }

        public static string OctetStringToString(string s)
        {
            int num = s.Length / 2;
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < num; i++)
            {
                byte value = Convert.ToByte(s.Substring(i * 2, 2), 16);
                stringBuilder.Append(Convert.ToChar(value));
            }

            return stringBuilder.ToString();
        }

        public static string StringToOctetString(string s)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                stringBuilder.Append(Convert.ToByte(s[i]).ToString("X2"));
            }

            return stringBuilder.ToString();
        }

        public static string StringToOctetString(string s, char filter)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string[] array = s.Split(filter);
            for (int i = 0; i < array.Length; i++)
            {
                stringBuilder.Append(Convert.ToByte(array[i]).ToString("X2"));
            }

            return stringBuilder.ToString();
        }

        public static string EncodeVarLength(int qty)
        {
            if (qty <= 127)
            {
                return qty.ToString("X2");
            }

            return "82" + qty.ToString("X4");
        }

        public static int DecodeVarLength(ref string s)
        {
            string value = s.Substring(0, 2);
            int num = Convert.ToInt32(value, 16);
            if (num < 128)
            {
                s = s.Substring(2);
                return num;
            }

            switch (num)
            {
                case 129:
                    value = s.Substring(2, 2);
                    s = s.Substring(4);
                    return Convert.ToInt32(value, 16);
                case 130:
                    value = s.Substring(2, 4);
                    s = s.Substring(6);
                    return Convert.ToInt32(value, 16);
                default:
                    return -1;
            }
        }

        public static bool VarLengthStringConstructor(ref string varLengthString, out string constructorValue)
        {
            constructorValue = null;
            string s = varLengthString;
            int num = DecodeVarLength(ref s);
            if (num < 0)
            {
                return false;
            }

            if (s.Length < num * 2)
            {
                return false;
            }

            constructorValue = s.Substring(0, num * 2);
            varLengthString = varLengthString.Substring((num + 1) * 2);
            return true;
        }

        public static string[] StringSplitWithSpecLength(string s, int specLength)
        {
            List<string> list = new List<string>();
            int num;
            for (num = s.Length; num >= specLength; num -= specLength)
            {
                list.Add(s.Substring(s.Length - num, specLength));
            }

            if (num > 0)
            {
                list.Add(s.Substring(s.Length - num, num));
            }

            return list.ToArray();
        }

        public static string ObisToHexCode(string s)
        {
            string[] array = s.Split('.');
            if (array.Length != 6)
            {
                return "";
            }

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < 6; i++)
            {
                if (!byte.TryParse(array[i], out byte result))
                {
                    return "";
                }

                stringBuilder.Append(result.ToString("X2"));
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

        public static string UInt32ToIpAddress(uint u)
        {
            StringBuilder stringBuilder = new StringBuilder();
            byte[] bytes = BitConverter.GetBytes(u);
            for (int num = bytes.Length - 1; num >= 0; num--)
            {
                stringBuilder.Append(bytes[num].ToString());
                if (num != 0)
                {
                    stringBuilder.Append(".");
                }
            }

            return stringBuilder.ToString();
        }
    }
}