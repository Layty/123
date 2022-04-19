using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace 三相智慧能源网关调试软件.Views
{
    /// <summary>
    /// MessageBoxWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MessageBoxWindow : Window
    {
        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(MessageBoxWindow));


        public MessageBoxWindow()
        {
            InitializeComponent();
            DataContext = this;
        }



        private void MessageBoxWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var loadAnimation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.Parse("0:0:1")),
                From = 0.1,
                To = 1,
                EasingFunction = new ElasticEase()
                {
                    EasingMode = EasingMode.EaseOut,
                    Springiness = 8,
                    Oscillations = 3
                }
            };
            var loadclock = loadAnimation.CreateClock();
            Scale.ApplyAnimationClock(ScaleTransform.ScaleXProperty, loadclock);
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            var unloadAnimation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.Parse("0:0:0.5")),
                To = 0
            };
            var loadclock = unloadAnimation.CreateClock();
            loadclock.Completed += (a, b) =>
            {
                DialogResult = true;
            };

            Scale.ApplyAnimationClock(ScaleTransform.ScaleXProperty, loadclock);

        }

        private void MessageBoxWindow_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (e.MouseDevice.Target is Control)
                {
                    return;
                }
                DragMove();
            }
        }


        private void ButtonClose_OnClick(object sender, RoutedEventArgs e)
        {
            var unloadAnimation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.Parse("0:0:0.5")),
                To = 0
            };
            var loadclock = unloadAnimation.CreateClock();
            loadclock.Completed += (a, b) =>
            {
                DialogResult = false;
            };

            Scale.ApplyAnimationClock(ScaleTransform.ScaleXProperty, loadclock);
        }
    }
}