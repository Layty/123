using System;
using System.Text;
using System.Windows;

namespace DataNotification.Commom
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
        /// 字符串 转16进制字符串  
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
                MessageBox.Show(e.Message);
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
                MessageBox.Show(@"不能将空数组转换为16进制字符串");
                return "";
            }

            string stringOut = "";
            try
            {
                for (int i = index; i < count; i++)
                {
                    stringOut += inBytes[i].ToString("X2") + " ";
                }

                //                foreach (byte inByte in inBytes)
                //                {
                //                    stringOut = stringOut + $"{inByte:X2}" + " ";
                //                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return stringOut.Trim();
        }

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