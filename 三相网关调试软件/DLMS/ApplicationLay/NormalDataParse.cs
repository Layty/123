using System;
using System.Linq;
using System.Text;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects;
using 三相智慧能源网关调试软件.DLMS.OBIS;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay
{
    public static class NormalDataParse
    {
        public static string ParsePduData(byte[] pduBytes)
        {
            string result = "";
            if (pduBytes[0] != (byte) Command.GetResponse)
            {
                return null;
            }

            switch (pduBytes[1])
            {
                case (byte) GetResponseType.Normal:
                    var InvokeIdAndPriority = pduBytes[2];
                    var datatype = pduBytes.Skip(3).Take(2).Reverse().ToArray();
                    var dt = BitConverter.ToInt16(datatype, 0);
                    switch (dt)
                    {
                        case (byte) DataType.UInt8:
                            result = pduBytes.Skip(5).Take(1).ToArray()[0].ToString();
                            break;
                        case (byte) DataType.UInt16:
                            result = BitConverter.ToUInt16(pduBytes.Skip(5).Take(2).Reverse().ToArray(), 0).ToString();
                            break;
                        case (byte) DataType.Int32:
                            result = BitConverter.ToInt32(pduBytes.Skip(5).Take(4).Reverse().ToArray(), 0).ToString();
                            break;
                        case (byte) DataType.UInt32:
                            result = BitConverter.ToUInt32(pduBytes.Skip(5).Take(4).Reverse().ToArray(), 0).ToString();
                            break;
                        case (byte) DataType.OctetString:
                            if ((pduBytes[5] & 0x80) == 0x80)
                            {
                                var index = pduBytes[5] - 0x80;
                                var range = BitConverter.ToInt16(pduBytes.Skip(6).Take(index).Reverse().ToArray(), 0);
                                var resultBytes = pduBytes.Skip(6 + index).Take(range).ToArray();
                                result = Encoding.Default.GetString(resultBytes);
                            }
                            else
                            {
                                result = pduBytes.Skip(6).Take(pduBytes[5]).ToArray().ByteToString();
                            }

                            break;
                        case (byte) DataType.Structure:
                            var rangeStruct = pduBytes.Skip(5).Take(1).ToArray()[0];
                            result = pduBytes.Skip(6).ToArray().ByteToString(); //返回结构体
                            break;
                        case (byte) DataType.Enum:
                            // var EnumValue = pduBytes.Skip(5).Take(1).ToArray()[0];
                            result = pduBytes.Skip(5).Take(1).ToArray()[0].ToString();
                            break;
                        case (byte) DataType.Array:
                            result = pduBytes.Skip(5).ToArray().ByteToString();
                            break;
                    }

                    break;
                case (byte) GetResponseType.WithDataBlock:
                    var InvokeIdAndPriority2 = pduBytes[2];
                    var lastblock = pduBytes[3];
                    var BlockNumber = pduBytes.Skip(4).Take(4).ToArray();
                    var rResult = pduBytes[9];
                    var rawData = pduBytes[10];
                    result = pduBytes.Skip(10).ToArray().ByteToString();
                    ;
                    break;
                case (byte) GetResponseType.WithList:
                    ;
                    break;
            }


            return result;
        }

        public static string ParsePduData(byte[] pduBytes, out DataType dataType, out ErrorCode dataBoolResult)
        {
            string result = "";
            dataType = DataType.NullData;
            dataBoolResult = ErrorCode.OtherReason;
            if (pduBytes[0] != (byte) Command.GetResponse)
            {
                return null;
            }

            switch (pduBytes[1])
            {
                case (byte) GetResponseType.Normal:
                    GetResponseNormal getResponseNormal = new GetResponseNormal();
                    var InvokeIdAndPriority = pduBytes[2];
                    if (pduBytes[3] == 0)
                    {
                        dataBoolResult = (ErrorCode) pduBytes[3];
                    }
                    else if (pduBytes[3] != 0)
                    {
                        dataBoolResult = (ErrorCode) pduBytes[4];
                        return "";
                    }

                    var datatype = pduBytes.Skip(3).Take(2).Reverse().ToArray();
                    var dt = BitConverter.ToInt16(datatype, 0);
                    switch (dt)
                    {
                        case (byte) DataType.UInt8:
                            dataType = DataType.UInt8;
                            result = pduBytes.Skip(5).Take(1).ToArray()[0].ToString();
                            break;
                        case (byte) DataType.UInt16:
                            dataType = DataType.UInt16;
                            result = BitConverter.ToUInt16(pduBytes.Skip(5).Take(2).Reverse().ToArray(), 0).ToString();
                            break;
                        case (byte) DataType.Int32:
                            dataType = DataType.Int32;
                            result = BitConverter.ToInt32(pduBytes.Skip(5).Take(4).Reverse().ToArray(), 0).ToString();
                            break;
                        case (byte) DataType.UInt32:
                            dataType = DataType.UInt32;
                            result = BitConverter.ToUInt32(pduBytes.Skip(5).Take(4).Reverse().ToArray(), 0).ToString();
                            break;
                        case (byte) DataType.OctetString:
                            dataType = DataType.OctetString;
                            if ((pduBytes[5] & 0x80) == 0x80)
                            {
                                var index = pduBytes[5] - 0x80;
                                var range = BitConverter.ToInt16(pduBytes.Skip(6).Take(index).Reverse().ToArray(), 0);
                                var resultBytes = pduBytes.Skip(6 + index).Take(range).ToArray();
                                result = Encoding.Default.GetString(resultBytes);
                            }
                            else
                            {
                                result = pduBytes.Skip(6).Take(pduBytes[5]).ToArray().ByteToString();
                            }

                            break;
                        case (byte) DataType.Structure:
                            dataType = DataType.Structure;
                            var rangeStruct = pduBytes.Skip(5).Take(1).ToArray()[0];
                            result = pduBytes.Skip(6).ToArray().ByteToString(); //返回结构体
                            break;
                        case (byte) DataType.Enum:
                            dataType = DataType.Enum;
                            // var EnumValue = pduBytes.Skip(5).Take(1).ToArray()[0];
                            result = pduBytes.Skip(5).Take(1).ToArray()[0].ToString();
                            break;
                        case (byte) DataType.Array:
                            dataType = DataType.Array;
                            result = pduBytes.Skip(5).ToArray().ByteToString();
                            break;
                    }

                    ;
                    break;
                case (byte) GetResponseType.WithDataBlock:
                    var InvokeIdAndPriority2 = pduBytes[2];
                    var lastblock = pduBytes[3];
                    var BlockNumber = pduBytes.Skip(4).Take(4).ToArray();
                    var rResult = pduBytes[9];
                    var rawData = pduBytes[10];
                    result = pduBytes.Skip(10).ToArray().ByteToString();
                    ;
                    break;
                case (byte) GetResponseType.WithList:
                    ;
                    break;
            }


            return result;
        }

        public static string HowToDisplayOctetString(byte[] dataBytes, DisplayFormatToShow displayFormat)
        {
            var displayString = "";
            switch (displayFormat)
            {
                case DisplayFormatToShow.ASCII:
                    return Encoding.Default.GetString(dataBytes);
                    break;
                case DisplayFormatToShow.Original:
                    return dataBytes.ByteToString();
                    break;
                case DisplayFormatToShow.DateTime:
                    var dlmsclock = new DLMSClock(dataBytes);
                    return dlmsclock.ToString();
                    break;
                case DisplayFormatToShow.OBIS:
                    if (dataBytes.Length == 6)
                    {
                        return ObisHelper.GetObisOriginal(dataBytes.ByteToString().Replace(" ", ""));
                    }

                    break;
            }

            return displayString;
        }
    }
}