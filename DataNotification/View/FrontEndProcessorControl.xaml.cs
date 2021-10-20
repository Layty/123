using System;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Toolkit.Mvvm.Messaging;
using MyDlmsStandard.Wrapper;

namespace DataNotification.View
{
    /// <summary>
    /// FrontEndProcessorControl.xaml 的交互逻辑
    /// </summary>
    public partial class FrontEndProcessorControl : UserControl
    {
        public int Count
        {
            get { return (int) GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for s.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CountProperty =
            DependencyProperty.Register("Count", typeof(int), typeof(FrontEndProcessorControl),
                new PropertyMetadata(0));


        public FrontEndProcessorControl()
        {
            InitializeComponent();

            StrongReferenceMessenger.Default.Register<Tuple<Socket, byte[]>, string>(this, "ServerReceiveDataEvent",
                (recipient, message) =>
                {
                    var heartBeatFrame = Wrapper47FrameFactory.CreateHeartBeatFrame(message.Item2);
                    if (heartBeatFrame != null)
                    {
                        var strAdd = heartBeatFrame.GetMeterAddressString();
                        if (ListBox.Items.Count == 0)
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                ListBox.Items.Add(message.Item1.RemoteEndPoint + "  <==>  " + strAdd);
                            });
                        }
                        else
                        {
                            var boo = false;
                            for (int i = 0; i < ListBox.Items.Count; i++)
                            {
                                if (ListBox.Items[i].ToString().Contains(strAdd))
                                {
                                    boo = false;
                                    break;
                                }
                                else
                                {
                                    boo = true;
                                }
                            }

                            if (boo)
                            {
                                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                {
                                    ListBox.Items.Add(message.Item1.RemoteEndPoint + "  <==>   " +
                                                      strAdd);
                                });
                            }
                        }
                    }
                });
        }

        private void ButtonConfig_OnClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"ncpa.cpl");
        }
    }
}