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

namespace 三相智慧能源网关调试软件.MyControl
{
    /// <summary>
    /// MessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class MessageBox : UserControl
    {
        public string Title { get; set; }
        public string Message { get; set; } = "Message";
        public MessageBox(string message,string title)
        {
            InitializeComponent();
            Message = message;
            Title = title;
            DataContext = this;
        }
    }
}
