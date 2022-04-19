using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace 三相智慧能源网关调试软件.Views.Management
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
            App.Current.Services.GetService<ENetClientHelper>();



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