using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Color = System.Drawing.Color;


namespace 三相智慧能源网关调试软件.Converters
{
    public class FactoryConverter : IValueConverter
    {
        public  object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var t=  int.TryParse(value.ToString(),out int result);
                if (t)
                {
                    if (result >= 8192)
                    {
                        return new SolidColorBrush(Colors.Red);
                    }
                }
                return new SolidColorBrush(Colors.Blue);

            }

            return new SolidColorBrush(Colors.Blue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}