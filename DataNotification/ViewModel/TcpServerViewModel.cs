using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.CosemObjects;
using MyDlmsStandard.Axdr;
using MyDlmsStandard.Common;
using MyDlmsStandard.Wrapper;
using NLog;

namespace DataNotification.ViewModel
{
    public class TcpServerViewModel : ObservableObject
    {
        public TcpServerHelper TcpServerHelper
        {
            get => _tcpServerHelper;
            set
            {
                _tcpServerHelper = value;
                OnPropertyChanged();
            }
        }

        private TcpServerHelper _tcpServerHelper;


        public RelayCommand StartListen { get; set; }


        public RelayCommand DisConnectServerCommand { get; set; }

        public RelayCommand<string> DisConnectClientCommand { get; set; }


        public RelayCommand SendDataToServerCommand { get; set; }


        public Socket CurrentSocketClient
        {
            get => _currentSocketClient;
            set
            {
                _currentSocketClient = value;
                OnPropertyChanged();
            }
        }

        private Socket _currentSocketClient;


        public string CurrentSendMsg
        {
            get => _currentSendMsg;
            set
            {
                _currentSendMsg = value;
                OnPropertyChanged();
            }
        }

        private string _currentSendMsg;

        public RelayCommand<Socket> SelectSocketCommand { get; set; }


//        public DlmsClient DLMSClient
//        {
//            get => _dlmsClient;
//            set
//            {
//                _dlmsClient = value;
//                OnPropertyChanged();
//            }
//        }
//
//        private DlmsClient _dlmsClient;


        public bool IsAutoResponseHeartBeat
        {
            get => _isAutoResponseHeartBeat;
            set
            {
                _isAutoResponseHeartBeat = value;
                OnPropertyChanged();
            }
        }

        private bool _isAutoResponseHeartBeat;


        public bool IsNeedTranslator
        {
            get => _isNeedTranslator;
            set
            {
                _isNeedTranslator = value;
                OnPropertyChanged();
            }
        }

        private bool _isNeedTranslator;


        public int HeartBeatDelayTime
        {
            get => _heartBeatDelayTime;
            set
            {
                _heartBeatDelayTime = value;
                OnPropertyChanged();
            }
        }

        private int _heartBeatDelayTime;

