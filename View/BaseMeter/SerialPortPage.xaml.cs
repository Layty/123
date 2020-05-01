using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommonServiceLocator;
using MaterialDesignThemes.Wpf;
using WPF_DLMS_DateTimeHelper.Views;
using 三相智慧能源网关调试软件.ViewModel;

namespace 三相智慧能源网关调试软件.View.BaseMeter
{
    /// <summary>
    /// SerialPortPage.xaml 的交互逻辑
    /// </summary>
    public partial class SerialPortPage : Page
    {
        public SerialPortPage()
        {
            InitializeComponent();
        }

        public SerialPortViewModel SerialPortViewModel = ServiceLocator.Current.GetInstance<SerialPortViewModel>();

        private async void BtnOpenSerialPort_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SerialPortViewModel.SerialPortModel.IsOpen)
                {
                    SerialPortViewModel.SerialPortModel.Close();
                }
                else
                {
                    SerialPortViewModel.SerialPortModel.Open();
                    SerialPortViewModel.SaveSerialPortConfigFileCommand.Execute(null);
                }
            }
            catch (Exception exception)
            {
                var view = new MyControl.MessageBox(exception.Message, exception.Source);
                 await DialogHost.Show(view, "RootDialog");
            }
        }
    }
}