using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;


namespace JobMaster.Converters
{
    public class FactoryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var t = int.TryParse(value.ToString(), out int result);
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