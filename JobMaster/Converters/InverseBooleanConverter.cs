using System;
using System.Globalization;

namespace JobMaster.Converters
{
    /// <summary>
    /// 真亦假时假亦真
    /// </summary>
    public class InverseBooleanConverter : BaseConverter<InverseBooleanConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

    }
}
