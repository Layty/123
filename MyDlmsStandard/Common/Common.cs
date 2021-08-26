using System;
using System.Dynamic;
using System.Text;

namespace MyDlmsStandard.Common
{
    public static class Common
    {
        /// <summary>
        /// 将制定16进制字符串转换为字节数组
        /// </summary>
        /// <param name="putStringData"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static byte[] StringToByte(this string putStringData)
        {
            //去掉空格，后以每两个字符进行分割到数组里
            try
            {
                if (putStringData == null)
                {
                    return null;
                }

                putStringData = putStringData.Replace(" ", "");
                var outByteData = new byte[putStringData.Length / 2];
                for (int i = 0, k = 0; i < putStringData.Length / 2; i++, k += 2)
                {
                    outByteData[i] = Convert.ToByte(putStringData.Substring(k, 2), 16);
                }

                return outByteData;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        ///  <summary>  
        /// 普通常规字符串 转16进制字符串  
        ///  </summary>  
        ///  <param name="inString">unico </param>  
        ///  <returns>类似“01 0f” </returns>  
        public static string Str_To_0X(this string inString)
        {
            return ByteToString(Encoding.Default.GetBytes(inString));
        }

        ///  <summary>  
        /// 将指定字节数组转为16进制的字符串  
        ///  </summary>  
        ///  <param name="inBytes"> 二进制字节 </param>
        ///  <param name="insertString">间隔中插入的字符串，默认不插入，一般情况可输入空格进行输出" "</param>
        ///  <returns>类似"01 02 0F" </returns>  
        public static string ByteToString(this byte[] inBytes, string insertString = "")
        {
            string stringOut = "";
            if (inBytes == null)
            {
                return "";
            }

            try
            {
                foreach (byte inByte in inBytes)
                {
                    stringOut = stringOut + $"{inByte:X2}" + insertString;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return stringOut.Trim();
        }

        /// <summary>
        /// 将指定字节数组中一个字节序列转为16进制的字符串
        /// </summary>
        /// <param name="inBytes"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string ByteToString(this byte[] inBytes, int index, int count)
        {
            if (inBytes == null)
            {
                throw new ArgumentException(@"不能将空数组转换为16进制字符串", nameof(inBytes));
            }

            string stringOut = "";
            try
            {
                for (int i = index; i < count; i++)
                {
                    stringOut += inBytes[i].ToString("X2") + " ";
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return stringOut.Trim();
        }
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
    }
}