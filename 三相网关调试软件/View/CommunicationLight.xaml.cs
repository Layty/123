﻿using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Net.Sockets;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using 三相智慧能源网关调试软件.Helpers;

namespace 三相智慧能源网关调试软件.View
{
    /// <summary>
    /// CommunicationLight.xaml 的交互逻辑
    /// </summary>
    public partial class CommunicationLight : UserControl
    {
        public readonly ColorAnimation ColorAnimation = new ColorAnimation
        { Duration = new TimeSpan(2000), From = Colors.Red, To = Colors.White };
        public CommunicationLight()
        {
            InitializeComponent();
            StrongReferenceMessenger.Default.Register<string, string>(this, "PlaySendFlashing",
               (sender, arg) =>
               {
                   DispatcherHelper.CheckBeginInvokeOnUI(() =>
                   {
                       BlkSend.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnimation);
                   });
               });
            StrongReferenceMessenger.Default.Register<string, string>(this, "PlayReceiveFlashing",
                (sender, arg) =>
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        BlkReceive.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnimation);
                    });
                });
            StrongReferenceMessenger.Default.Register<Tuple<Socket, byte[]>, string>(this, "ServerSendDataEvent",
                ((recipient, message) =>
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        BlkNetSend.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnimation);
                    });
                }));

            StrongReferenceMessenger.Default.Register<Tuple<Socket, byte[]>, string>(this, "ServerReceiveDataEvent",
                ((recipient, message) =>
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        BlkNetReceive.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnimation);
                    });
                }));

            StrongReferenceMessenger.Default.Register<Tuple<Socket, byte[]>, string>(this, "ClientSendDataEvent",
                ((recipient, message) =>
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        BlkNetSend.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnimation);
                    });
                }));

            StrongReferenceMessenger.Default.Register<Tuple<Socket, byte[]>, string>(this, "ClientReceiveDataEvent",
                ((recipient, message) =>
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        BlkNetReceive.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, ColorAnimation);
                    });
                }));
        }
    }
}
