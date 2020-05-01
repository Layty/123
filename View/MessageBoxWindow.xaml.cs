using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WPF_DLMS_DateTimeHelper.Views
{
    /// <summary>
    /// MessageBoxWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MessageBoxWindow : Window
    {
        public MessageBoxWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string Message { get; set; } = "Message";

        private void MessageBoxWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var loadAnimation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.Parse("0:0:1")),
                From = 0.1,
                To = 1,
                EasingFunction = new ElasticEase()
                {
                    EasingMode = EasingMode.EaseOut, Springiness = 8, Oscillations = 3
                }
            };
            var loadclock = loadAnimation.CreateClock();
            Scale.ApplyAnimationClock(ScaleTransform.ScaleXProperty, loadclock);
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            var unloadAnimation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.Parse("0:0:0.5")), From = 1, To = 0.1
            };
            var loadclock = unloadAnimation.CreateClock();
            loadclock.Completed += (a, b) => { DialogResult = true; };
            Scale.ApplyAnimationClock(ScaleTransform.ScaleXProperty, loadclock);
         
        }

        private void MessageBoxWindow_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton== MouseButtonState.Pressed)
            {
                if (e.MouseDevice.Target is Control)
                {
                    return;
                }
                DragMove();
            }
        }

      
    }
}