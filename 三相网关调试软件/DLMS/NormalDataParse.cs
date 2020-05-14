using System;
using System.Linq;
using System.Windows;

namespace 三相智慧能源网关调试软件.DLMS
{
   public static class NormalDataParse
    {

        public static byte[] GetDataContent(byte[] bytes,byte index, out bool result)
        {
            byte[] date = bytes;
            result = false;
            try
            {
                byte len = bytes[19- index];
                date = bytes.Skip(20- index).Take((int)(len )).ToArray<byte>();
                result = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return date;
        }
        public static byte[] GetDataFactoryContent(byte[] bytes,byte index, out bool result)
        {
            byte[] date = bytes;
            result = false;
            try
            {
                // byte len = bytes[19];
                // byte len = bytes[17];
                date = bytes.Skip(19- index).Take((int)(2)).ToArray<byte>();
                result = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return date;
        }

        public static byte[] GetUtilityTablesDataContent(byte[] bytes, byte index, out bool result)
        {
            byte[] date = bytes;
            result = false;
            try
            {
              //  byte len = bytes[19 - index];
                var lens = bytes.Skip(20 - index).Take(2).Reverse().ToArray();
                var len = BitConverter.ToInt16(lens,0);
                date = bytes.Skip(20+lens.Length - index).Take((int)(len)).ToArray<byte>();
                result = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return date;
        }
    }
}
