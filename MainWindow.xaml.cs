using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Messaging;
using 三相智慧能源网关调试软件.View;
using GalaSoft.MvvmLight.Threading;
using 三相智慧能源网关调试软件.View.Management;
using 三相智慧能源网关调试软件.ViewModel;

namespace 三相智慧能源网关调试软件
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public DispatcherTimer Timer = new DispatcherTimer();

        public readonly ColorAnimation ColorAnimation = new ColorAnimation
            {Duration = new TimeSpan(2000), From = Colors.Red, To = Colors.White};

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
            MaxWidth = SystemParameters.WorkArea.Width;
            MaxHeight = SystemParameters.WorkArea.Height;
            Messenger.Default.Register<string>(this, "PlaySendFlashing", PlaySendFlashing);
            Messenger.Default.Register<string>(this, "PlayReceiveFlashing", PlayReceiveFlashing);
            Messenger.Default.Register<byte[]>(this, "SendDataEvent", PlayNetSendFlashing);
            Messenger.Default.Register<byte[]>(this, "ReceiveDataEvent", PlayNetReceiveFlashing);
        }

        private void SerialPortMasterModelSerialDataSend(MySerialPortMaster.SerialPortMaster source, MySerialPortMaster.SerialPortEventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                BlkSend.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnimation);
            });
        }

        private void SerialPortMasterModelSerialDataReceived(MySerialPortMaster.SerialPortMaster source, MySerialPortMaster.SerialPortEventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                BlkReceive.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnimation);
            });
        }

        private void PlaySendFlashing(string obj)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                BlkSend.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnimation);
            });
        }

        private void PlayReceiveFlashing(string obj)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                BlkReceive.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnimation);
            });
        }


        private void PlayNetSendFlashing(byte[] obj)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                BlkNetSend.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnimation);
            });
        }

        private void PlayNetReceiveFlashing(byte[] obj)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                BlkNetReceive.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnimation);
            });
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
                Frame.Navigate(new GateWayLoginPage());
            }
        }

      

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(new UserLoginPage());
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
            Frame.Navigate(new GateWayLoginPage());
        }

        private void ButtonUserLogin_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(new UserLoginPage());
        }

     


        private void CheckBoxLogShowUp_OnClick(object sender, RoutedEventArgs e)
        {
            //if (CheckBoxLogShowUp.IsChecked == true)
            //{
            //    CardLog.Visibility = Visibility.Visible;
            //    CardLog.Width = 250;
            //}
            //else
            //{
            //    CardLog.Visibility = Visibility.Collapsed;
            //    CardLog.Width = 0;
            //}

            GridLog.Width = CheckBoxLogShowUp.IsChecked == true ? 250 : 0;
        }
    }
}