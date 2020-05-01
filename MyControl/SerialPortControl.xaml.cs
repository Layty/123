using System;
using System.Windows;
using System.Windows.Controls;
using CommonServiceLocator;
using MaterialDesignThemes.Wpf;
using 三相智慧能源网关调试软件.ViewModel;

namespace 三相智慧能源网关调试软件.MyControl
{
    /// <summary>
    /// SerialPortControl.xaml 的交互逻辑
    /// </summary>
    public partial class SerialPortControl : UserControl
    {
        public SerialPortControl()
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
