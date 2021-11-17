using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace JobMaster.ViewModels
{

    public class FrontEndProcessorViewModel : BindableBase
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

        public DelegateCommand StartListen { get; set; }

        public DelegateCommand CloseServer { get; set; }

        public DelegateCommand<string> DisConnectClientCommand { get; set; }


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

        public DelegateCommand<Socket> SelectSocketCommand { get; set; }


        public DlmsClient DLMSClient
        {
            get => _dlmsClient;
            set
            {
                _dlmsClient = value;
                RaisePropertyChanged();
            }
        }

        private DlmsClient _dlmsClient;

        public bool IpDetectResult
        {
            get => _ipDetectResult;
            set
            {
                _ipDetectResult = value;
                RaisePropertyChanged();
            }
        }

        private bool _ipDetectResult;

        public DelegateCommand<string> IpDetectCommand
        {
            get => _ipDetectCommand;
            set
            {
                _ipDetectCommand = value;
                RaisePropertyChanged();
            }
        }

        private DelegateCommand<string> _ipDetectCommand;

        public FrontEndProcessorViewModel()
        {
            var ip = TcpServerHelper.GetHostIp();
            TcpServerHelper = new TcpServerHelper(ip, 8881);
            StartListen = new DelegateCommand(() =>
            {
                TcpServerHelper.StartListen();
            });
            CloseServer = new DelegateCommand(TcpServerHelper.CloseSever);
            DisConnectClientCommand = new DelegateCommand<string>(t => TcpServerHelper.DisConnectClient(t));
            IpDetectCommand = new DelegateCommand<string>(t => IpDetectResult = PingIp(t));

        }

        //private byte[] _returnBytes;
        //private readonly List<byte> _listReturnBytes = new List<byte>();
        //private bool _isNeedContinue;
        //private int TotalLength { get; set; }
        //private int NeedReceiveLength { get; set; }

        //private void CalcTcpServerHelper_ReceiveBytes(Socket clientSocket, byte[] bytes)
        //{
        //    if (bytes == null) return;

        //    if (!_isNeedContinue)
        //    {
        //        if (bytes.Length < 7)
        //        {
        //            var lgLogger = LogManager.GetCurrentClassLogger();
        //            lgLogger.Debug("This Is Not 47Message Should Never Enter Here");
        //            _listReturnBytes.AddRange(bytes);
        //            _returnBytes = _listReturnBytes.ToArray();

        //            _listReturnBytes.Clear();
        //        }
        //        else
        //        {
        //            if (bytes[7] == bytes.Length - 8)
        //            {
        //                _listReturnBytes.AddRange(bytes);
        //                _returnBytes = _listReturnBytes.ToArray();
        //                // Socket_ReceiveBytes_Notify(clientSocket, _returnBytes);
        //                TcpServerHelper_ReceiveBytesHandleHeartBeat(clientSocket, _returnBytes);
        //                _listReturnBytes.Clear();
        //                _isNeedContinue = false;
        //            }

        //            if (bytes[7] > bytes.Length - 8)
        //            {
        //                TotalLength = bytes[7];
        //                NeedReceiveLength = TotalLength - (bytes.Length - 8);
        //                _listReturnBytes.AddRange(bytes);
        //                _isNeedContinue = true;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (bytes.Length < NeedReceiveLength)
        //        {
        //            NeedReceiveLength -= bytes.Length;
        //            _listReturnBytes.AddRange(bytes);
        //        }

        //        if (bytes.Length >= NeedReceiveLength)
        //        {
        //            NeedReceiveLength = 0;
        //            _isNeedContinue = false;

        //            _listReturnBytes.AddRange(bytes);
        //            _returnBytes = _listReturnBytes.ToArray();
        //            TcpServerHelper_ReceiveBytesHandleHeartBeat(clientSocket, _returnBytes);
        //            //  Socket_ReceiveBytes_Notify(clientSocket, _returnBytes);
        //            _listReturnBytes.Clear();
        //        }
        //    }
        //}


        ///// <summary>
        ///// 根据是否自动回心跳帧，判断是否为心跳帧类型，模拟主站处理心跳帧功能
        ///// </summary>
        ///// <param name="clientSocket"></param>
        ///// <param name="bytes"></param>
        //private async void TcpServerHelper_ReceiveBytesHandleHeartBeat(Socket clientSocket, byte[] bytes)
        //{
        //    if (!IsAutoResponseHeartBeat) return;

        //    try
        //    {
        //        var heartBeatFrame = Wrapper47FrameFactory.CreateHeartBeatFrame(bytes);

        //        if (heartBeatFrame != null)
        //        {
        //            heartBeatFrame.WrapperHeader.OverturnDestinationSource();
        //            await Task.Delay(HeartBeatDelayTime);
        //            TcpServerHelper.SendDataToClient(clientSocket, heartBeatFrame.ToPduStringInHex().StringToByte());
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //    }
        //}

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