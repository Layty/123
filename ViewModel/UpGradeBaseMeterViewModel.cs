using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
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
                    SerialPortViewModel.SerialPortMasterModel.SerialPort.DataReceived += SerialPort_DataReceived;
                }
                else
                {
                    SerialPortViewModel.SerialPortMasterModel.SerialPort.DataReceived -= SerialPort_DataReceived;
                }

                _isInitUpGradeSerialPort = value;
                RaisePropertyChanged();
            }
        }


        public YModem YModem
        {
            get => _yModem;
            set
            {
                _yModem = value;
                RaisePropertyChanged();
            }
        }

        private YModem _yModem;

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
                Percentage = (_currentCount / FileSize)*100;
                RaisePropertyChanged();
            }
        }


        public double Percentage
        {
            get => _percentage;
            set { _percentage = value; RaisePropertyChanged(); }
        }

        private double _percentage;



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
            YModem = new YModem(TransmitMode, YModemType, 10);
            UserComCommand = new RelayCommand(() =>
            {
                SerialPortViewModel.SerialPortMasterModel.SerialPort.DataReceived += SerialPort_DataReceived;
            });
            ReleaseComCommand = new RelayCommand(() =>
            {
                SerialPortViewModel.SerialPortMasterModel.SerialPort.DataReceived -= SerialPort_DataReceived;
            });
            StartCommand = new RelayCommand(() =>
            {
                YModem = new YModem(TransmitMode, YModemType, 10);
                YModem.StartSend += YModem_StartSend;
                YModem.SendNextPacket += YModem_SendNextPacket;
                YModem.SendToUartEvent += YModem_SendToUartEvent;
                YModem.EndOfTransmit += YModem_EndOfTransmit;
                YModem.ReceivedPacket += YModem_ReceivedPacket;
                YModem.ReSendPacket += YModem_ReSendPacket;

                PacketNo = 1;
                FileIndex = 0;
                YModem.Start();
            });

            StopCommand = new RelayCommand(() => { YModem.Stop(); });

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
            get { return _releaseComCommand; }
            set
            {
                _releaseComCommand = value;
                RaisePropertyChanged();
            }
        }


        private void YModem_ReSendPacket(object sender, EventArgs e)
        {
            YModem.SendPacket(new PacketEventArgs(PacketNo, PacketBuff));
        }

        private void YModem_ReceivedPacket(object sender, PacketEventArgs e)
        {
            throw new NotImplementedException();
        }


        private void YModem_EndOfTransmit(object sender, EventArgs e)
        {
            if (YModem == null || SerialPortViewModel == null)
            {
                return;
            }
           
            YModem.StartSend -= YModem_StartSend;
            YModem.SendNextPacket -= YModem_SendNextPacket;
            YModem.SendToUartEvent -= YModem_SendToUartEvent;
            YModem.EndOfTransmit -= YModem_EndOfTransmit;
            YModem.ReceivedPacket -= YModem_ReceivedPacket;
            YModem.ReSendPacket -= YModem_ReSendPacket;
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
                YModem.SendPacket(new PacketEventArgs(0, PacketBuff));
                FileIndex = 0;
                PacketNo = 0;
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] dBytes = new byte[_serialPortViewModel.SerialPortMasterModel.SerialPort.BytesToRead];
            _serialPortViewModel.SerialPortMasterModel.SerialPort.Read(dBytes, 0, dBytes.Length);
            _serialPortViewModel.SerialPortMasterModel.SerialPortLogger.SendAndReceiveDataCollections =
                Encoding.Default.GetString(dBytes);
          //  Messenger.Default.Send(dBytes, "ReceiveMsgFormBaseMeter");
            YModem.ReceivedFromUart(dBytes);
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
                YModem.Stop();
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

                YModem.SendPacket(new PacketEventArgs(PacketNo, PacketBuff));
            }
        }

        private void YModem_SendToUartEvent(object sender, SendToUartEventArgs e)
        {
            if (!_serialPortViewModel.SerialPortMasterModel.SerialPort.IsOpen)
            {
                _serialPortViewModel.SerialPortMasterModel.SerialPort.Open();
            }
            _serialPortViewModel.SerialPortMasterModel.Send(e.Data);//使用Send可捕捉发送日志
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