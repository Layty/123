using System;
using System.Net.Sockets;
using System.Text;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using 三相智慧能源网关调试软件.Model;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class NetLogViewModel : ViewModelBase
    {
        public NetLogViewModel()
        {
            MyNetLog = new MyNetLogModel();
            ClearBufferCommand = new RelayCommand(() => { MyNetLog.ClearBuffer(); });
            Messenger.Default.Register<byte[]>(this, "SendDataEvent",
                sendData => { MyNetLog.Log = DateTime.Now + "=>" + Encoding.Default.GetString(sendData); });
            Messenger.Default.Register<byte[]>(this, "ReceiveDataEvent",
                receiveData => { MyNetLog.Log = DateTime.Now + "<=" + Encoding.Default.GetString(receiveData); });
            Messenger.Default.Register<string>(this, "ENetErrorEvent",
                errorMessage => { MyNetLog.Log = DateTime.Now + "ErrorEvent" + errorMessage + Environment.NewLine; });
            Messenger.Default.Register<string>(this, "TelNetErrorEvent",
                errorMessage => { MyNetLog.Log = DateTime.Now + "ErrorEvent" + errorMessage + Environment.NewLine; });
            Messenger.Default.Register<string>(this, "Status",
                status => { MyNetLog.Log = DateTime.Now + "Status" + status + Environment.NewLine; });
            Messenger.Default.Register<(Socket, byte[])>(this, "ReceiveDataEvent",
                (s) =>
                {
                    MyNetLog.HandlerReceiveData(s.Item1, s.Item2);
                    //MyNetLog.Log = DateTime.Now + "ReceiveDataEvent" + "收到" + s.Item1.RemoteEndPoint+"数据" + Encoding.Default.GetString(s.Item2) +
                    //               Environment.NewLine;
                });
            Messenger.Default.Register<(Socket, byte[])>(this, "SendDataEvent",
                (s) =>
                {
                    MyNetLog.HandlerSendData(s.Item1,s.Item2);
                    //MyNetLog.Log = DateTime.Now + "SendDataEvent" + "向" + s.Item1.RemoteEndPoint + "发送数据" +
                    //               Encoding.Default.GetString(s.Item2) +
                    //               Environment.NewLine;
                });
            Messenger.Default.Register<string>(this, "ErrorEvent",
                (errorString) =>
                {
                    MyNetLog.Log = DateTime.Now + "ErrorEvent" + errorString +
                                   Environment.NewLine;
                });
        }


        private MyNetLogModel _myNetLog;

        public MyNetLogModel MyNetLog
        {
            get => _myNetLog;
            set
            {
                _myNetLog = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand ClearBufferCommand
        {
            get => _clearBufferCommand;
            set
            {
                _clearBufferCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _clearBufferCommand;
    }
}