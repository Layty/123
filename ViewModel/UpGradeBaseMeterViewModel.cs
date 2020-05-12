using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.FileTransmit;
using 三相智慧能源网关调试软件.Properties;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class UpGradeBaseMeterViewModel : ViewModelBase
    {
        private SerialPortViewModel _serialPortViewModel;

        public SerialPortViewModel SerialPortViewModel
        {
            get => _serialPortViewModel;
            set
            {
                _serialPortViewModel = value;
                RaisePropertyChanged();
            }
        }

        private bool _isInitUpGradeSerialPort;

        public bool IsInitUpGradeSerialPort
        {
            get => _isInitUpGradeSerialPort;
            set
            {
                if (value)
                {
                    if (!SerialPortViewModel.SerialPortMasterModel.IsAutoDataReceived)
                    {
                        SerialPortViewModel.SerialPortMasterModel.IsAutoDataReceived = true;
                    }
                    //SerialPortViewModel.SerialPortMasterModel.SerialPort.DataReceived += SerialPort_DataReceived;
                    SerialPortViewModel.SerialPortMasterModel.MySerialDataReceived += SerialPortMasterModel_MySerialDataReceived;
                }
                else
                {
                    //SerialPortViewModel.SerialPortMasterModel.SerialPort.DataReceived -= SerialPort_DataReceived;
                    SerialPortViewModel.SerialPortMasterModel.MySerialDataReceived -= SerialPortMasterModel_MySerialDataReceived;
                }

                _isInitUpGradeSerialPort = value;
                RaisePropertyChanged();
            }
        }

        private void SerialPortMasterModel_MySerialDataReceived(MySerialPortMaster.SerialPortMaster source, MySerialPortMaster.SerialPortEventArgs e)
        {
            FileTransmitProtocol.ReceivedFromUart(e.DataBytes);
        }

        public IFileTransmit FileTransmitProtocol
        {
            get => _fileTransmitProtocol;
            set
            {
                _fileTransmitProtocol = value;
                RaisePropertyChanged();
            }
        }

        private IFileTransmit _fileTransmitProtocol;

        public int FileIndex
        {
            get => _fileIndex;
            set
            {
                _fileIndex = value;
                RaisePropertyChanged();
            }
        }

        private int _fileIndex;

        private int _packetNo;

        public int PacketNo
        {
            get => _packetNo;
            set
            {
                _packetNo = value;
                RaisePropertyChanged();
            }
        }

        private string _fileName;

        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                RaisePropertyChanged();
            }
        }

        private long _fileSize;

        public long FileSize
        {
            get => _fileSize;
            set
            {
                _fileSize = value;
                RaisePropertyChanged();
            }
        }

        private int _currentCount;

        public int CurrentCount
        {
            get => _currentCount;
            set
            {
                _currentCount = value;
                RaisePropertyChanged();
            }
        }


        public byte[] PacketBuff
        {
            get => _packetBuff;
            set
            {
                _packetBuff = value;
                RaisePropertyChanged();
            }
        }

        private byte[] _packetBuff;
        public int PacketLen { get; set; }


        private TransmitMode _transmitMode;

        public TransmitMode TransmitMode
        {
            get => _transmitMode;
            set
            {
                _transmitMode = value;
                RaisePropertyChanged();
            }
        }

        private YModemType _yModemType;

        public YModemType YModemType
        {
            get => _yModemType;
            set
            {
                _yModemType = value;
                RaisePropertyChanged();
            }
        }

        public Array YModemTypeArray => Enum.GetValues(typeof(YModemType));


        public UpGradeBaseMeterViewModel()
        {
            SerialPortViewModel = ServiceLocator.Current.GetInstance<SerialPortViewModel>();
            TransmitMode = TransmitMode.Send;
            YModemType = YModemType.YModem_1K;
            PacketLen = 1024;
            //默认初始化为YModem_1K，1024 ，Send
            FileTransmitProtocol = new YModem(TransmitMode, YModemType, 10);
            UserComCommand = new RelayCommand(() =>
            {
                //SerialPortViewModel.SerialPortMasterModel.SerialPort.DataReceived += SerialPort_DataReceived;
                SerialPortViewModel.SerialPortMasterModel.MySerialDataReceived +=
                    SerialPortMasterModel_MySerialDataReceived;
            });
            ReleaseComCommand = new RelayCommand(() =>
            {
                // SerialPortViewModel.SerialPortMasterModel.SerialPort.DataReceived -= SerialPort_DataReceived;
                SerialPortViewModel.SerialPortMasterModel.MySerialDataReceived -=
                    SerialPortMasterModel_MySerialDataReceived;
            });
            StartCommand = new RelayCommand(() =>
            {
                FileTransmitProtocol = new YModem(TransmitMode, YModemType, 10);
                switch (YModemType)
                {
                    case YModemType.YModem:
                        PacketLen = 128;
                        break;
                    case YModemType.YModem_1K:
                        PacketLen = 1024;
                        break;
                    default:
                        PacketLen = 1024;
                        break;
                }

                FileTransmitProtocol.StartSend += YModem_StartSend;
                FileTransmitProtocol.SendNextPacket += YModem_SendNextPacket;
                FileTransmitProtocol.SendToUartEvent += YModem_SendToUartEvent;
                FileTransmitProtocol.EndOfTransmit += YModem_EndOfTransmit;
                FileTransmitProtocol.ReceivedPacket += YModem_ReceivedPacket;
                FileTransmitProtocol.ReSendPacket += YModem_ReSendPacket;

                PacketNo = 1;
                FileIndex = 0;
                FileTransmitProtocol.Start();
            });

            StopCommand = new RelayCommand(() => { FileTransmitProtocol.Stop(); });

            SelectFileCommand = new RelayCommand(SelectFile);
            FileName = Settings.Default.BaseMeterUpGradeFile;
        }


        private RelayCommand _startCommand;

        public RelayCommand StartCommand
        {
            get => _startCommand;
            set
            {
                _startCommand = value;
                RaisePropertyChanged();
            }
        }


        private RelayCommand _stopCommand;

        public RelayCommand StopCommand
        {
            get => _stopCommand;
            set
            {
                _stopCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _useComCommand;

        public RelayCommand UserComCommand
        {
            get => _useComCommand;
            set
            {
                _useComCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _releaseComCommand;

        public RelayCommand ReleaseComCommand
        {
            get => _releaseComCommand;
            set
            {
                _releaseComCommand = value;
                RaisePropertyChanged();
            }
        }


        private void YModem_ReSendPacket(object sender, EventArgs e)
        {
            FileTransmitProtocol.SendPacket(new PacketEventArgs(PacketNo, PacketBuff));
        }

        private void YModem_ReceivedPacket(object sender, PacketEventArgs e)
        {
            throw new NotImplementedException();
        }


        private void YModem_EndOfTransmit(object sender, EventArgs e)
        {
            if (FileTransmitProtocol == null || SerialPortViewModel == null)
            {
                return;
            }

            FileTransmitProtocol.StartSend -= YModem_StartSend;
            FileTransmitProtocol.SendNextPacket -= YModem_SendNextPacket;
            FileTransmitProtocol.SendToUartEvent -= YModem_SendToUartEvent;
            FileTransmitProtocol.EndOfTransmit -= YModem_EndOfTransmit;
            FileTransmitProtocol.ReceivedPacket -= YModem_ReceivedPacket;
            FileTransmitProtocol.ReSendPacket -= YModem_ReSendPacket;
        }

        private void YModem_StartSend(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(FileName))
            {
                PacketNo = 0;
                FileInfo fileInfo = new FileInfo(FileName);
                byte[] fileNameBytes = Encoding.Default.GetBytes(fileInfo.Name);
                int index = 0;
                PacketBuff = new byte[PacketLen];
                Array.Copy(fileNameBytes, 0, PacketBuff, 0, fileNameBytes.Length);
                index += fileNameBytes.Length;
                PacketBuff[index] = 0;
                index++;
                byte[] fileSizeBytes = Encoding.Default.GetBytes(fileInfo.Length.ToString());
                FileSize = fileInfo.Length;
                Array.Copy(fileSizeBytes, 0, PacketBuff, index, fileSizeBytes.Length);
                FileTransmitProtocol.SendPacket(new PacketEventArgs(0, PacketBuff));
                FileIndex = 0;
                PacketNo = 0;
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //byte[] dBytes = new byte[_serialPortViewModel.SerialPortMasterModel.SerialPort.BytesToRead];
            //_serialPortViewModel.SerialPortMasterModel.SerialPort.Read(dBytes, 0, dBytes.Length);
            //_serialPortViewModel.SerialPortMasterModel.SerialPortLogger.SendAndReceiveDataCollections =
            //    Encoding.Default.GetString(dBytes);
            //_serialPortViewModel.SerialPortMasterModel.SerialPortLogger.SendAndReceiveDataCollections =
            //    $"{DateTime.Now} <= {dBytes.ByteToString()}{Environment.NewLine}";
            //FileTransmitProtocol.ReceivedFromUart(dBytes);
        }

        private void YModem_SendNextPacket(object sender, EventArgs e)
        {
            PacketBuff = new byte[PacketLen];
            PacketNo++;
            FileIndex += PacketLen;

            if (PacketNo == 1)
            {
                FileIndex = 0;
            }

            int readBytes = ReadPacketFromFile(FileIndex, PacketBuff, PacketLen);

            if (readBytes <= 0)
            {
                FileTransmitProtocol.Stop();
            }
            else
            {
                if (YModemType == YModemType.YModem || YModemType == YModemType.YModem_1K)
                {
                    if (readBytes < PacketLen)
                    {
                        CurrentCount += readBytes; //补全
                        for (int i = readBytes; i < PacketLen; i++)
                        {
                            PacketBuff[i] = 0x1A;
                        }
                    }
                }

                FileTransmitProtocol.SendPacket(new PacketEventArgs(PacketNo, PacketBuff));
            }
        }

        private void YModem_SendToUartEvent(object sender, SendToUartEventArgs e)
        {
            if (!_serialPortViewModel.SerialPortMasterModel.IsOpen)
            {
                _serialPortViewModel.SerialPortMasterModel.Open();
            }

            _serialPortViewModel.SerialPortMasterModel.Send(e.Data); //使用Send可捕捉发送日志
            //_serialPortViewModel.SerialPortMasterModel.SerialPort.Write(e.Data, 0, e.Data.Length);
            //  Messenger.Default.Send(e.Data, "SendFileToBaseMeter");
        }


        private int ReadPacketFromFile(int filePos, byte[] data, int packetLen)
        {
            try
            {
                if (string.IsNullOrEmpty(FileName)) return 0;
                using (FileStream fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read))
                {
                    BinaryReader binaryReader = new BinaryReader(fileStream);
                    if (filePos < fileStream.Length)
                    {
                        fileStream.Seek(filePos, SeekOrigin.Begin);
                        int len = binaryReader.Read(data, 0, packetLen);

                        FileSize = fileStream.Length;
                        CurrentCount = filePos;

                        return len;
                    }

                    return 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        private RelayCommand _selectFileCommand;

        public RelayCommand SelectFileCommand
        {
            get => _selectFileCommand;
            set
            {
                _selectFileCommand = value;
                RaisePropertyChanged();
            }
        }


        private void SelectFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "二进制文件(*.bin)|*.bin|所有文件(*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                FileName = openFileDialog.FileName;
                Settings.Default.BaseMeterUpGradeFile = FileName;
                Settings.Default.Save();
            }
        }
    }
}