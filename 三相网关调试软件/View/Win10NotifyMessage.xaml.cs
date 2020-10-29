using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace 三相智慧能源网关调试软件.View
{
    /// <summary>
    /// Win10NotifyMessage.xaml 的交互逻辑
    /// </summary>
    public partial class Win10NotifyMessage
    {
        public string NotifyText { get; set; }
        public Win10NotifyMessage()
        {
            InitializeComponent();
            Left = SystemParameters.WorkArea.Size.Width - Width;
            Top = SystemParameters.WorkArea.Size.Height - Height - 20;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NotifyTextBlock.Text = NotifyText;
        }

        private async void OpenStoryboard_OnCompleted(object sender, EventArgs e)
        {
            await Task.Delay(3000);
            BeginStoryboard(FindResource("CloseStoryboard") as Storyboard);
        }

        private void CloseStoryboard_OnCompleted(object sender, EventArgs e)
        {
            Close();
        }
    }
}