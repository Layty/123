using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using MyDlmsStandard;
using 三相智慧能源网关调试软件.ViewModels;
using 三相智慧能源网关调试软件.ViewModels.DlmsViewModels;

namespace 三相智慧能源网关调试软件.Views.ServerCenter
{
    /// <summary>
    /// DLMSClientPage.xaml 的交互逻辑
    /// </summary>
    public partial class DlmsClientPage : Page
    {
        private DlmsClient Client { get; set; }

        public DlmsClientPage()
        {
            InitializeComponent();
            Client = App.Current.Services.GetService<DlmsClient>();
        }

        private void ListBox_OnSelected(object sender, RoutedEventArgs e)
        {
            App.Current.Services.GetService<DlmsClient>().CurrentSocket =
                (ListBox.SelectedItem as TcpServerViewModel.MeterIdMatchSocket)?.MySocket;
            App.Current.Services.GetService<TcpServerViewModel>().CurrentSocketClient =
                App.Current.Services.GetService<DlmsClient>().CurrentSocket;
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //            App.Current.Services.GetService<DlmsClient>().CurrentSocket = ListBox.SelectedItem as Socket;
            //            App.Current.Services.GetService<TcpServerViewModel>().CurrentSocketClient =
            //                App.Current.Services.GetService<DlmsClient>().CurrentSocket;
            App.Current.Services.GetService<DlmsClient>().CurrentSocket =
                (ListBox.SelectedItem as TcpServerViewModel.MeterIdMatchSocket)?.MySocket;
            App.Current.Services.GetService<TcpServerViewModel>().CurrentSocketClient =
                App.Current.Services.GetService<DlmsClient>().CurrentSocket;
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            //            App.Current.Services.GetService<DlmsClient>().CurrentSocket = (Socket)ListBox.SelectedItem;
            //            App.Current.Services.GetService<TcpServerViewModel>().CurrentSocketClient =
            //                App.Current.Services.GetService<DlmsClient>().CurrentSocket;
            App.Current.Services.GetService<DlmsClient>().CurrentSocket =
                (ListBox.SelectedItem as TcpServerViewModel.MeterIdMatchSocket)?.MySocket;
            App.Current.Services.GetService<TcpServerViewModel>().CurrentSocketClient =
                App.Current.Services.GetService<DlmsClient>().CurrentSocket;
        }


        private void ToggleButtonSelectChannel_OnClick(object sender, RoutedEventArgs e)
        {
            if (ToggleButtonSelectChannel.IsChecked == true)
            {
                ToggleButtonSelectChannel.Content = "Wrapper";
                Client.DlmsSettingsViewModel.PhysicalChanelType = PhysicalChanelType.FrontEndProcess;
                Client.DlmsSettingsViewModel.ProtocolInterfaceType = ProtocolInterfaceType.WRAPPER;
            }
            else
            {
                ToggleButtonSelectChannel.Content = "SerialPort";
                Client.DlmsSettingsViewModel.PhysicalChanelType = PhysicalChanelType.SerialPort;
                Client.DlmsSettingsViewModel.ProtocolInterfaceType = ProtocolInterfaceType.HDLC;
            }
        }


        private void ListBox_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            App.Current.Services.GetService<DlmsClient>().CurrentSocket =
                (ListBox.SelectedItem as TcpServerViewModel.MeterIdMatchSocket)?.MySocket;
            App.Current.Services.GetService<TcpServerViewModel>().CurrentSocketClient =
                App.Current.Services.GetService<DlmsClient>().CurrentSocket;
        }
    }
}