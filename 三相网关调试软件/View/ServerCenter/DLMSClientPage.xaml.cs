using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommonServiceLocator;
using 三相智慧能源网关调试软件.DLMS;
using 三相智慧能源网关调试软件.ViewModel;
using 三相智慧能源网关调试软件.ViewModel.DlmsViewModels;

namespace 三相智慧能源网关调试软件.View.ServerCenter
{
    /// <summary>
    /// DLMSClientPage.xaml 的交互逻辑
    /// </summary>
    public partial class DLMSClientPage : Page
    {
        private DLMSClient Client { get; set; }

        public DLMSClientPage()
        {
            InitializeComponent();
            Client = ServiceLocator.Current.GetInstance<DLMSClient>();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ServiceLocator.Current.GetInstance<DLMSClient>().CurrentSocket = ListBox.SelectedItem as Socket;
            ServiceLocator.Current.GetInstance<TcpServerViewModel>().CurrentSocketClient =
                ServiceLocator.Current.GetInstance<DLMSClient>().CurrentSocket;
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            ServiceLocator.Current.GetInstance<DLMSClient>().CurrentSocket = (Socket)ListBox.SelectedItem;
            ServiceLocator.Current.GetInstance<TcpServerViewModel>().CurrentSocketClient =
                ServiceLocator.Current.GetInstance<DLMSClient>().CurrentSocket;
        }


        private void ToggleButtonSelectChannel_OnClick(object sender, RoutedEventArgs e)
        {
            if (ToggleButtonSelectChannel.IsChecked == true)
            {
                ToggleButtonSelectChannel.Content = "Wrapper";
                Client.DlmsSettingsViewModel.CommunicationType = CommunicationType.FrontEndProcess;
                Client.DlmsSettingsViewModel.InterfaceType = InterfaceType.WRAPPER;
            }
            else
            {
                ToggleButtonSelectChannel.Content = "SerialPort";
                Client.DlmsSettingsViewModel.CommunicationType = CommunicationType.SerialPort;
                Client.DlmsSettingsViewModel.InterfaceType = InterfaceType.HDLC;
            }
        }
    }
}