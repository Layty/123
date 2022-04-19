using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Tftp.Net;
using 三相智慧能源网关调试软件.Helpers;

namespace 三相智慧能源网关调试软件.ViewModels
{
    public class TftpClientViewModel : ObservableObject
    {
        /// <summary>
        /// 远端TFTP服务器IP
        /// </summary>
        public string RemoteIpAddress
        {
            get => _remoteIpAddress;
            set
            {
                _remoteIpAddress = value;
                OnPropertyChanged();
            }
        }

        private string _remoteIpAddress = "127.0.0.1";

        /// <summary>
        /// 远端TFTP服务器Port,默认69
        /// </summary>
        public int Port
        {
            get => _port;
            set
            {
                _port = value;
                OnPropertyChanged();
            }
        }

        private int _port = 69;

        /// <summary>
        /// 日志记录
        /// </summary>
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

        /// <summary>
        /// 需要从服务端当前目录下的的文件名
        /// </summary>
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

        /// <summary>
        /// TFTP客户端当前目录
        /// </summary>
        public string TftpClientDirectory
        {
            get => _tftpClientDirectory;
            set
            {
                _tftpClientDirectory = value;
                OnPropertyChanged();
            }
        }

        private string _tftpClientDirectory;

        /// <summary>
        /// 需要从本地客户端上传至服务器的文件名
        /// </summary>
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

        /// <summary>
        /// 是否准备好了上传
        /// </summary>
        public bool IsReadyToUpLoad
        {
            get => _isReadyToUpLoad;
            set
            {
                _isReadyToUpLoad = value;
                OnPropertyChanged();
            }
        }

        private bool _isReadyToUpLoad;
        private Stream _downLoadStream;
        private Stream _upLoadStream;

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

        public RelayCommand SelectFileToUploadCommand { get; set; }

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

        public TftpClientViewModel()
        {
            TftpClientDirectory = Properties.Settings.Default.TftpClientDirectory;
            StartDownLoadCommand = new RelayCommand(
                () =>
                {
                    TftpClient = new TftpClient(IPAddress.Parse(RemoteIpAddress), Port);
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
                TftpClient = new TftpClient(IPAddress.Parse(RemoteIpAddress), Port);
                var transfer = TftpClient.Upload(UpLoadFileName);
                if (IsReadyToUpLoad)
                {
                    Task.Run(() => { StartTransfer(transfer, _upLoadStream); });
                }
            });
        }

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

        public void StartTransfer(ITftpTransfer transfer)
        {
            transfer.OnProgress += Transfer_OnProgress;
            transfer.OnFinished += TransferDownLoad_OnFinished;
            transfer.OnError += Transfer_OnError;
            _downLoadStream = new MemoryStream();
            transfer.Start(_downLoadStream);
        }

        public void StartTransfer(ITftpTransfer transfer, Stream stream)
        {
            transfer.OnProgress += Transfer_OnProgress;
            transfer.OnFinished += TransferUpLoad_OnFinished;
            transfer.OnError += Transfer_OnError;
            transfer.Start(stream);
        }

        private void Transfer_OnError(ITftpTransfer transfer, TftpTransferError error)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => StatusLog += error.ToString());
            transfer.OnProgress -= Transfer_OnProgress;
            transfer.OnFinished -= TransferDownLoad_OnFinished;
            transfer.OnError -= Transfer_OnError;
            IsReadyToUpLoad = false;
        }

        private void TransferDownLoad_OnFinished(ITftpTransfer transfer)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => StatusLog += "TransferDownLoad succeeded.");
            var filePath = Path.Combine(TftpClientDirectory, DownLoadFileName);
            StreamToFile(filePath);
            transfer.OnProgress -= Transfer_OnProgress;
            transfer.OnFinished -= TransferDownLoad_OnFinished;
            transfer.OnError -= Transfer_OnError;
        }

        private void TransferUpLoad_OnFinished(ITftpTransfer transfer)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => { StatusLog += "TransferUpLoad succeeded."; });
            transfer.OnProgress -= Transfer_OnProgress;
            transfer.OnFinished -= TransferUpLoad_OnFinished;
            transfer.OnError -= Transfer_OnError;
            IsReadyToUpLoad = false;
        }

        private void Transfer_OnProgress(ITftpTransfer transfer, TftpTransferProgress progress)
        {
            StrongReferenceMessenger.Default.Send(progress, "ClientProgressStatus");
        }
    }
}