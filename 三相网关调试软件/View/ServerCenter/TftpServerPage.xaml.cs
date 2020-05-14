using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Tftp.Net;

namespace 三相智慧能源网关调试软件.View.ServerCenter
{
    /// <summary>
    /// TftpServerPage.xaml 的交互逻辑
    /// </summary>
    public partial class TftpServerPage : Page
    {
        public TftpServerPage()
        {
            InitializeComponent();
            Messenger.Default.Register<TftpTransferProgress>(this, "ProgressStatus", UpdateProgressStatus);
        }

        private void UpdateProgressStatus(TftpTransferProgress transferProgress)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                WaveProgressBar.Maximum = transferProgress.TotalBytes;
                WaveProgressBar.Value = transferProgress.TransferredBytes;
              //  WaveProgressBar.Text = obj.ToString();
            });
        }
    }
}