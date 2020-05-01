using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using 三相智慧能源网关调试软件.View;
using GalaSoft.MvvmLight.Threading;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.View.Management;

namespace 三相智慧能源网关调试软件
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public DispatcherTimer Timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            Timer.Interval = new TimeSpan(500);
            Timer.Tick += Timer_Tick;
            Timer.Start();

            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, (send, e) =>
            {
                var result = MessageBox.Show("是否退出程序", "提示", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    Close();
                }
            }));
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand,
                (send, e) => { WindowState = WindowState.Minimized; }));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, (send, e) =>
            {
                WindowState = WindowState.Maximized;
                ButtonRestore.Visibility = Visibility.Visible;
                ButtonMaximize.Visibility = Visibility.Collapsed;
            }));
            CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand,
                (send, e) =>
                {
                    WindowState = WindowState.Normal;
                    ButtonRestore.Visibility = Visibility.Collapsed;
                    ButtonMaximize.Visibility = Visibility.Visible;
                }));
            //避免最大化时覆盖任务栏
            this.MaxWidth = SystemParameters.WorkArea.Width;
            this.MaxHeight = SystemParameters.WorkArea.Height;

            //Messenger.Default.Register<byte[]>(this, "SendFileToBaseMeter", ShowBaseMeter);
            //Messenger.Default.Register<byte[]>(this, "ReceiveMsgFormBaseMeter", ShowReceiveMsg);
            Messenger.Default.Register<string>(this, "PlaySendFlashing", PlaySendFlashing);
            Messenger.Default.Register<string>(this, "PlayReceiveFlashing", PlayReceiveFlashing);
        }

        private void PlayReceiveFlashing(string obj)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                var ani = new ColorAnimation();
                ani.Duration = new TimeSpan(1000);
                ani.From = (Color) ColorConverter.ConvertFromString("#4EEE94");
                ani.To = (Color) ColorConverter.ConvertFromString("#EE3B3B");
                BlkReceive.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ani);
            });
        }

        private void PlaySendFlashing(string obj)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                var ani = new ColorAnimation();
                ani.Duration = new TimeSpan(1000);
                ani.From = (Color) ColorConverter.ConvertFromString("#4EEE94");
                ani.To = (Color) ColorConverter.ConvertFromString("#EE3B3B");
                BlkSend.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ani);
            });
        }

        private void ShowReceiveMsg(byte[] obj)
        {
            DispatcherHelper.CheckBeginInvokeOnUI((() =>
            {
                TextBlockMyLog.Text += $"{DateTime.Now} {(obj.ByteToString())}\r\n";
            }));
        }

        private void ShowBaseMeter(byte[] obj)
        {
            TextBlockMyLog.Text += $"{DateTime.Now} 发送 {obj.ByteToString()}\r\n";
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TextBlockTime.Text = DateTime.Now.ToString("yy-MM-dd ddd HH:mm:ss");
        }

        private void ExpandMenu_OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ExpandMenu.IsEnabled)
            {
                ExpandMenu.IsChecked = true;
                Frame.Navigate(GateWayLoginPage);
            }
        }

        public UserLoginPage UserLoginPage = new UserLoginPage();
        public GateWayLoginPage GateWayLoginPage = new GateWayLoginPage();

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(UserLoginPage);
        }


        private void ColorZone_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            // 按下拖动
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }

            // 双击放大
            if (e.ClickCount == 2 && e.ChangedButton == MouseButton.Left)
            {
                this.WindowState = this.WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;

                if (WindowState == WindowState.Maximized)
                {
                    ButtonRestore.Visibility = Visibility.Visible;
                    ButtonMaximize.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ButtonRestore.Visibility = Visibility.Collapsed;
                    ButtonMaximize.Visibility = Visibility.Visible;
                }
            }
        }

        private void RaiseThumb_OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                Width += e.HorizontalChange;
                Height += e.VerticalChange;
            }
        }

        private void ButtonGateWayLogin_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(GateWayLoginPage);
        }

        private void ButtonUserLogin_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(UserLoginPage);
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (CheckBoxScrollToEnd.IsChecked == true)
            {
                TextBoxReceive.ScrollToEnd();
            }
        }


        private void CheckBoxLogShowUp_OnClick(object sender, RoutedEventArgs e)
        {
            if (CheckBoxLogShowUp.IsChecked == true)
            {
                GridLog.Width = 250;
            }
            else
            {
                GridLog.Width = 0;
            }
        }
    }
}