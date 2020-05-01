using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;


namespace 三相智慧能源网关调试软件.Converters
{
    public class FactoryConverter : IValueConverter
    {
        public  object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var t=   int.Parse(value.ToString());
                if (t >= 8192)
                {
                    return Color.Red;
                }
            }
           
            return Color.Blue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}