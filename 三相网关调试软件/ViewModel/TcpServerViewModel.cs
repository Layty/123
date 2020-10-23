using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.DataNotification;
using 三相智慧能源网关调试软件.DLMS.Axdr;
using 三相智慧能源网关调试软件.DLMS.Wrapper;
using 三相智慧能源网关调试软件.Properties;
using 三相智慧能源网关调试软件.ViewModel.DlmsViewModels;

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

        public TcpTranslator Translator
        {
            get => translator;
            set
            {
                translator = value;
                RaisePropertyChanged();
            }
        }

        private TcpTranslator translator;

        public bool IsNeedTranslator
        {
            get => _isNeedTranslator;
            set
            {
                _isNeedTranslator = value;
                RaisePropertyChanged();
            }
        }

        private bool _isNeedTranslator;


        public int HeartBeatDelayTime
        {
            get => _heartBeatDelayTime;
            set
            {
                _heartBeatDelayTime = value;
                RaisePropertyChanged();
            }
        }

        private int _heartBeatDelayTime;

        public ObservableCollection<Alarm> Alarms
        {
            get => _alarms;
            set
            {
                _alarms = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<Alarm> _alarms;


        public TcpServerViewModel()
        {
            HeartBeatDelayTime = 1000;
            TcpServerHelper = new TcpServerHelper(Settings.Default.GatewayIpAddress, 8881);
            IsAutoResponseHeartBeat = true;
            TcpServerHelper.ReceiveBytes += TcpServerHelper_ReceiveBytes;
            TcpServerHelper.ReceiveBytes += Socket_ReceiveBytes_Notify;
            CurrentSendMsg = "00 02 00 16 00 02 00 0F 00 01 03 30 30 30 30 30 30 30 30 30 30 30 31";
            SelectSocketCommand = new RelayCommand<Socket>(Select);
            Translator = new TcpTranslator();
            StartListen = new RelayCommand(() =>
            {
                if (IsNeedTranslator)
                {
                    translator.StartListen();
                }
                else
                {
                    TcpServerHelper.StartListen();
                }
            });
            DisConnectServerCommand = new RelayCommand(TcpServerHelper.CloseSever);
            DisConnectClientCommand = new RelayCommand<string>(t => TcpServerHelper.DisConnectClient(t));
            SendDataToServerCommand = new RelayCommand(() =>
            {
                TcpServerHelper.SendDataToClient(CurrentSocketClient, CurrentSendMsg.StringToByte());
            });
            Alarms = new ObservableCollection<Alarm>();
            SocketAndAddressCollection=new ConcurrentDictionary<Socket, string>();
        }

        public class Alarm : IPduStringInHexConstructor
        {
            public string DateTime { get; set; }
            public string IpAddress { get; set; }
            public string AlarmClockTime { get; set; }
            public AxdrOctetStringFixed PushId { get; set; }

            public AxdrOctetString CosemLogicalDeviceName { get; set; }
            public AxdrUnsigned32 AlarmDescriptor1 { get; set; }
            public AxdrUnsigned32 AlarmDescriptor2 { get; set; }


            public bool PduStringInHexConstructor(ref string pduStringInHex)
            {
                PushId = new AxdrOctetStringFixed(6);
                if (!PushId.PduStringInHexConstructor(ref pduStringInHex))
                {
                    return false;
                }

                CosemLogicalDeviceName = new AxdrOctetString();
                if (!CosemLogicalDeviceName.PduStringInHexConstructor(ref pduStringInHex))
                {
                    return false;
                }

                AlarmDescriptor1 = new AxdrUnsigned32();
                if (!AlarmDescriptor1.PduStringInHexConstructor(ref pduStringInHex))
                {
                    return false;
                }

                AlarmDescriptor2 = new AxdrUnsigned32();
                if (!AlarmDescriptor2.PduStringInHexConstructor(ref pduStringInHex))
                {
                    return false;
                }

                return true;
            }
        }

        private void Socket_ReceiveBytes_Notify(Socket clientSocket, byte[] bytes)
        {
            try
            {
                var s = bytes.ByteToString();
                NetFrame netFrame = new NetFrame();
                if (!netFrame.PduStringInHexConstructor(ref s))
                {
                    return;
                }

                var s1 = netFrame.DLMSApduDataBytes.ByteToString();
                DataNotification dataNotification = new DataNotification();
                if (dataNotification.PduStringInHexConstructor(ref s1))
                {
                    var AlarmObject = new Alarm();
                    CosemClock cosemClock = new CosemClock();
                    cosemClock.DlmsClockParse(dataNotification.DateTime.Value.StringToByte());
                    AlarmObject.AlarmClockTime = cosemClock.ToDateTime().ToString();
                    if (dataNotification.NotificationBody.DataType == DataType.Structure)
                    {
                        string value = dataNotification.NotificationBody.ValueBytes.ByteToString();
                        var dlmsStructure = new DlmsStructure();
                        if (dlmsStructure.PduStringInHexConstructor(ref value))
                        {
                            var itemstring = new List<string> { };
                            foreach (var dlmsStructureItem in dlmsStructure.Items)
                            {
                                itemstring.Add(dlmsStructureItem.ValueDisplay.ValueString);
                            }

                            AlarmObject.PushId = new AxdrOctetStringFixed(itemstring[0], 6);
                            AlarmObject.CosemLogicalDeviceName = new AxdrOctetString(itemstring[1]);

                            AlarmObject.AlarmDescriptor1 = new AxdrUnsigned32(uint.Parse(itemstring[2]).ToString("X8"));
                            AlarmObject.AlarmDescriptor2 = new AxdrUnsigned32(uint.Parse(itemstring[3]).ToString("X8"));
                            AlarmObject.DateTime = DateTime.Now.ToString("yy-MM-dd ddd HH:mm:ss");
                            AlarmObject.IpAddress = clientSocket.RemoteEndPoint.ToString();
                            DispatcherHelper.CheckBeginInvokeOnUI(() => { Alarms.Add(AlarmObject); });
                        }
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
//                throw;
            }
        }



        public IDictionary<Socket,string> SocketAndAddressCollection    
        {
            get => _socketAndAddressCollection;
            set { _socketAndAddressCollection = value; RaisePropertyChanged(); }
        }
        private IDictionary<Socket, string> _socketAndAddressCollection;


        /// <summary>
        /// 根据是否自动回心跳帧，判断是否为心跳帧类型，模拟主站处理心跳帧功能
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="bytes"></param>
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
                    Thread.Sleep(HeartBeatDelayTime);
//                   var stringaddr= Encoding.Default.GetString(heart.MeterAddressBytes);
//                    if (!SocketAndAddressCollection.ContainsKey(clientSocket))
//                    {
//                        SocketAndAddressCollection[clientSocket] = stringaddr;
//                    }
                   
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