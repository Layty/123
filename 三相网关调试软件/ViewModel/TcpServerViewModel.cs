using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS;
using 三相智慧能源网关调试软件.Properties;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class TcpServerViewModel : ViewModelBase
    {
        public TcpServerHelper TcpServerHelper
        {
            get => _tcpServerHelper;
            set
            {
                _tcpServerHelper = value;
                RaisePropertyChanged();
            }
        }

        private TcpServerHelper _tcpServerHelper;


        public RelayCommand StartListen
        {
            get => _startListen;
            set
            {
                _startListen = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _startListen;

        public RelayCommand DisConnectServerCommand
        {
            get => _disConnectServerCommand;
            set
            {
                _disConnectServerCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _disConnectServerCommand;

        public RelayCommand<string> DisConnectClientCommand
        {
            get => _disConnectClientCommand;
            set
            {
                _disConnectClientCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand<string> _disConnectClientCommand;


        public RelayCommand SendDataToServerCommand
        {
            get => _sendDataToServerCommand;
            set
            {
                _sendDataToServerCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _sendDataToServerCommand;


        public Socket CurrentSocketClient
        {
            get => _currentSocketClient;
            set
            {
                _currentSocketClient = value;
                RaisePropertyChanged();
            }
        }

        private Socket _currentSocketClient;


        public string CurrentSendMsg
        {
            get => _currentSendMsg;
            set
            {
                _currentSendMsg = value;
                RaisePropertyChanged();
            }
        }

        private string _currentSendMsg;


        public RelayCommand<Socket> SelectSocketCommand
        {
            get => _selectSocketCommand;
            set
            {
                _selectSocketCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand<Socket> _selectSocketCommand;


        public DLMSClient DLMSClient
        {
            get => _DLMSClient;
            set
            {
                _DLMSClient = value;
                RaisePropertyChanged();
            }
        }

        private DLMSClient _DLMSClient;


        public bool IsAutoResponseHeartBeat
        {
            get => _isAutoResponseHeartBeat;
            set
            {
                _isAutoResponseHeartBeat = value;
                RaisePropertyChanged();
            }
        }

        private bool _isAutoResponseHeartBeat;


        public TcpServerViewModel()
        {
            TcpServerHelper = new TcpServerHelper(Settings.Default.GatewayIpAddress, 8881);
            IsAutoResponseHeartBeat = true;
            TcpServerHelper.ReceiveBytes += TcpServerHelper_ReceiveBytes;
            CurrentSendMsg = "00 02 00 16 00 02 00 0F 00 01 03 30 30 30 30 30 30 30 30 30 30 30 31";
            SelectSocketCommand = new RelayCommand<Socket>(Select);
            StartListen = new RelayCommand(() => { TcpServerHelper.StartListen(); });
            DisConnectServerCommand = new RelayCommand(TcpServerHelper.CloseSever);
            DisConnectClientCommand = new RelayCommand<string>(t => TcpServerHelper.DisConnectClient(t));
            SendDataToServerCommand = new RelayCommand(() =>
            {
                TcpServerHelper.SendDataToClient(CurrentSocketClient, CurrentSendMsg.StringToByte());
            });
        }

        public class HeartBeatFrame
        {
            public byte[] VersionBytes { get; set; }
            public byte[] SourceAddressBytes { get; set; }
            public byte[] DestinationAddressBytes { get; set; }
            public byte[] LengthBytes { get; set; }
            public byte[] MeterAddressBytes { get; set; }
            public byte[] HeartBeatFrameType { get; set; }

            public HeartBeatFrame()
            {
                VersionBytes = new byte[] {0x00, 0x02};
                HeartBeatFrameType = new byte[] {0x00, 0x01, 0x03};
            }

            public bool ParseHeartBeatFrame(byte[] bytes, out byte[] outPutBytes)
            {
                outPutBytes = new byte[] { };
                if (bytes == null || bytes.Length <= 11)
                {
                    return false;
                }

                //比对版本号
                if (!Common.ByteArraysEqual(bytes.Take(2).ToArray(), VersionBytes))
                {
                    return false;
                }


                {
                    SourceAddressBytes = bytes.Skip(2).Take(2).ToArray();
                    DestinationAddressBytes = bytes.Skip(4).Take(2).ToArray();
                    LengthBytes = bytes.Skip(6).Take(2).ToArray();
                    var length = BitConverter.ToUInt16(LengthBytes.Reverse().ToArray(), 0);
                    if (bytes.Skip(8).ToArray().Length != length)
                        return false;
                    var data = bytes.Skip(8).ToArray();
                    if (!Common.ByteArraysEqual(data.Take(3).ToArray(), HeartBeatFrameType))
                        return false;
                    MeterAddressBytes = data.Skip(3).ToArray();
                    outPutBytes = BuildBytes();
                    return true;
                }
            }

            private byte[] BuildBytes()
            {
                List<byte> list = new List<byte>();
                list.AddRange(VersionBytes);
                list.AddRange(DestinationAddressBytes);
                list.AddRange(SourceAddressBytes);
                list.AddRange(LengthBytes);
                list.AddRange(HeartBeatFrameType);
                list.AddRange(MeterAddressBytes);
                return list.ToArray();
            }
        }

        private void TcpServerHelper_ReceiveBytes(Socket clientSocket, byte[] bytes)
        {
            if (!IsAutoResponseHeartBeat)
            {
                return;
            }
            try
            {
                var heart = new HeartBeatFrame();
                var result = heart.ParseHeartBeatFrame(bytes, out var outPutBytes);
                if (result)
                {
                    TcpServerHelper.SendDataToClient(clientSocket, outPutBytes);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Select(Socket clientSocket)
        {
            DLMSClient.CurrentSocket = clientSocket;
            CurrentSocketClient = clientSocket;
        }
    }
}