﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using NLog;
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
//            TcpServerHelper.ReceiveBytes += TcpServerHelper_ReceiveBytes;
            TcpServerHelper.ReceiveBytes += TcpServerHelper_ReceiveBytes1;

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
            SocketAndAddressCollection = new ConcurrentDictionary<Socket, string>();
        }

        private void TcpServerHelper_ReceiveBytes1(Socket arg1, byte[] arg2)
        {
            CalcTcpServerHelper_ReceiveBytes(arg1, arg2);
        }

        public enum AlarmType
        {
            None,
            PowerOff,
            ByPass,
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

            public AlarmType AlarmType { get; set; }

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
                    var alarmObject = new Alarm();
                    CosemClock cosemClock = new CosemClock();
                    cosemClock.DlmsClockParse(dataNotification.DateTime.Value.StringToByte());
                    alarmObject.AlarmClockTime = cosemClock.ToDateTime().ToString();
                    if (dataNotification.NotificationBody.DataType == DataType.Structure)
                    {
                        string value = dataNotification.NotificationBody.ValueBytes.ByteToString();
                        var dlmsStructure = new DlmsStructure();
                        if (dlmsStructure.PduStringInHexConstructor(ref value))
                        {
                            var itemstring = new List<string>();
                            foreach (var dlmsStructureItem in dlmsStructure.Items)
                            {
                                itemstring.Add(dlmsStructureItem.ValueDisplay.ValueString);
                            }

                            alarmObject.PushId = new AxdrOctetStringFixed(itemstring[0], 6);
                            alarmObject.CosemLogicalDeviceName = new AxdrOctetString(itemstring[1]);

                            alarmObject.AlarmDescriptor1 = new AxdrUnsigned32(uint.Parse(itemstring[2]).ToString("X8"));
                            alarmObject.AlarmDescriptor2 = new AxdrUnsigned32(uint.Parse(itemstring[3]).ToString("X8"));
                            alarmObject.DateTime = DateTime.Now.ToString("yy-MM-dd ddd HH:mm:ss");
                            alarmObject.IpAddress = clientSocket.RemoteEndPoint.ToString();
                            switch (alarmObject.AlarmDescriptor2.Value)
                            {
                                case "02000000":
                                    alarmObject.AlarmType = AlarmType.ByPass;
                                    break;
                                case "00000001":
                                    alarmObject.AlarmType = AlarmType.PowerOff;
                                    break;
                                default:
                                    alarmObject.AlarmType = AlarmType.None;
                                    break;
                            }

                            DispatcherHelper.CheckBeginInvokeOnUI(() => { Alarms.Add(alarmObject); });
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

        private byte[] _returnBytes;
        private readonly List<byte> _listReturnBytes = new List<byte>();
        private bool _isNeedContinue = false;
        private int TotalLength { get; set; }
        private int NeedReceiveLength { get; set; }

        private void CalcTcpServerHelper_ReceiveBytes(Socket clientSocket, byte[] bytes)
        {
            if (bytes == null)
            {
                return;
            }

            if (!_isNeedContinue)
            {
                if (bytes.Length < 7)
                {
                    Logger lgLogger = LogManager.GetCurrentClassLogger();
                    lgLogger.Debug("This Is Not 47Message Should Never Enter Here");
                    _listReturnBytes.AddRange(bytes);
                    _returnBytes = _listReturnBytes.ToArray();

                    _listReturnBytes.Clear();
                }
                else
                {
                    if (bytes[7] == (bytes.Length - 8))
                    {
                        _listReturnBytes.AddRange(bytes);
                        _returnBytes = _listReturnBytes.ToArray();
                        Socket_ReceiveBytes_Notify(clientSocket, _returnBytes);
                        TcpServerHelper_ReceiveBytes(clientSocket, _returnBytes);
                        _listReturnBytes.Clear();
                        _isNeedContinue = false;
                    }

                    if (bytes[7] > (bytes.Length - 8))
                    {
                        TotalLength = bytes[7];
                        NeedReceiveLength = TotalLength - (bytes.Length - 8);
                        _listReturnBytes.AddRange(bytes);
                        _isNeedContinue = true;
                    }
                }
            }
            else
            {
                if (bytes.Length < NeedReceiveLength)
                {
                    NeedReceiveLength -= bytes.Length;
                    _listReturnBytes.AddRange(bytes);
                }

                if (bytes.Length >= NeedReceiveLength)
                {
                    NeedReceiveLength = 0;
                    _isNeedContinue = false;

                    _listReturnBytes.AddRange(bytes);
                    _returnBytes = _listReturnBytes.ToArray();
                    TcpServerHelper_ReceiveBytes(clientSocket, _returnBytes);
                    Socket_ReceiveBytes_Notify(clientSocket, _returnBytes);
                    _listReturnBytes.Clear();
                }
            }
        }

        public IDictionary<Socket, string> SocketAndAddressCollection
        {
            get => _socketAndAddressCollection;
            set
            {
                _socketAndAddressCollection = value;
                RaisePropertyChanged();
            }
        }

        private IDictionary<Socket, string> _socketAndAddressCollection;


        /// <summary>
        /// 根据是否自动回心跳帧，判断是否为心跳帧类型，模拟主站处理心跳帧功能
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="bytes"></param>
        private async void TcpServerHelper_ReceiveBytes(Socket clientSocket, byte[] bytes)
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

                    await Task.Delay(HeartBeatDelayTime);

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