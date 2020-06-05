using System;
using System.Linq;
using System.Text;
using System.Windows;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS
{
    public static class NormalDataParse
    {
        public static string ParsePduData(byte[] pudBytes)
        {
            string result = "";
            if (pudBytes[0] != (byte) Command.GetResponse)
            {
                return null;
            }

            switch (pudBytes[1])
            {
                case (byte) GetResponseType.Normal:
                    ;
                    break;
                case (byte) GetResponseType.WithDataBlock:
                    ;
                    break;
                case (byte) GetResponseType.WithList:
                    ;
                    break;
            }

            var InvokeIdAndPriority = pudBytes[2];
            var datatype = pudBytes.Skip(3).Take(2).Reverse().ToArray();
            var dt = BitConverter.ToInt16(datatype, 0);
            switch (dt)
            {
                case (byte) DataType.UInt16:
                    result = BitConverter.ToUInt16(pudBytes.Skip(5).Take(2).Reverse().ToArray(), 0).ToString();
                    break;
                case (byte) DataType.Int32:
                    result = BitConverter.ToInt32(pudBytes.Skip(5).Take(4).Reverse().ToArray(), 0).ToString();
                    break;
                case (byte) DataType.UInt32:
                    result = BitConverter.ToUInt32(pudBytes.Skip(5).Take(4).Reverse().ToArray(), 0).ToString();
                    break;
                case (byte) DataType.OctetString:
                    if ((pudBytes[5] & 0x80) == 0x80)
                    {
                        var index = pudBytes[5] - 0x80;
                        var range = BitConverter.ToInt16(pudBytes.Skip(6).Take(index).Reverse().ToArray(), 0);
                        result = pudBytes.Skip(6 + index).Take(range).ToArray().ByteToString();
                    }
                    else
                    {
                        result = pudBytes.Skip(6).Take(pudBytes[5]).ToArray().ByteToString();
                    }

                    break;
                case (byte) DataType.Structure:
                    var rangeStruct = pudBytes.Skip(5).Take(1).ToArray()[0];
                    result = pudBytes.Skip(6).ToArray().ByteToString(); //返回结构体
                    break;
            }

            return result;
        }

        public static byte[] GetDataContent(byte[] bytes, byte index, out bool result)
        {
            byte[] date = bytes;
            result = false;
            try
            {
                byte len = bytes[19 - index];
                date = bytes.Skip(20 - index).Take((int) (len)).ToArray<byte>();
                result = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return date;
        }

        public static byte[] GetDataFactoryContent(byte[] bytes, byte index, out bool result)
        {
            byte[] date = bytes;
            result = false;
            try
            {
                // byte len = bytes[19];
                // byte len = bytes[17];
                date = bytes.Skip(19 - index).Take((int) (2)).ToArray<byte>();
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
                var len = BitConverter.ToInt16(lens, 0);
                date = bytes.Skip(20 + lens.Length - index).Take((int) (len)).ToArray<byte>();
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