using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace 三相智慧能源网关调试软件.Converters
{
    public class StringBuilderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ss = value as StringBuilder;
         
            return ss.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}