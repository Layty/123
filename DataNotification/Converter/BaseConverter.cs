using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace DataNotification.Converter
{
    public abstract class BaseConverter<T> : MarkupExtension, IValueConverter where T : class, new()
    {
        private static T _instance;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new T());
        }

        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);


        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}