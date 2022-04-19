using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.CosemObjects;
using MyDlmsStandard.ApplicationLay.DataNotification;
using MyDlmsStandard.Axdr;
using MyDlmsStandard.OBIS;
using MyDlmsStandard.Wrapper;
using Newtonsoft.Json;
using NLog;
using 三相智慧能源网关调试软件.Common;
using 三相智慧能源网关调试软件.Helpers;
using 三相智慧能源网关调试软件.ViewModels.DlmsViewModels;

namespace 三相智慧能源网关调试软件.ViewModels
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


        public DlmsClient DLMSClient
        {
            get => _dlmsClient;
            set
            {
                _dlmsClient = value;
                OnPropertyChanged();
            }
        }

        private DlmsClient _dlmsClient;

        /// <summary>
        /// 是否自动响应47心跳帧
        /// </summary>
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

        /// <summary>
        /// 以太网转发实例，将网关的数据中转值内部测试服务器
        /// </summary>
        public TcpTranslator Translator
        {
            get => _translator;
            set
            {
                _translator = value;
                OnPropertyChanged();
            }
        }

        private TcpTranslator _translator;

        /// <summary>
        /// 是否开启转发
        /// </summary>
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


        /// <summary>
        /// 心跳帧延时响应时间(ms)
        /// </summary>
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


        public bool IpDetectResult
        {
            get => _ipDetectResult;
            set
            {
                _ipDetectResult = value;
                OnPropertyChanged();
            }
        }

        private bool _ipDetectResult;

        public RelayCommand<string> IpDetectCommand
        {
            get => _ipDetectCommand;
            set
            {
                _ipDetectCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand<string> _ipDetectCommand;

        public class MeterIdMatchSocket : ObservableObject
        {
            public Socket MySocket
            {
                get => _MySocket;
                set
                {
                    _MySocket = value;
                    OnPropertyChanged();
                }
            }

            private Socket _MySocket;

            public string IpString
            {
                get => _ipString;
                set
                {
                    _ipString = value;
                    OnPropertyChanged();
                }
            }

            private string _ipString;


            public string MeterId
            {
                get => _meterId;
                set
                {
                    _meterId = value;
                    OnPropertyChanged();
                }
            }

            private string _meterId;


            public bool IsCheck
            {
                get => _isCheck;
                set
                {
                    _isCheck = value;
                    OnPropertyChanged();
                }
            }

            private bool _isCheck;
        }

        public ObservableCollection<MeterIdMatchSocket> MeterIdMatchSockets
        {
            get => _meterIdMatchSockets;
            set
            {
                _meterIdMatchSockets = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<MeterIdMatchSocket> _meterIdMatchSockets;

        public MeterIdMatchSocket CurrentMeterIdMatchSocket { get; set; }
        public TcpServerViewModel()
        {
            IsAutoResponseHeartBeat = true;
            HeartBeatDelayTime = 1000;
            var ip = TcpServerHelper.GetHostIp();
            TcpServerHelper = new TcpServerHelper(ip, 8881);

            TcpServerHelper.ReceiveBytes += CalcTcpServerHelper_ReceiveBytes;

            CurrentSendMsg = "00 02 00 16 00 02 00 0F 00 01 03 30 30 30 30 30 30 30 30 30 30 30 31";
            SelectSocketCommand = new RelayCommand<Socket>(Select);
            Translator = new TcpTranslator();
            StartListen = new RelayCommand(() =>
            {
                if (IsNeedTranslator)
                    Translator.StartListen();
                else
                    TcpServerHelper.StartListen();
            });
            DisConnectServerCommand = new RelayCommand(TcpServerHelper.CloseSever);
            DisConnectClientCommand = new RelayCommand<string>(t => TcpServerHelper.DisConnectClient(t));
            SendDataToServerCommand = new RelayCommand(() =>
            {
                TcpServerHelper.SendDataToClient(CurrentSocketClient, CurrentSendMsg.StringToByte());
            });
            Alarms = new ObservableCollection<AlarmViewModel>();

            IpDetectCommand = new RelayCommand<string>(t => IpDetectResult = PingIp(t));


            MeterIdMatchSockets = new ObservableCollection<MeterIdMatchSocket>();
            StrongReferenceMessenger.Default.Register<Tuple<Socket, byte[]>, string>(this, "ServerReceiveDataEvent",
                (recipient, message) =>
                {
                    var heartBeatFrame = Wrapper47FrameFactory.CreateHeartBeatFrame(message.Item2);
                    if (heartBeatFrame != null)
                    {
                        var strAdd = heartBeatFrame.GetMeterAddressString();
                        if (MeterIdMatchSockets.Count == 0)
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                MeterIdMatchSockets.Add(new MeterIdMatchSocket()
                                {
                                    MySocket = message.Item1,
                                    IpString = message.Item1.RemoteEndPoint.ToString(),
                                    MeterId = strAdd,
                                    IsCheck = false
                                });
                            });
                        }
                        else
                        {
                            var boo = false;
                            foreach (var myClass in MeterIdMatchSockets)
                            {
                                if (myClass.MeterId.Contains(strAdd))
                                {
                                    boo = false;
                                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                    {
                                        //socket链接可能会变，但表号唯一,此处进行更新最新的Socket链接
                                        myClass.IpString = message.Item1.RemoteEndPoint.ToString();
                                        myClass.MySocket = message.Item1;
                                    });
                                    break;
                                }
                                else
                                {
                                    boo = true;
                                }
                            }

                            if (boo)
                            {
                                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                {
                                    MeterIdMatchSockets.Add(new MeterIdMatchSocket()
                                    {
                                        MySocket = message.Item1,
                                        IpString = message.Item1.RemoteEndPoint.ToString(),
                                        MeterId = strAdd,
                                        IsCheck = false
                                    });
                                });
                            }
                        }
                    }
                });
        }
        public static class PushId
        {
            /// <summary>
            /// 停电上报相关PushId
            /// </summary>
            public const string Meter = "0.4.25.9.0.255";
            /// <summary>
            /// 烟感水浸的PushId
            /// </summary>
            public const string SmokeAndWater = "0.5.25.9.0.255";
            /// <summary>
            /// 风机控制的PushId
            /// </summary>
            public const string Wind = "0.6.25.9.0.255";

        }
        [Flags]
        public enum AlarmDescriptorObject2
        {
            None,
            [JsonProperty("停电")] PowerOff = 0x00000001,
            [JsonProperty("复电")] PowerOn = 0x00000004,
            [JsonProperty("过载")] OverLoad = 0x10000000,
            [JsonProperty("过流")] OverCurrent = 0x08000000,
            [JsonProperty("漏电流")] ByPass = 0x02000000,
        }
        [Flags]
        public enum SmokeAlarmDescriptor { None, 状态1 = 1, 状态2 = 2, 状态3 = 4, 状态4 = 8, 状态5 = 16, 状态6 = 32, 状态7 = 64, 状态8 = 128 }
        [Flags]
        public enum WaterAlarmDescriptor { None, 状态1 = 1, 状态2 = 2, 状态3 = 4, 状态4 = 8, 状态5 = 16, 状态6 = 32, 状态7 = 64, 状态8 = 128 }
        /// <summary>
        /// 风机控制状态描数字
        /// </summary>
        [Flags] public enum WindControlDescriptor { None, 状态1 = 1, 状态2 = 2, 状态3 = 4, 状态4 = 8, 状态5 = 16, 状态6 = 32, 状态7 = 64, 状态8 = 128 }
        /// <summary>
        /// 风机回路状态描数字
        /// </summary>
        [Flags] public enum WindLoopDescriptor { None, 状态1 = 1, 状态2 = 2, 状态3 = 4, 状态4 = 8, 状态5 = 16, 状态6 = 32, 状态7 = 64, 状态8 = 128 }
     

        public class CustomAlarm : DlmsStructure
        {
            public AxdrOctetStringFixed PushId { get; set; }
            public AxdrOctetString CosemLogicalDeviceName { get; set; }
            public AxdrIntegerUnsigned32 AlarmDescriptor1 { get; set; }
            public AxdrIntegerUnsigned32 AlarmDescriptor2 { get; set; }


            public override bool PduStringInHexConstructor(ref string pduStringInHex)
            {
                if (base.PduStringInHexConstructor(ref pduStringInHex))
                {
                    PushId = new AxdrOctetStringFixed(6);
                    var pid = Items[0].Value.ToString();
                    if (!PushId.PduStringInHexConstructor(ref pid)) return false;
                    var deviceName = Items[1].ToPduStringInHex().Substring(2);
                    CosemLogicalDeviceName = new AxdrOctetString();
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
            public string AlarmType { get; set; }

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
                var netFrame = new WrapperFrame();
                if (!netFrame.PduStringInHexConstructor(ref s)) return;

                var s1 = netFrame.WrapperBody.DataBytes.ByteToString();
                var dataNotification = new DataNotification();
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
                        var dlmsStructure = (DlmsStructure)dataNotification.NotificationBody.DataValue.Value;
                        var stringStructure = dlmsStructure.ToPduStringInHex();

                        if (alarmViewModel.CustomAlarm.PduStringInHexConstructor(ref stringStructure))
                        {
                            var pushIdObis = ObisHelper.GetObisOriginal(alarmViewModel.CustomAlarm.PushId.Value);
                            switch (pushIdObis)
                            {
                                case PushId.Meter:

                                    //停电上报相关
                                    var intvalue = alarmViewModel.CustomAlarm.AlarmDescriptor2.GetEntityValue();
                                    var ttt = (AlarmDescriptorObject2)intvalue;
                                    alarmViewModel.AlarmType = ttt.ToString();


                                    break;
                                case PushId.SmokeAndWater:

                                    var smokeValue = alarmViewModel.CustomAlarm.AlarmDescriptor1.GetEntityValue();
                                    var smoke = (SmokeAlarmDescriptor)smokeValue;
                                    alarmViewModel.AlarmType += "烟感告警：" + smoke.ToString() + "\r\n";
                                    //水浸烟感上报相关
                                    var waterValue = alarmViewModel.CustomAlarm.AlarmDescriptor2.GetEntityValue();
                                    var water = (WaterAlarmDescriptor)waterValue;
                                    alarmViewModel.AlarmType += "水浸告警" + water.ToString();

                                    break;
                                case PushId.Wind:
                                    //风机控制上报相关

                                    var WindControlValue = alarmViewModel.CustomAlarm.AlarmDescriptor1.GetEntityValue();
                                    var windControl = (WindControlDescriptor)WindControlValue;
                                    alarmViewModel.AlarmType += "风机控制:" + windControl.ToString() + "\r\n";
                                    //水浸烟感上报相关
                                    var WindLoopValue = alarmViewModel.CustomAlarm.AlarmDescriptor2.GetEntityValue();
                                    var WindLoop = (WaterAlarmDescriptor)WindLoopValue;
                                    alarmViewModel.AlarmType += "风机回路:" + WindLoop.ToString();
                                    break;
                                default:
                                    alarmViewModel.AlarmType = "Unknown";
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
                    var interrr = bytes.Take(NeedReceiveLength).ToArray();
                    NeedReceiveLength = 0;
                    _isNeedContinue = false;

                    _listReturnBytes.AddRange(interrr);
                    _returnBytes = _listReturnBytes.ToArray();
                    TcpServerHelper_ReceiveBytes(clientSocket, _returnBytes);
                    Socket_ReceiveBytes_Notify(clientSocket, _returnBytes);
                    _listReturnBytes.Clear();
                }
            }
        }


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
                var heartBeatFrame = Wrapper47FrameFactory.CreateHeartBeatFrame(bytes);

                if (heartBeatFrame != null)
                {
                    heartBeatFrame.WrapperHeader.OverturnDestinationSource();
                    await Task.Delay(HeartBeatDelayTime);
                    TcpServerHelper.SendDataToClient(clientSocket, heartBeatFrame.ToPduStringInHex().StringToByte());
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
            DLMSClient.Business.LinkLayer.CurrentSocket = clientSocket;
            CurrentSocketClient = clientSocket;
        }

        /// <summary>
        /// ping ip,测试能否ping通
        /// </summary>
        /// <param name="strIP">IP地址</param>
        /// <returns></returns>
        private bool PingIp(string strIP)
        {
            bool bRet = false;
            try
            {
                Ping pingSend = new Ping();
                PingReply reply = pingSend.Send(strIP, 1000);
                if (reply.Status == IPStatus.Success)
                    bRet = true;
            }
            catch (Exception)
            {
                bRet = false;
            }

            return bRet;
        }
    }
}