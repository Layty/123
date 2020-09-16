﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommonServiceLocator;
using NPOI.OpenXmlFormats.Dml;
using 三相智慧能源网关调试软件.DLMS;
using 三相智慧能源网关调试软件.DLMS.HDLC.Enums;
using 三相智慧能源网关调试软件.ViewModel;

namespace 三相智慧能源网关调试软件.View.ServerCenter
{
    /// <summary>
    /// DLMSClientPage.xaml 的交互逻辑
    /// </summary>
    public partial class DLMSClientPage : Page
    {
        private DLMSClient client { get; set; }

        public DLMSClientPage()
        {
            InitializeComponent();
            client = ServiceLocator.Current.GetInstance<DLMSClient>();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ServiceLocator.Current.GetInstance<DLMSClient>().CurrentSocket = ListBox.SelectedItem as Socket;
            ServiceLocator.Current.GetInstance<TcpServerViewModel>().CurrentSocketClient =
                ServiceLocator.Current.GetInstance<DLMSClient>().CurrentSocket;
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            ServiceLocator.Current.GetInstance<DLMSClient>().CurrentSocket = (Socket) ListBox.SelectedItem;
            ServiceLocator.Current.GetInstance<TcpServerViewModel>().CurrentSocketClient =
                ServiceLocator.Current.GetInstance<DLMSClient>().CurrentSocket;
        }


        private void ToggleButtonSelectChannel_OnClick(object sender, RoutedEventArgs e)
        {
            if (ToggleButtonSelectChannel.IsChecked == true)
            {
                ToggleButtonSelectChannel.Content = "WRAPPER";
                client.MyDlmsSettings.CommunicationType = CommunicationType.FrontEndProcess;
                client.MyDlmsSettings.InterfaceType = InterfaceType.WRAPPER;
            }
            else
            {
                ToggleButtonSelectChannel.Content = "SerialPort";
                client.MyDlmsSettings.CommunicationType = CommunicationType.SerialPort;
                client.MyDlmsSettings.InterfaceType = InterfaceType.HDLC;
            }
        }
    }
}