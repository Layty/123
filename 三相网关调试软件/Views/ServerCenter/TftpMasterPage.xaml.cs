using System.Windows.Controls;
using Tftp.Net;
using 三相智慧能源网关调试软件.Helpers;

namespace 三相智慧能源网关调试软件.Views.ServerCenter
{
    /// <summary>
    /// TftpMasterPage.xaml 的交互逻辑
    /// </summary>
    public partial class TftpMasterPage : Page
    {
        public TftpMasterPage()
        {
            InitializeComponent();
            //            Messenger.Default.Register<TftpTransferProgress>(this, "ServerProgressStatus", UpdateProgressStatus);
            //            Messenger.Default.Register<TftpTransferProgress>(this, "ClientProgressStatus", UpdateClientProgressStatus);
        }

        private void UpdateClientProgressStatus(TftpTransferProgress transferProgress)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                ClientProgressBar.Maximum = transferProgress.TotalBytes;
                ClientProgressBar.Value = transferProgress.TransferredBytes;
            });
        }

        private void UpdateProgressStatus(TftpTransferProgress transferProgress)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                WaveProgressBar.Maximum = transferProgress.TotalBytes;
                WaveProgressBar.Value = transferProgress.TransferredBytes;
            });
        }
    }
}