using CommonServiceLocator;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using 三相智慧能源网关调试软件.ViewModel;

namespace 三相智慧能源网关调试软件.View
{
    /// <summary>
    /// UserLoginPage.xaml 的交互逻辑
    /// </summary>
    public partial class UserLoginPage : Page
    {
        public UserLoginPage()
        {
            InitializeComponent();
        }

        private void UserLoginPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            var userLoginViewModel = ServiceLocator.Current.GetInstance<UserLoginViewModel>();
            userLoginViewModel.ReadUserInfoFromResource();
        }

        private void UserLoginPage_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var userLoginViewModel = ServiceLocator.Current.GetInstance<UserLoginViewModel>();
                userLoginViewModel.LoginCommand.Execute(null);
            }
        }
    }
}