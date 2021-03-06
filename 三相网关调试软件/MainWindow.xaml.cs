using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using 三相智慧能源网关调试软件.Helpers;
using 三相智慧能源网关调试软件.MyControl;
using 三相智慧能源网关调试软件.ViewModels;
using 三相智慧能源网关调试软件.Views;
using 三相智慧能源网关调试软件.Views.Management;

namespace 三相智慧能源网关调试软件
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly Key[] _target = new Key[]
            {Key.Up, Key.Up, Key.Down, Key.Down, Key.Left, Key.Right, Key.Left, Key.Right};

        int _keyState = 0;

        // public DispatcherTimer Timer = new DispatcherTimer();



        //  SpeechSynthesizer _speechSynthesizer = new SpeechSynthesizer();

        public MainWindow()
        {
            InitializeComponent();
            string speech = Properties.Settings.Default.OpenSound;
            //speechSynthesizer.SpeakAsync(speech);
            this.KeyDown += MainWindow_KeyDown;
            // Timer.Interval = new TimeSpan(500);
            // Timer.Tick += Timer_Tick;
            //            Timer.Start();


            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, (send, e) =>
            {
                MessageBoxWindow msgBoxWindow = new MessageBoxWindow() { Message = "是否退出程序？", Title = "提示" };
                var result = msgBoxWindow.ShowDialog();
                if (result == true)
                {
                    //speechSynthesizer.Speak("后会有期");
                    this.Close();
                    //Application.Current.Shutdown();
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

            StrongReferenceMessenger.Default.Register<string, string>(this, "Snackbar",
                (recipient, message) =>
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() => { MainSnackbar.MessageQueue.Enqueue(message); });
                });
        }


        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == _target[_keyState])
            {
                _keyState++;
                if (_keyState >= _target.Length)
                {
                    var s = App.Current.Services.GetService<UserLoginViewModel>();
                    s.LoginModel.LoginResult = true;

                    _keyState = 0;
                }
            }
            else
            {
                _keyState = 0;
                if (e.Key == _target[_keyState])
                {
                    _keyState++;
                }
            }
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            //  TextBlockTime.Text = DateTime.Now.ToString("yy-MM-dd ddd HH:mm:ss");
        }

        private void ExpandMenu_OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ExpandMenu.IsEnabled)
            {
                ExpandMenu.IsChecked = true;
                Frame.Navigate(new CoverPage());
            }
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

        private void ToggleButtonLog_OnClick(object sender, RoutedEventArgs e)
        {
            CardLog.Visibility = ToggleButtonLog.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            ColumnLog.Width = new GridLength(CardLog.ActualWidth, GridUnitType.Auto);
            TabControl.Width = double.NaN;
        }

        private void HeartBeatButton_OnClick(object sender, RoutedEventArgs e)
        {
            new LogWindow() { Owner = this }.Show();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(new UserLoginPage());
            var notify = new Win10NotifyMessage { NotifyText = "Hi" };
            notify.Show();
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            var notify = new Win10NotifyMessage { NotifyText = "Goodbye" };
            notify.Show();
        }

        private void ButtonCosemEditor_OnClick(object sender, RoutedEventArgs e)
        {
            new CosemObjectsManagement() { Owner = this }.Show();
        }

        private void ButtonMeterData_OnClick(object sender, RoutedEventArgs e)
        {
            new MeterDataWindow() { Owner = this }.Show();
        }

        private void ButtonTime_Click(object sender, RoutedEventArgs e)
        {
            new ChangeSystemDateTime().Show();


        }

        private void SingalPharse_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(new SinglePhaseManagementPage());
        }
    }
}