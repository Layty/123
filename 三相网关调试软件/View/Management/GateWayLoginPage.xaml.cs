using System.Windows;
using System.Windows.Controls;

namespace 三相智慧能源网关调试软件.View.Management
{
    /// <summary>
    /// GateWayLoginPage.xaml 的交互逻辑
    /// </summary>
    public partial class GateWayLoginPage : Page
    {
        public GateWayLoginPage()
        {
            InitializeComponent();
        }

        public ENetClientHelper ENetInstance =
            CommonServiceLocator.ServiceLocator.Current.GetInstance<ENetClientHelper>();



        private void ButtonConnect_OnClick(object sender, RoutedEventArgs e)
        {
            if (ENetInstance.ConnectResult)
            {
                ENetInstance.DisconnectToServer();
            }
            else
            {
                ENetInstance.ConnectToServer();
            }
        }
    }
}