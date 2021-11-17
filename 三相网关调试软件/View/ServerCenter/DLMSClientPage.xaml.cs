using CommonServiceLocator;
using MyDlmsStandard;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using 三相智慧能源网关调试软件.ViewModel;
using 三相智慧能源网关调试软件.ViewModel.DlmsViewModels;

namespace 三相智慧能源网关调试软件.View.ServerCenter
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
            Client = ServiceLocator.Current.GetInstance<DlmsClient>();
        }

        private void ListBox_OnSelected(object sender, RoutedEventArgs e)
        {
            ServiceLocator.Current.GetInstance<DlmsClient>().CurrentSocket =
                (ListBox.SelectedItem as TcpServerViewModel.MeterIdMatchSocket)?.MySocket;
            ServiceLocator.Current.GetInstance<TcpServerViewModel>().CurrentSocketClient =
                ServiceLocator.Current.GetInstance<DlmsClient>().CurrentSocket;
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //            ServiceLocator.Current.GetInstance<DlmsClient>().CurrentSocket = ListBox.SelectedItem as Socket;
            //            ServiceLocator.Current.GetInstance<TcpServerViewModel>().CurrentSocketClient =
            //                ServiceLocator.Current.GetInstance<DlmsClient>().CurrentSocket;
            ServiceLocator.Current.GetInstance<DlmsClient>().CurrentSocket =
                (ListBox.SelectedItem as TcpServerViewModel.MeterIdMatchSocket)?.MySocket;
            ServiceLocator.Current.GetInstance<TcpServerViewModel>().CurrentSocketClient =
                ServiceLocator.Current.GetInstance<DlmsClient>().CurrentSocket;
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            //            ServiceLocator.Current.GetInstance<DlmsClient>().CurrentSocket = (Socket)ListBox.SelectedItem;
            //            ServiceLocator.Current.GetInstance<TcpServerViewModel>().CurrentSocketClient =
            //                ServiceLocator.Current.GetInstance<DlmsClient>().CurrentSocket;
            ServiceLocator.Current.GetInstance<DlmsClient>().CurrentSocket =
                (ListBox.SelectedItem as TcpServerViewModel.MeterIdMatchSocket)?.MySocket;
            ServiceLocator.Current.GetInstance<TcpServerViewModel>().CurrentSocketClient =
                ServiceLocator.Current.GetInstance<DlmsClient>().CurrentSocket;
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
            ServiceLocator.Current.GetInstance<DlmsClient>().CurrentSocket =
                (ListBox.SelectedItem as TcpServerViewModel.MeterIdMatchSocket)?.MySocket;
            ServiceLocator.Current.GetInstance<TcpServerViewModel>().CurrentSocketClient =
                ServiceLocator.Current.GetInstance<DlmsClient>().CurrentSocket;
        }
    }
}