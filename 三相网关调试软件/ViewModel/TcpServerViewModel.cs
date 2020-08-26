using System;
using System.Net.Sockets;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects;
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

        public string CurrentSendMsgLast
        {
            get => _currentSendMsgLast;
            set
            {
                _currentSendMsgLast = value;
                RaisePropertyChanged();
            }
        }

        private string _currentSendMsgLast;
        public string CurrentSendMsgEarly
        {
            get => _currentSendMsgEarly;
            set
            {
                _currentSendMsgEarly = value;
                RaisePropertyChanged();
            }
        }

        private string _currentSendMsgEarly;
        public string CurrentSendMsgAboutTime
        {
            get => _currentSendMsgAboutTime;
            set
            {
                _currentSendMsgAboutTime = value;
                RaisePropertyChanged();
            }
        }

        private string _currentSendMsgAboutTime;
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

        private TcpTranslator translator { get; set; }
        public TcpServerViewModel()
        {
            TcpServerHelper = new TcpServerHelper(Settings.Default.GatewayIpAddress, 8881);
            IsAutoResponseHeartBeat = true;
            TcpServerHelper.ReceiveBytes += TcpServerHelper_ReceiveBytes;
            CurrentSendMsg = "00 02 00 16 00 02 00 0F 00 01 03 30 30 30 30 30 30 30 30 30 30 30 31";
            CurrentSendMsgLast =
                "00 01 00 01 00 01 00 16 C0 01 40 20 6D 00 01 80 00 00 FF 02 01 02 02 02 12 00 01 12 00 01";
            CurrentSendMsgEarly =
                "00 01 00 01 00 01 00 16 C0 01 40 20 6D 00 01 80 00 00 FF 02 01 02 02 02 12 00 00 12 00 01";
            CurrentSendMsgAboutTime =
                "00 01 00 01 00 01 00 21C00140206D0001800000FF0201010202090C07 E4 08 0C 04 10 2E 36 00 80 00 00120001";
            SelectSocketCommand = new RelayCommand<Socket>(Select);
            translator = new TcpTranslator();
//              StartListen = new RelayCommand(() => { TcpServerHelper.StartListen(); });
                  StartListen = new RelayCommand(() => { translator.StartListenNew(); });
            DisConnectServerCommand = new RelayCommand(TcpServerHelper.CloseSever);
            DisConnectClientCommand = new RelayCommand<string>(t => TcpServerHelper.DisConnectClient(t));
            SendDataToServerCommand = new RelayCommand(() =>
            {
                TcpServerHelper.SendDataToClient(CurrentSocketClient, CurrentSendMsg.StringToByte());
            });

          
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
                var result = heart.PduBytesToConstructor(bytes);
                if (result)
                {
                    heart.OverturnDestinationSource();
                    TcpServerHelper.SendDataToClient(clientSocket, heart.ToPduBytes());
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