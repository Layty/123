using System;
using System.Net.Sockets;
using GalaSoft.MvvmLight.Messaging;
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


        public RelayCommand ClearServerBufferCommand
        {
            get => _clearServerBufferCommand;
            set
            {
                _clearServerBufferCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _clearServerBufferCommand;

        public RelayCommand ClearClientBufferCommand
        {
            get => _clearClientBufferCommand;
            set
            {
                _clearClientBufferCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _clearClientBufferCommand;

        public NetLogViewModel()
        {
            MyServerNetLogModel = new MyNetLogModel();


            MyClientNetLogModel = new MyNetLogModel();
            ClearServerBufferCommand = new RelayCommand(() => { MyServerNetLogModel.ClearBuffer(); });
            ClearClientBufferCommand = new RelayCommand(() => { MyClientNetLogModel.ClearBuffer(); });

            Messenger.Default.Register<(Socket, byte[])>(this, "ClientReceiveDataEvent",
                (s) => { MyClientNetLogModel.HandlerReceiveData(s.Item1, s.Item2); });
            Messenger.Default.Register<(Socket, byte[])>(this, "ClientSendDataEvent",
                (s) => { MyClientNetLogModel.HandlerSendData(s.Item1, s.Item2); });
            Messenger.Default.Register<string>(this, "ClientStatus",
                status => { MyClientNetLogModel.Log = DateTime.Now + "ClientStatus" + status + Environment.NewLine; });
            Messenger.Default.Register<string>(this, "ClientErrorEvent",
                errorMessage =>
                {
                    MyClientNetLogModel.Log = DateTime.Now + "ClientErrorEvent" + errorMessage + Environment.NewLine;
                });

     

            Messenger.Default.Register<string>(this, "ServerStatus",
                status => { MyServerNetLogModel.Log = DateTime.Now + "ServerStatus" + status + Environment.NewLine; });
            Messenger.Default.Register<(Socket, byte[])>(this, "ServerReceiveDataEvent",
                (s) => { MyServerNetLogModel.HandlerReceiveData(s.Item1, s.Item2); });
            Messenger.Default.Register<(Socket, byte[])>(this, "ServerSendDataEvent",
                (s) => { MyServerNetLogModel.HandlerSendData(s.Item1, s.Item2); });
            Messenger.Default.Register<string>(this, "ServerErrorEvent",
                (errorString) =>
                {
                    MyServerNetLogModel.Log = DateTime.Now + "ServerErrorEvent" + errorString +
                                              Environment.NewLine;
                });
        }
    }
}