using System.Windows;
using System.Windows.Controls;

namespace 三相智慧能源网关调试软件.View
{
    public static class PasswordBoxHelper
    {
        public static string GetPassword(DependencyObject obj)
        {
            return (string)obj.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject obj, string value)
        {
            obj.SetValue(PasswordProperty, value);
        }

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached("Password", typeof(string), typeof(PasswordBoxHelper),
                new PropertyMetadata("", OnPasswordPropertyChanged));

        private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            string password = (string)e.NewValue;
            if (sender is PasswordBox box && box.Password != password)
            {
                box.Password = password;
            }
        }
    }
}