using System.Text;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;

namespace 三相智慧能源网关调试软件.View.ServerCenter
{
    /// <summary>
    /// TelnetPage.xaml 的交互逻辑
    /// </summary>
    public partial class TelnetPage : Page
    {
        public TelnetPage()
        {
            InitializeComponent();
            Messenger.Default.Register<byte[]>(this, "ReceiveDataEvent", ShowReceiveData);
            Messenger.Default.Register<string>(this, "Status",ShowStatus);
        }

        private void ShowStatus(string obj)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                TextBoxShow.Text = obj;
            });
          
        }

        private void ShowReceiveData(byte[] obj)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                TextBoxShow.Text = Encoding.Default.GetString(obj);
            });
           
        }

        public TcpClientHelper TcpClientHelper =
            CommonServiceLocator.ServiceLocator.Current.GetInstance<TcpClientHelper>();

        private void ButtonConnect_OnClick(object sender, RoutedEventArgs e)
        {
            if (TcpClientHelper.ClientSocket.Connected)
            {
                TcpClientHelper.Disconnect();
            }
            else
            {
                TcpClientHelper.ConnectToServer();
            }
        }
    }
}