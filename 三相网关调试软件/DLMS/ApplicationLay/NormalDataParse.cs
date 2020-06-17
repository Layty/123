using System;
using System.Linq;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay
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
                    var InvokeIdAndPriority = pudBytes[2];
                    var datatype = pudBytes.Skip(3).Take(2).Reverse().ToArray();
                    var dt = BitConverter.ToInt16(datatype, 0);
                    switch (dt)
                    {
                        case (byte)DataType.UInt16:
                            result = BitConverter.ToUInt16(pudBytes.Skip(5).Take(2).Reverse().ToArray(), 0).ToString();
                            break;
                        case (byte)DataType.Int32:
                            result = BitConverter.ToInt32(pudBytes.Skip(5).Take(4).Reverse().ToArray(), 0).ToString();
                            break;
                        case (byte)DataType.UInt32:
                            result = BitConverter.ToUInt32(pudBytes.Skip(5).Take(4).Reverse().ToArray(), 0).ToString();
                            break;
                        case (byte)DataType.OctetString:
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
                        case (byte)DataType.Structure:
                            var rangeStruct = pudBytes.Skip(5).Take(1).ToArray()[0];
                            result = pudBytes.Skip(6).ToArray().ByteToString(); //返回结构体
                            break;
                        case (byte)DataType.Enum:
                            // var EnumValue = pudBytes.Skip(5).Take(1).ToArray()[0];
                            result = pudBytes.Skip(5).Take(1).ToArray()[0].ToString();
                            break;
                        case (byte)DataType.Array:
                            result = pudBytes.Skip(5).ToArray().ByteToString();
                            break;
                    };
                    break;
                case (byte) GetResponseType.WithDataBlock:
                    var InvokeIdAndPriority2 = pudBytes[2];
                    var lastblock = pudBytes[3];
                    var BlockNumber = pudBytes.Skip(4).Take(4).ToArray();
                    var rResult = pudBytes[9];
                    var rawData = pudBytes[10];
                    result = pudBytes.Skip(10).ToArray().ByteToString();
                    ;
                    break;
                case (byte) GetResponseType.WithList:
                    ;
                    break;
            }

        

            return result;
        }

      
    }
}