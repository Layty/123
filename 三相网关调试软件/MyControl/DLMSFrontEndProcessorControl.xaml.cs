using System.Net.Sockets;
using System.Windows.Controls;
using System.Windows.Input;
using CommonServiceLocator;
using 三相智慧能源网关调试软件.DLMS;
using 三相智慧能源网关调试软件.ViewModel;

namespace 三相智慧能源网关调试软件.MyControl
{
    /// <summary>
    /// DLMSFrontEndProcessorControl.xaml 的交互逻辑
    /// </summary>
    public partial class DLMSFrontEndProcessorControl : UserControl
    {
        public DLMSFrontEndProcessorControl()
        {
            InitializeComponent();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ServiceLocator.Current.GetInstance<DLMSClient>().CurrentSocket = (Socket) ListBox.SelectedItem;
            ServiceLocator.Current.GetInstance<TcpServerViewModel>().CurrentSocketClient =
                ServiceLocator.Current.GetInstance<DLMSClient>().CurrentSocket;
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            ServiceLocator.Current.GetInstance<DLMSClient>().CurrentSocket = (Socket) ListBox.SelectedItem;
            ServiceLocator.Current.GetInstance<TcpServerViewModel>().CurrentSocketClient =
                ServiceLocator.Current.GetInstance<DLMSClient>().CurrentSocket;
        }
    }
}