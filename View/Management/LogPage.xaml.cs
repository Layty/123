using System.Windows.Controls;

namespace 三相智慧能源网关调试软件.View.Management
{
    /// <summary>
    /// LogPage.xaml 的交互逻辑
    /// </summary>
    public partial class LogPage : Page
    {
        public LogPage()
        {
            InitializeComponent();
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            
                TextBoxReceive.ScrollToEnd();
           
        }
    }
}
