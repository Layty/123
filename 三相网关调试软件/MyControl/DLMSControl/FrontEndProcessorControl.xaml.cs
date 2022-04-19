using Microsoft.Extensions.DependencyInjection;
using System.Net.Sockets;
using System.Windows;

using System.Windows.Controls;
using System.Windows.Input;
using 三相智慧能源网关调试软件.ViewModels;
using 三相智慧能源网关调试软件.ViewModels.DlmsViewModels;

namespace 三相智慧能源网关调试软件.MyControl.DLMSControl
{
    /// <summary>
    /// FrontEndProcessorControl.xaml 的交互逻辑
    /// </summary>
    public partial class FrontEndProcessorControl : UserControl
    {
        public FrontEndProcessorControl()
        {
            InitializeComponent();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App.Current.Services.GetService<DlmsClient>().CurrentSocket = (Socket)ListBox.SelectedItem;
            App.Current.Services.GetService<TcpServerViewModel>().CurrentSocketClient =
                App.Current.Services.GetService<DlmsClient>().CurrentSocket;
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            App.Current.Services.GetService<DlmsClient>().CurrentSocket = (Socket)ListBox.SelectedItem;
            App.Current.Services.GetService<TcpServerViewModel>().CurrentSocketClient =
                App.Current.Services.GetService<DlmsClient>().CurrentSocket;
        }

        private void ButtonConfig_OnClick(object sender, RoutedEventArgs e)
        {
            // new SetNetwork().Show();
            System.Diagnostics.Process.Start(@"ncpa.cpl");
        }
    }
}