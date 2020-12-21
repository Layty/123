using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DataNotification.Commom;
using DataNotification.Model;
using Microsoft.Toolkit.Mvvm.Messaging;

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
                    HeartBeatFrame heartBeatFrame = new HeartBeatFrame();
                    var str = message.Item2.ByteToString();
                    if (heartBeatFrame.PduStringInHexConstructor(ref str))
                    {
                        var strAdd = Encoding.Default.GetString(heartBeatFrame.MeterAddressBytes);
                        if (ListBox.Items.Count == 0)
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                ListBox.Items.Add(message.Item1.RemoteEndPoint + "  <==>  " +
                                                  Encoding.Default.GetString(heartBeatFrame.MeterAddressBytes));
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
                                                      Encoding.Default.GetString(heartBeatFrame.MeterAddressBytes));
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