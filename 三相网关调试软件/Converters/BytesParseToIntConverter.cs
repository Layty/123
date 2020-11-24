using System;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using MyDlmsStandard.ApplicationLay.CosemObjects;
using 三相智慧能源网关调试软件.Common;

namespace 三相智慧能源网关调试软件.Converters
{
    public class HexStringToDateTimeConverter : BaseConverter<HexStringToDateTimeConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var clock = new CosemClock();
            string str = value.ToString();
            try
            {
                var b = clock.DlmsClockParse(str.StringToByte());
                if (b)
                {
                    return clock.ToString();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return value;
        }
    }

    public class SocketToEndPointConverter : BaseConverter<SocketToEndPointConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Socket) value)?.RemoteEndPoint.ToString();
        }
    }

    public class BytesParseToIntConverter : BaseConverter<BytesParseToIntConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = (byte[]) value;
            if (data.Length == 1)
            {
                return (byte) data[0];
            }

            if (data.Length == 2)
            {
                return BitConverter.ToInt16(data.Reverse().ToArray(), 0);
            }
            else
            {
                return BitConverter.ToInt32(data.Reverse().ToArray(), 0);
            }
        }
    }
}