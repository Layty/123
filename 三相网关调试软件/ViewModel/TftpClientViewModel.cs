using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Tftp.Net;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class TftpClientViewModel : ObservableObject
    {
        public string RemoteIpAddress { get; set; }
        public int Port { get; set; }

        public string StatusLog
        {
            get => _log;
            set
            {
                _log = value;
                OnPropertyChanged();
            }
        }

        private string _log;


        public string DownLoadFileName
        {
            get => _downLoadFileName;
            set
            {
                _downLoadFileName = value;
                OnPropertyChanged();
            }
        }

        private string _downLoadFileName = "base_meter";


        public string UpLoadFileName
        {
            get => _upLoadFileName;
            set
            {
                _upLoadFileName = value;
                OnPropertyChanged();
            }
        }

        private string _upLoadFileName;


        public RelayCommand StartDownLoadCommand
        {
            get => _startDownLoadCommand;
            set
            {
                _startDownLoadCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _startDownLoadCommand;

        public RelayCommand StartUpLoadCommand
        {
            get => _startUpLoadCommand;
            set
            {
                _startUpLoadCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _startUpLoadCommand;
        public TftpClient TftpClient { get; set; }
        private string _tftpClientDirectory;

        public string TftpClientDirectory
        {
            get => _tftpClientDirectory;
            set
            {
                _tftpClientDirectory = value;
                OnPropertyChanged();
            }
        }

        

        public bool IsReadyToUpLoad
        {
            get => _isReadyToUpLoad;
            set { _isReadyToUpLoad = value; OnPropertyChanged(); }
        }
        private bool _isReadyToUpLoad;

        public TftpClientViewModel()
        {
            TftpClientDirectory = Properties.Settings.Default.TftpClientDirectory;
            StartDownLoadCommand = new RelayCommand(
                () =>
                {
                    TftpClient = new TftpClient("localhost");
                    var transfer = TftpClient.Download(DownLoadFileName);
                    Task.Run(() => { StartTransfer(transfer); });
                });

            SelectFileToUploadCommand = new RelayCommand(() =>
            {
                using (OpenFileDialog f = new OpenFileDialog())
                {
                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        _upLoadStream = f.OpenFile();
                        UpLoadFileName = f.SafeFileName;
                        IsReadyToUpLoad = true;
                    }
                }
            });
            StartUpLoadCommand = new RelayCommand(() =>
            {
                TftpClient = new TftpClient("localhost");
                var transfer = TftpClient.Upload(UpLoadFileName);
                if (IsReadyToUpLoad)
                {
                    Task.Run(() => { StartTransfer(transfer, _upLoadStream); });
                }
               
            });
        }

        public RelayCommand SelectFileToUploadCommand { get; set; }


        public void StreamToFile(string fileName)
        {
            // 把 Stream 转换成 byte[]
            byte[] bytes = new byte[_downLoadStream.Length];

            _downLoadStream.Read(bytes, 0, bytes.Length);

            // 设置当前流的位置为流的开始

            _downLoadStream.Seek(0, SeekOrigin.Begin);

            // 把 byte[] 写入文件

            FileStream fs = new FileStream(fileName, FileMode.Create);

            BinaryWriter bw = new BinaryWriter(fs);

            bw.Write(bytes);

            bw.Close();

            fs.Close();
        }

        public void StartClient()
        {
        }

        private Stream _downLoadStream;
        private Stream _upLoadStream;

        public void StartTransfer(ITftpTransfer transfer)
        {
            transfer.OnProgress += transfer_OnProgress;
            transfer.OnFinished += transferDownLoad_OnFinished;
            transfer.OnError += transfer_OnError;
            _downLoadStream = new MemoryStream();
            transfer.Start(_downLoadStream);
        }

        public void StartTransfer(ITftpTransfer transfer, Stream stream)
        {
            transfer.OnProgress += transfer_OnProgress;
            transfer.OnFinished += transferUpLoad_OnFinished;
            transfer.OnError += transfer_OnError;
            transfer.Start(stream);
        }


        private void transfer_OnError(ITftpTransfer transfer, TftpTransferError error)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(delegate { StatusLog += (error.ToString()); });
            transfer.OnProgress -= transfer_OnProgress;
            transfer.OnFinished -= transferDownLoad_OnFinished;
            transfer.OnError -= transfer_OnError;
            IsReadyToUpLoad = false;
        }

        private void transferDownLoad_OnFinished(ITftpTransfer transfer)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(delegate { StatusLog += "Transfer succeeded."; });
            var filePath = Path.Combine(TftpClientDirectory, DownLoadFileName);
            StreamToFile(filePath);
            transfer.OnProgress -= transfer_OnProgress;
            transfer.OnFinished -= transferDownLoad_OnFinished;
            transfer.OnError -= transfer_OnError;
        }

        private void transferUpLoad_OnFinished(ITftpTransfer transfer)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(delegate { StatusLog += "Transfer succeeded."; });
            transfer.OnProgress -= transfer_OnProgress;
            transfer.OnFinished -= transferUpLoad_OnFinished;
            transfer.OnError -= transfer_OnError;
            IsReadyToUpLoad = false;
        }

        private void transfer_OnProgress(ITftpTransfer transfer, TftpTransferProgress progress)
        {
            Task.Run(() => { Messenger.Default.Send(progress, "ClientProgressStatus"); });
        }
    }
}