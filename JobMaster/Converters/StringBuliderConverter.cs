using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace JobMaster.Converters
{
    public class StringBuilderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is StringBuilder ss) return ss.ToString();
            else
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}