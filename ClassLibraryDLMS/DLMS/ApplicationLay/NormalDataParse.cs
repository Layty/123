using System;
using System.Linq;
using System.Text;
using ClassLibraryDLMS.Common;
using ClassLibraryDLMS.DLMS.ApplicationLay.ApplicationLayEnums;
using ClassLibraryDLMS.DLMS.ApplicationLay.CosemObjects;
using ClassLibraryDLMS.DLMS.OBIS;

namespace ClassLibraryDLMS.DLMS.ApplicationLay
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

       

        public static string HowToDisplayIntValue(byte[] dataBytes, UInt32ValueDisplayFormat octetStringDisplayFormat)
        {
            var displayString = "";
            switch (octetStringDisplayFormat)
            {
                case UInt32ValueDisplayFormat.Original:
                    return dataBytes.ByteToString();
                case UInt32ValueDisplayFormat.IpAddress:
                    string[] str=new string[4] ;
                    for (int i = 0; i < dataBytes.Length; i++)
                    {
                        str[i] = dataBytes[i].ToString();
                    }
                    return $"{str[0]}.{str[1]}.{str[2]}.{str[3]}";
                case UInt32ValueDisplayFormat.IntValue:
                    return BitConverter.ToUInt32(dataBytes.Reverse().ToArray(),0).ToString();
            }

            return displayString;
        }

        public static string HowToDisplayOctetString(byte[] dataBytes,
            OctetStringDisplayFormat octetStringDisplayFormat)
        {
            var displayString = "";
            dataBytes = dataBytes.Skip(1).ToArray();
            switch (octetStringDisplayFormat)
            {
                case OctetStringDisplayFormat.Ascii:
                    return Encoding.Default.GetString(dataBytes);
                case OctetStringDisplayFormat.Original:
                    return dataBytes.ByteToString();
                case OctetStringDisplayFormat.DateTime:
                    var dlmsclock = new CosemClock(dataBytes);
                    return dlmsclock.ToString();
                case OctetStringDisplayFormat.Obis:
                    if (dataBytes.Length == 6)
                    {
                        return ObisHelper.GetObisOriginal(dataBytes.ByteToString().Replace(" ", ""));
                    }

                    break;
                case OctetStringDisplayFormat.Date:
                    var year = BitConverter.ToUInt16(dataBytes.Take(2).Reverse().ToArray(), 0);
                    var month = Convert.ToString(dataBytes[2]).PadLeft(2, '0');
                    var day = Convert.ToString(dataBytes[3]).PadLeft(2, '0');
                    var week = Convert.ToString(dataBytes[4]).PadLeft(2, '0');
                    return year + month + day + week;

                case OctetStringDisplayFormat.Time: break;
            }

            return displayString;
        }
    }
}