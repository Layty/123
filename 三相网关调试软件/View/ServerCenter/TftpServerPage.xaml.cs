using System;
using System.Threading.Tasks;
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
//            Messenger.Default.Register<TftpTransferProgress>(this, "ServerProgressStatus", UpdateProgressStatus);
//            Messenger.Default.Register<TftpTransferProgress>(this, "ClientProgressStatus", UpdateClientProgressStatus);
        }

        private void UpdateClientProgressStatus(TftpTransferProgress transferProgress)
        {
//            Dispatcher.BeginInvoke(new Action(() =>
//            {
//                ClientProgressBar.Maximum = transferProgress.TotalBytes;
//                ClientProgressBar.Value = transferProgress.TransferredBytes;
//            }));
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                ClientProgressBar.Maximum = transferProgress.TotalBytes;
                ClientProgressBar.Value = transferProgress.TransferredBytes;
            });
        }

        private void UpdateProgressStatus(TftpTransferProgress transferProgress)
        {
//            Dispatcher.BeginInvoke(new Action(() => {
//                WaveProgressBar.Maximum = transferProgress.TotalBytes;
//                WaveProgressBar.Value = transferProgress.TransferredBytes;
//            }));
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                WaveProgressBar.Maximum = transferProgress.TotalBytes;
                WaveProgressBar.Value = transferProgress.TransferredBytes;
            });
        }
    }
}