        public ObservableCollection<AlarmViewModel> Alarms
        {
            get => _alarms;
            set
            {
                _alarms = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<AlarmViewModel> _alarms;


        public TcpServerViewModel()
        {
            HeartBeatDelayTime = 1000;
            var ip = TcpServerHelper.GetHostIp();
            TcpServerHelper = new TcpServerHelper(ip, 8881);
            IsAutoResponseHeartBeat = true;
//            TcpServerHelper.ReceiveBytes += TcpServerHelper_ReceiveBytes;
            TcpServerHelper.ReceiveBytes += TcpServerHelper_ReceiveBytes1;

            CurrentSendMsg = "00 02 00 16 00 02 00 0F 00 01 03 30 30 30 30 30 30 30 30 30 30 30 31";
            SelectSocketCommand = new RelayCommand<Socket>(Select);

            StartListen = new RelayCommand(() => { TcpServerHelper.StartListen(); });
            DisConnectServerCommand = new RelayCommand(TcpServerHelper.CloseSever);
            DisConnectClientCommand = new RelayCommand<string>(t => TcpServerHelper.DisConnectClient(t));
            SendDataToServerCommand = new RelayCommand(() =>
            {
                TcpServerHelper.SendDataToClient(CurrentSocketClient, CurrentSendMsg.StringToByte());
            });
            Alarms = new ObservableCollection<AlarmViewModel>();
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

        public class CustomAlarm : DlmsStructure
        {
            public AxdrOctetStringFixed PushId { get; set; }
            public AxdrIntegerOctetString CosemLogicalDeviceName { get; set; }
            public AxdrIntegerUnsigned32 AlarmDescriptor1 { get; set; }
            public AxdrIntegerUnsigned32 AlarmDescriptor2 { get; set; }


            public new bool PduStringInHexConstructor(ref string pduStringInHex)
            {
                if (base.PduStringInHexConstructor(ref pduStringInHex))
                {
                    PushId = new AxdrOctetStringFixed(6);
                    var pid = Items[0].Value.ToString();
                    if (!PushId.PduStringInHexConstructor(ref pid)) return false;
                    var deviceName = Items[1].ToPduStringInHex().Substring(2);
                    CosemLogicalDeviceName = new AxdrIntegerOctetString();
                    if (!CosemLogicalDeviceName.PduStringInHexConstructor(ref deviceName)) return false;
                    var descriptor1 = Items[2].Value.ToString();
                    AlarmDescriptor1 = new AxdrIntegerUnsigned32();
                    if (!AlarmDescriptor1.PduStringInHexConstructor(ref descriptor1)) return false;
                    var descriptor2 = Items[3].Value.ToString();
                    AlarmDescriptor2 = new AxdrIntegerUnsigned32();
                    if (!AlarmDescriptor2.PduStringInHexConstructor(ref descriptor2)) return false;

                    return true;
                }


                return false;
            }
        }

        public class AlarmViewModel : ObservableObject
        {
            public string DateTime { get; set; }
            public string IpAddress { get; set; }
            public string AlarmDateTime { get; set; }
            public AlarmType AlarmType { get; set; }

            public CustomAlarm CustomAlarm
            {
                get => _customAlarm;
                set
                {
                    _customAlarm = value;
                    OnPropertyChanged();
                }
            }


            private CustomAlarm _customAlarm;

            public AlarmViewModel()
            {
                CustomAlarm = new CustomAlarm();
            }
        }

        private void Socket_ReceiveBytes_Notify(Socket clientSocket, byte[] bytes)
        {
            try
            {
                var s = bytes.ByteToString();
                var netFrame = new NetFrame();
                if (!netFrame.PduStringInHexConstructor(ref s)) return;

                var s1 = netFrame.DLMSApduDataBytes.ByteToString();
                var dataNotification = new MyDlmsStandard.ApplicationLay.DataNotification.DataNotification();
                if (dataNotification.PduStringInHexConstructor(ref s1))
                {
                    var alarmViewModel = new AlarmViewModel()
                    {
                        DateTime = DateTime.Now.ToString("yy-MM-dd ddd HH:mm:ss"),
                        IpAddress = clientSocket.RemoteEndPoint.ToString(),
                    };

                    var cosemClock = new CosemClock();
                    cosemClock.DlmsClockParse(dataNotification.DateTime.Value.StringToByte());
                    alarmViewModel.AlarmDateTime = cosemClock.ToDateTime().ToString();

                    if (dataNotification.NotificationBody.DataValue.DataType == DataType.Structure)
                    {
                        var dlmsStructure = (DlmsStructure) dataNotification.NotificationBody.DataValue.Value;
                        var stringStructure = dlmsStructure.ToPduStringInHex();

                        if (alarmViewModel.CustomAlarm.PduStringInHexConstructor(ref stringStructure))
                        {
                            switch (alarmViewModel.CustomAlarm.AlarmDescriptor2.Value)
                            {
                                case "02000000":
                                    alarmViewModel.AlarmType = AlarmType.ByPass;
                                    break;
                                case "00000001":
                                    alarmViewModel.AlarmType = AlarmType.PowerOff;
                                    break;
                                default:
                                    alarmViewModel.AlarmType = AlarmType.None;
                                    break;
                            }

                            DispatcherHelper.CheckBeginInvokeOnUI(() => { Alarms.Add(alarmViewModel); });
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
        private bool _isNeedContinue;
        private int TotalLength { get; set; }
        private int NeedReceiveLength { get; set; }

        private void CalcTcpServerHelper_ReceiveBytes(Socket clientSocket, byte[] bytes)
        {
            if (bytes == null) return;

            if (!_isNeedContinue)
            {
                if (bytes.Length < 7)
                {
                    var lgLogger = LogManager.GetCurrentClassLogger();
                    lgLogger.Debug("This Is Not 47Message Should Never Enter Here");
                    _listReturnBytes.AddRange(bytes);
                    _returnBytes = _listReturnBytes.ToArray();

                    _listReturnBytes.Clear();
                }
                else
                {
                    if (bytes[7] == bytes.Length - 8)
                    {
                        _listReturnBytes.AddRange(bytes);
                        _returnBytes = _listReturnBytes.ToArray();
                        Socket_ReceiveBytes_Notify(clientSocket, _returnBytes);
                        TcpServerHelper_ReceiveBytes(clientSocket, _returnBytes);
                        _listReturnBytes.Clear();
                        _isNeedContinue = false;
                    }

                    if (bytes[7] > bytes.Length - 8)
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
                OnPropertyChanged();
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
            if (!IsAutoResponseHeartBeat) return;

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
//            DLMSClient.CurrentSocket = clientSocket;
            CurrentSocketClient = clientSocket;
        }
    }
}