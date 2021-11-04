using System;

namespace MySerialPortMaster
{
    internal static class Common
    {

        ///  <summary>  
        /// 将指定字节数组转为16进制的字符串  
        ///  </summary>  
        ///  <param name="inBytes"> 二进制字节 </param>  
        ///  <returns>类似"01 02 0F" </returns>  
        public static string ByteToString(this byte[] inBytes)
        {
            string stringOut = "";
            try
            {
                foreach (byte inByte in inBytes)
                {
                    stringOut = stringOut + $"{inByte:X2}" + " ";
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
                throw new Exception(@"不能将空数组转换为16进制字符串");
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
    }
}