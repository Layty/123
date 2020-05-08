using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;

namespace 三相智慧能源网关调试软件.View.BaseMeter
{
    /// <summary>
    /// UpGradeBaseMeterPage.xaml 的交互逻辑
    /// </summary>
    public partial class UpGradeBaseMeterPage : Page
    {
        public UpGradeBaseMeterPage()
        {
            InitializeComponent();
            Messenger.Default.Register<(int, int)>(this, "ReportProgressBar", ReportProgressBar);
        }

        private void ReportProgressBar((int, int) obj)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                ProgressBar.Value = obj.Item1;
                ProgressBar.Maximum = obj.Item2;
            });
        }
    }
}