using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tftp.Net;
using 三相智慧能源网关调试软件.Helpers;
using 三相智慧能源网关调试软件.Properties;


namespace 三相智慧能源网关调试软件.ViewModel
{
    public class TftpServerViewModel : ObservableObject
    {
        private TftpServer _tftpServer;
        private readonly OpenFileDialog _openFileDialog;
        private readonly FolderBrowserDialog _folderBrowserDialog1;
        private bool _isStarted;

        public bool IsStarted
        {
            get => _isStarted;
            set
            {
                _isStarted = value;
                OnPropertyChanged();
            }
        }

        private string _tftpServerDirectory;

        public string TftpServerDirectory
        {
            get => _tftpServerDirectory;
            set
            {
                _tftpServerDirectory = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> _directoryCollection;

        public ObservableCollection<string> DirectoryCollection
        {
            get => _directoryCollection;
            set
            {
                _directoryCollection = value;
                OnPropertyChanged();
            }
        }

        private string _log;

        public string StatusLog
        {
            get => _log;
            set
            {
                _log = value;
                OnPropertyChanged();
            }
        }


        public TftpServerViewModel()
        {
            TftpServerDirectory = Settings.Default.TftpServerDirectory;
            DirectoryCollection = new ObservableCollection<string>();
            _folderBrowserDialog1 = new FolderBrowserDialog { SelectedPath = TftpServerDirectory };

            BrowseCommand = new RelayCommand(BrowseDialog);

            _openFileDialog = new OpenFileDialog();
            OpenDialogCommand = new RelayCommand(OpenFileDialog);

            StartServerCommand = new RelayCommand(() =>
            {
                if (GetServerDirectory())
                {
                    _tftpServer = new TftpServer();

                    _tftpServer.OnReadRequest += TftpServer_OnReadRequest;
                    _tftpServer.OnWriteRequest += TftpServer_OnWriteRequest;
                    _tftpServer.OnError += TftpServer_OnError;
                    _tftpServer?.Start();
                    IsStarted = true;

                }
            });
            StopServerCommand = new RelayCommand(() =>
            {
                _tftpServer.OnReadRequest -= TftpServer_OnReadRequest;
                _tftpServer.OnWriteRequest -= TftpServer_OnWriteRequest;
                _tftpServer.OnError -= TftpServer_OnError;
                _tftpServer?.Dispose();
                DirectoryCollection.Clear();
                IsStarted = false;
            });
        }

        public void OpenFileDialog()
        {
            _openFileDialog.InitialDirectory = Environment.CurrentDirectory + TftpServerDirectory;
            _openFileDialog.ShowDialog();
        }


        private RelayCommand _browseCommand;

        public RelayCommand BrowseCommand
        {
            get => _browseCommand;
            set
            {
                _browseCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _openDialogCommand;

        public RelayCommand OpenDialogCommand
        {
            get => _openDialogCommand;
            set
            {
                _openDialogCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _startCommand;

        public RelayCommand StartServerCommand
        {
            get => _startCommand;
            set
            {
                _startCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _stopCommand;

        public RelayCommand StopServerCommand
        {
            get => _stopCommand;
            set
            {
                _stopCommand = value;
                OnPropertyChanged();
            }
        }


        private void BrowseDialog()
        {
            DialogResult result = _folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                TftpServerDirectory = _folderBrowserDialog1.SelectedPath;
            }
        }


        private void TftpServer_OnError(TftpTransferError error)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            { StatusLog += (error.ToString()); });
        }

        /// <summary>
        /// 请求
        /// </summary>
        /// <param name="transfer"></param>
        /// <param name="client"></param>
        private void TftpServer_OnReadRequest(ITftpTransfer transfer, EndPoint client)
        {
            string path = Path.Combine(Environment.CurrentDirectory + TftpServerDirectory, transfer.Filename);
            FileInfo file = new FileInfo(path);

            if (!file.Exists)
            {
                CancelTransfer(transfer, TftpErrorPacket.FileNotFound);
            }
            else
            {
                OutputTransferStatus(transfer, "Accepting request from " + client);
                StartTransfer(transfer, new FileStream(file.FullName, FileMode.Open));
            }
        }

        private void TftpServer_OnWriteRequest(ITftpTransfer transfer, EndPoint client)
        {

            string file = Path.Combine(Environment.CurrentDirectory + _tftpServerDirectory, transfer.Filename);
            bool flag = File.Exists(file);
            if (flag)
            {
                CancelTransfer(transfer, TftpErrorPacket.FileAlreadyExists);
            }
            else
            {
                OutputTransferStatus(transfer, "Accepting write request from " + client);
                StartTransfer(transfer, new FileStream(file, FileMode.CreateNew));
            }

        }


        private void OutputTransferStatus(ITftpTransfer transfer, string acceptingWriteRequestFrom)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                StatusLog += $"[{transfer.Filename}] {acceptingWriteRequestFrom}]";
            });
        }

        private void CancelTransfer(ITftpTransfer transfer, TftpErrorPacket reason)
        {
            OutputTransferStatus(transfer, "Cancelling transfer: " + reason.ErrorMessage);
            transfer.Cancel(reason);
        }

        private void StartTransfer(ITftpTransfer transfer, Stream stream)
        {
            transfer.OnProgress += Transfer_OnProgress;
            transfer.OnError += Transfer_OnError;
            transfer.OnFinished += Transfer_OnFinished;
            transfer.Start(stream);
        }

        private void Transfer_OnFinished(ITftpTransfer transfer)
        {
            OutputTransferStatus(transfer, "Finished");
            transfer.OnProgress -= Transfer_OnProgress;
            transfer.OnError -= Transfer_OnError;
            transfer.OnFinished -= Transfer_OnFinished;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                DirectoryCollection.Add(transfer.Filename);//完成后更新列表
            });


        }

        private void Transfer_OnError(ITftpTransfer transfer, TftpTransferError error)
        {
            OutputTransferStatus(transfer, "Error: " + error);
        }

        private void Transfer_OnProgress(ITftpTransfer transfer, TftpTransferProgress progress)
        {
            Task.Run(() => { StrongReferenceMessenger.Default.Send(progress, "ServerProgressStatus"); });
        }


        public bool GetServerDirectory()
        {
            if (!string.IsNullOrEmpty(TftpServerDirectory) && Directory.Exists(TftpServerDirectory))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(TftpServerDirectory);
                DirectoryCollection.Clear();
                foreach (FileInfo fileName in directoryInfo.GetFiles())
                {
                    DirectoryCollection.Add(fileName.ToString());
                }

                return true;
            }

            return false;
        }
    }
}