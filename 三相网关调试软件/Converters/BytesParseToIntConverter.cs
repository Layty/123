using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects.DataStorage;

namespace 三相智慧能源网关调试软件.Converters
{
    public class UnitConverter : BaseConverter<UnitConverter> 
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Unit unit)
            {
                var ScalarUnit =new ScalarUnit(){Unit = unit};
                var Unitestring = ScalarUnit.GetUnitName();
                return Unitestring;
            }
            else
            {
                return "";
            }
        }
    }
    public class SocketToEndPointConverter : BaseConverter<SocketToEndPointConverter> {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Socket) value)?.RemoteEndPoint;
        }
    }
    public class BytesParseToIntConverter : BaseConverter<BytesParseToIntConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = (byte[]) value;
            if (data.Length==1)
            {
                return (byte) data[0];
            }

            if (data.Length==2)
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