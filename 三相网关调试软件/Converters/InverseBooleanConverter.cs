using System;
using System.Globalization;

namespace 三相智慧能源网关调试软件.Converters
{
    /// <summary>
    /// 真亦假时假亦真
    /// </summary>
    public class InverseBooleanConverter:BaseConverter<InverseBooleanConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

    }
}
