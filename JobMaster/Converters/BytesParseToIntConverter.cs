using System;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;

namespace JobMaster.Converters
{
    public class SocketToEndPointConverter : BaseConverter<SocketToEndPointConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Socket)value)?.RemoteEndPoint.ToString();
        }
    }

    public class BytesParseToIntConverter : BaseConverter<BytesParseToIntConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = (byte[])value;
            if (data.Length == 1)
            {
                return data[0];
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