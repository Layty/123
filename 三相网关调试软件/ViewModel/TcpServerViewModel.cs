using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using NLog;
using 三相智慧能源网关调试软件.Common;
using 三相智慧能源网关调试软件.ViewModel.DlmsViewModels;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using MyDlmsStandard;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.CosemObjects;
using MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage;
using MyDlmsStandard.ApplicationLay.DataNotification;
using MyDlmsStandard.ApplicationLay.Get;
using MyDlmsStandard.Axdr;
using MyDlmsStandard.Wrapper;
using Quartz;
using Quartz.Impl;
using 三相智慧能源网关调试软件.Model;

namespace 三相智慧能源网关调试软件.ViewModel
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

        public RelayCommand StartTaskCommand { get; set; }
        public RelayCommand StopTaskCommand { get; set; }
        public JobCenter JobCenter { get; set; }


        public class MyClass : ObservableObject
        {
            public string IpString
            {
                get => _IpString;
                set
                {
                    _IpString = value;
                    OnPropertyChanged();
                }
            }

            private string _IpString;


            public string MeterAddress
            {
                get => _MeterAddress;
                set
                {
                    _MeterAddress = value;
                    OnPropertyChanged();
                }
            }

            private string _MeterAddress;


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

        public ObservableCollection<MyClass> ListBoxExtend
        {
            get => _ListBoxExtend;
            set
            {
                _ListBoxExtend = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<MyClass> _ListBoxExtend;


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
            SocketAndAddressCollection = new ConcurrentDictionary<Socket, string>();
            IpDetectCommand = new RelayCommand<string>(t => IpDetectResult = PingIp(t));


//            JobCenter = ServiceLocator.Current.GetInstance<JobCenter>();
            JobCenter = new JobCenter();
            StartTaskCommand = new RelayCommand(() => { JobCenter.Start(); });
            StopTaskCommand = new RelayCommand(() => { JobCenter.Shutdown(); });
            ListBoxExtend = new ObservableCollection<MyClass>();
            StrongReferenceMessenger.Default.Register<Tuple<Socket, byte[]>, string>(this, "ServerReceiveDataEvent",
                (recipient, message) =>
                {
                    HeartBeatFrame heartBeatFrame = new HeartBeatFrame();
                    var str = message.Item2.ByteToString();
                    if (heartBeatFrame.PduStringInHexConstructor(ref str))
                    {
                        var strAdd = Encoding.Default.GetString(heartBeatFrame.MeterAddressBytes);
                        if (ListBoxExtend.Count == 0)
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                ListBoxExtend.Add(new MyClass()
                                {
                                    IpString = message.Item1.RemoteEndPoint.ToString(),
                                    MeterAddress = strAdd,
                                    IsCheck = false
                                });
                            });
                        }
                        else
                        {
                            var boo = false;
                            for (int i = 0; i < ListBoxExtend.Count; i++)
                            {
                                if (ListBoxExtend[i].MeterAddress.Contains(strAdd))
                                {
                                    boo = false;
                                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                    {
                                        //socket链接可能会变，但表号唯一,此处进行更新最新的Socket链接
                                        ListBoxExtend[i].IpString = message.Item1.RemoteEndPoint.ToString();
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
                                    ListBoxExtend.Add(new MyClass()
                                    {
                                        IpString = message.Item1.RemoteEndPoint.ToString(),
                                        MeterAddress = strAdd,
                                        IsCheck = false
                                    });
                                });
                            }
                        }
                    }
                });
        }


        public enum AlarmType
        {
            Unknown,
            PowerOff,
            ByPass,
            烟感and水浸,
            风机控制
        }

        public class CustomAlarm : DlmsStructure
        {
            public AxdrOctetStringFixed PushId { get; set; }
            public AxdrOctetString CosemLogicalDeviceName { get; set; }
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
                var netFrame = new WrapperFrame();
                if (!netFrame.PduStringInHexConstructor(ref s)) return;

                var s1 = netFrame.WrapperData.ByteToString();
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
                        var dlmsStructure = (DlmsStructure) dataNotification.NotificationBody.DataValue.Value;
                        var stringStructure = dlmsStructure.ToPduStringInHex();

                        if (alarmViewModel.CustomAlarm.PduStringInHexConstructor(ref stringStructure))
                        {
                            switch (alarmViewModel.CustomAlarm.PushId.Value)
                            {
                                case "0004190900FF":
                                    //停电上报相关
                                    switch (alarmViewModel.CustomAlarm.AlarmDescriptor2.Value)
                                    {
                                        case "02000000":
                                            alarmViewModel.AlarmType = AlarmType.ByPass;
                                            break;
                                        case "00000001":
                                            alarmViewModel.AlarmType = AlarmType.PowerOff;
                                            break;
                                        default:
                                            alarmViewModel.AlarmType = AlarmType.Unknown;
                                            break;
                                    }

                                    break;
                                case "0005190900FF":
                                    //水浸烟感上报相关
                                    alarmViewModel.AlarmType = AlarmType.烟感and水浸;
                                    break;
                                case "06190900FF":
                                    //风机控制上报相关
                                    alarmViewModel.AlarmType = AlarmType.风机控制;
                                    break;
                                default:
                                    alarmViewModel.AlarmType = AlarmType.Unknown;
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
                var pduString = bytes.ByteToString();
                var result = heart.PduStringInHexConstructor(ref pduString);
                if (result)
                {
                    heart.OverturnDestinationSource();
                    await Task.Delay(HeartBeatDelayTime);
                    TcpServerHelper.SendDataToClient(clientSocket, heart.ToPduStringInHex().StringToByte());
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