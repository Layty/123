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

                case OctetStringDisplayFormat.Time:
                    var hour = Convert.ToString(dataBytes[0]).PadLeft(2, '0');
                    var min = Convert.ToString(dataBytes[1]).PadLeft(2, '0');
                    var sen = Convert.ToString(dataBytes[2]).PadLeft(2, '0');
                    return hour + min + sen;
                    
            }

            return displayString;
        }
    }
}