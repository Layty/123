using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace JobMaster.Views
{
    /// <summary>
    /// FrontEndProcessorControl.xaml 的交互逻辑
    /// </summary>
    public partial class FrontEndProcessorView : UserControl
    {
        public FrontEndProcessorView()
        {
            InitializeComponent();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ServiceLocator.Current.GetInstance<DlmsClient>().CurrentSocket = (Socket)ListBox.SelectedItem;
            //ServiceLocator.Current.GetInstance<TcpServerViewModel>().CurrentSocketClient =
            //    ServiceLocator.Current.GetInstance<DlmsClient>().CurrentSocket;
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            //ServiceLocator.Current.GetInstance<DlmsClient>().CurrentSocket = (Socket)ListBox.SelectedItem;
            //ServiceLocator.Current.GetInstance<TcpServerViewModel>().CurrentSocketClient =
            //    ServiceLocator.Current.GetInstance<DlmsClient>().CurrentSocket;
        }

        private void ButtonConfig_OnClick(object sender, RoutedEventArgs e)
        {
            // new SetNetwork().Show();
            //  System.Diagnostics.Process.Start(@"ncpa.cpl");
        }
    }
}