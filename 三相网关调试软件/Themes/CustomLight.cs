using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace 三相智慧能源网关调试软件.Themes
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:三相智慧能源网关调试软件"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:三相智慧能源网关调试软件;assembly=三相智慧能源网关调试软件"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:CustomLight/>
    ///
    /// </summary>
    public class CustomLight : Control
    {
        static CustomLight()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomLight),
                new FrameworkPropertyMetadata(typeof(CustomLight)));
        }


        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(CustomLight), new PropertyMetadata("dimi"));



        public CustomLight()
        {
            this.Loaded += CustomLight_Loaded;
        }

        private ColorAnimation ColorAnimation;
        private void CustomLight_Loaded(object sender, RoutedEventArgs e)
        {
            ColorAnimation = new ColorAnimation
            { Duration = new TimeSpan(2000), From = FromColor, To = ToColor };
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
        public bool IsStart
        {
            get { return (bool)GetValue(IsStartProperty); }
            set { SetValue(IsStartProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsStart.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsStartProperty =
            DependencyProperty.Register("IsStart", typeof(bool), typeof(CustomLight), new PropertyMetadata(true, OnIsStartPropertyChangedCallBack));

        private static void OnIsStartPropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is CustomLight customLight))
            {
                return;
            }
            Storyboard storyboard = new Storyboard();

        }
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(CustomLight), new PropertyMetadata("dimi"));


        public Color FromColor
        {
            get { return (Color)GetValue(FromColorProperty); }
            set { SetValue(FromColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FromColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FromColorProperty =
            DependencyProperty.Register("FromColor", typeof(Color), typeof(CustomLight),
                new PropertyMetadata(Colors.Red));


        public Color ToColor
        {
            get { return (Color)GetValue(ToColorProperty); }
            set { SetValue(ToColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToColorProperty =
            DependencyProperty.Register("ToColor", typeof(Color), typeof(CustomLight),
                new PropertyMetadata(Colors.White));
    }
}