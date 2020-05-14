using System.Windows.Controls;

namespace 三相智慧能源网关调试软件.MyControl
{
    /// <summary>
    /// MessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class MessageBox : UserControl
    {
        public string Title { get; set; }
        public string Message { get; set; } = "Message";

        public MessageBox()
        {
            InitializeComponent();
            DataContext = this;
        }
        public MessageBox(string message, string title)
        {
            InitializeComponent();
            Message = message;
            Title = title;
            DataContext = this;
        }
    }
}
