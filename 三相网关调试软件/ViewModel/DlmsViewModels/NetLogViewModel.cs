using System;
using System.Net.Sockets;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using 三相智慧能源网关调试软件.Model;

namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{
    public class NetLogViewModel : ObservableObject
    {
        private MyNetLogModel _myServerNetLogModel;

        public MyNetLogModel MyServerNetLogModel
        {
            get => _myServerNetLogModel;
            set
            {
                _myServerNetLogModel = value;
                OnPropertyChanged();
            }
        }


        public MyNetLogModel MyClientNetLogModel
        {
            get => _myClientNetLogModel;
            set
            {
                _myClientNetLogModel = value;
                OnPropertyChanged();
            }
        }

        private MyNetLogModel _myClientNetLogModel;


        public RelayCommand ClearServerBufferCommand { get; set; }

        public RelayCommand ClearClientBufferCommand { get; set; }

        public NetLogViewModel()
        {
            MyServerNetLogModel = new MyNetLogModel();


            MyClientNetLogModel = new MyNetLogModel();
            ClearServerBufferCommand = new RelayCommand(() => { MyServerNetLogModel.ClearBuffer(); });
            ClearClientBufferCommand = new RelayCommand(() => { MyClientNetLogModel.ClearBuffer(); });

            StrongReferenceMessenger.Default.Register<Tuple<Socket, byte[]>,string>(this, "ClientReceiveDataEvent",
                (sender,args) => { MyClientNetLogModel.HandlerReceiveData(args.Item1, args.Item2); });
            StrongReferenceMessenger.Default.Register<Tuple<Socket, byte[]>, string>(this, "ClientSendDataEvent",
                (sender, args) => { MyClientNetLogModel.HandlerSendData(args.Item1, args.Item2); });
            StrongReferenceMessenger.Default.Register<string,string>(this, "ClientStatus",
                (sender,status) => { MyClientNetLogModel.Log = DateTime.Now + "ClientStatus" + status + Environment.NewLine; });
            StrongReferenceMessenger.Default.Register<string, string>(this, "ClientErrorEvent",
                (sender,errorMessage) =>
                {
                    MyClientNetLogModel.Log = DateTime.Now + "ClientErrorEvent" + errorMessage + Environment.NewLine;
                });


            StrongReferenceMessenger.Default.Register<string,string>(this, "ServerStatus",
                (sender,status) => { MyServerNetLogModel.Log = DateTime.Now + "ServerStatus" + status + Environment.NewLine; });
            StrongReferenceMessenger.Default.Register<Tuple<Socket, byte[]>,string>(this, "ServerReceiveDataEvent",
                (sender, s) => { MyServerNetLogModel.HandlerReceiveData(s.Item1, s.Item2); });
            StrongReferenceMessenger.Default.Register<Tuple<Socket, byte[]>, string>(this, "ServerSendDataEvent",
                (sender, s) => { MyServerNetLogModel.HandlerSendData(s.Item1, s.Item2); });
            StrongReferenceMessenger.Default.Register<string,string>(this, "ServerErrorEvent",
                (sender,errorString) =>
                {
                    MyServerNetLogModel.Log = DateTime.Now + "ServerErrorEvent" + errorString +
                                              Environment.NewLine;
                });
        }
    }
}