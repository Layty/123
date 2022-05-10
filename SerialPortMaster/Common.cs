using System;
using System.Text;

namespace MySerialPortMaster
{
    internal static class Common
    {
        ///  <summary>
        /// 将指定字节数组转为16进制的字符串
        ///  </summary>
        ///  <param name="inBytes"> 字节数组 </param>
        ///  <param name="separator">分隔符</param>
        ///  <returns>类似"01 02 0F" </returns>
        public static string ByteToString(this byte[] inBytes, string separator = " ")
        {
            if (inBytes == null)
            {
                throw new ArgumentNullException(nameof(inBytes), @"不能将空数组转换为16进制字符串");
            }
            if (separator == null)
            {
                throw new ArgumentNullException(nameof(separator), "分隔符不能为空");
            }
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
    }
}