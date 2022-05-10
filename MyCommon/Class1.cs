using System;
using System.Text;
using CommunityToolkit.Diagnostics;

namespace MyCommon
{
    public class Class1
    {
    }

    public static class Extension
    {
        /// <summary>
        /// 比较两个字节数组是否一一对应相等
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        public static bool ByteArraysEqual(byte[] b1, byte[] b2)
        {
            if (b1 == b2) return true;
            if (b1 == null || b2 == null) return false;
            if (b1.Length != b2.Length) return false;
            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i]) return false;
            }

            return true;
        }

        ///  <summary>  
        /// 将指定字节数组转为16进制的字符串  
        ///  </summary>
        ///  <param name="inBytes"> 字节数组 </param>
        ///  <param name="separator">分隔符</param>
        ///  <returns>类似"01 02 0F" </returns>  
        public static string ByteArrayToHexString(this byte[] inBytes, string separator = " ")
        {
            Guard.IsNotNull(inBytes, nameof(inBytes));
            Guard.IsNotNull(separator, nameof(separator));

            StringBuilder stringBuilder = new StringBuilder();
            for (var i = 0; i < inBytes.Length; i++)
            {
                byte inByte = inBytes[i];
                if (i == (inBytes.Length - 1))
                {
                    stringBuilder.Append($"{inByte:X2}");
                }
                else
                {
                    stringBuilder.Append($"{inByte:X2}" + separator);
                }

            }


            return stringBuilder.ToString();
        }


        public static string ByteArrayToHexString(this byte[] inBytes, int index, int count, string separator = " ")
        {
            Guard.IsNotNull(inBytes, nameof(inBytes));

            Guard.IsGreaterThan(count, 0, nameof(count));
            Guard.IsLessThanOrEqualTo(count, inBytes.Length - index, nameof(count));

            Guard.IsBetweenOrEqualTo(index, 0, inBytes.Length - 1, nameof(index));
            Guard.IsNotNull(separator, nameof(separator));
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = index, j = 0; j < count; j++, i++)
            {
                byte inByte = inBytes[i];
                if (j == (count - 1))
                {
                    stringBuilder.Append($"{inByte:X2}");
                }
                else
                {
                    stringBuilder.Append($"{inByte:X2}" + separator);
                }

            }
            return stringBuilder.ToString();
        }

    }
}
