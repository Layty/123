using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace 三相智慧能源网关调试软件.MyControl
{
    /// <summary>
    /// HeartBeatButton.xaml 的交互逻辑
    /// </summary>
    public partial class HeartBeatButton : Button
    {
        public HeartBeatButton()
        {
            InitializeComponent();

            Loaded += HeartBeat_Loaded;
        }

        private void HeartBeat_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation()
            {
                From = 0.5,
                To = 0.1,
                RepeatBehavior = RepeatBehavior.Forever,
                Duration = new Duration(TimeSpan.FromSeconds(1))
            };
            var text = Template.FindName("TextBlock", this) as ContentPresenter;
            text.BeginAnimation(OpacityProperty, animation);
        }
    }
}
