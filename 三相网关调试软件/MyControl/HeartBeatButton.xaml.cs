using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
