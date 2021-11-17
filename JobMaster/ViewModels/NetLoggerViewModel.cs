using JobMaster.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;

namespace JobMaster.ViewModels
{
    public class NetLoggerViewModel : BindableBase
    {
        private MyNetLogModel _myServerNetLogModel;

        public MyNetLogModel MyServerNetLogModel
        {
            get => _myServerNetLogModel;
            set
            {
                _myServerNetLogModel = value;
              RaisePropertyChanged();
            }
        }


        public MyNetLogModel MyClientNetLogModel
        {
            get => _myClientNetLogModel;
            set
            {
                _myClientNetLogModel = value;
                RaisePropertyChanged();
            }
        }

        private MyNetLogModel _myClientNetLogModel;

      
        public DelegateCommand ClearServerBufferCommand { get; set; }

        public DelegateCommand ClearClientBufferCommand { get; set; }

        public NetLoggerViewModel(FrontEndProcessorViewModel frontEndProcessorViewModel)
        {
            MyServerNetLogModel = new MyNetLogModel();
            frontEndProcessorViewModel.TcpServerHelper.SendBytesToClient += TcpServerHelper_SendBytesToClient; ;
            frontEndProcessorViewModel.TcpServerHelper.ReceiveBytes += TcpServerHelper_ReceiveBytes;
            frontEndProcessorViewModel.TcpServerHelper.AcceptNewClient += TcpServerHelper_AcceptNewClient;
            frontEndProcessorViewModel.TcpServerHelper.ErrorMsg += TcpServerHelper_ErrorMsg;
            frontEndProcessorViewModel.TcpServerHelper.StatusMsg += TcpServerHelper_StatusMsg;
            MyClientNetLogModel = new MyNetLogModel();
            ClearServerBufferCommand = new DelegateCommand(() => { MyServerNetLogModel.ClearBuffer(); });
            ClearClientBufferCommand = new DelegateCommand(() => { MyClientNetLogModel.ClearBuffer(); });

            //StrongReferenceMessenger.Default.Register<Tuple<Socket, byte[]>, string>(this, "ClientReceiveDataEvent",
            //    (sender, args) => { MyClientNetLogModel.HandlerReceiveData(args.Item1, args.Item2); });
            //StrongReferenceMessenger.Default.Register<Tuple<Socket, byte[]>, string>(this, "ClientSendDataEvent",
            //    (sender, args) => { MyClientNetLogModel.HandlerSendData(args.Item1, args.Item2); });
            //StrongReferenceMessenger.Default.Register<string, string>(this, "ClientStatus",
            //    (sender, status) => { MyClientNetLogModel.Log = DateTime.Now + "ClientStatus" + status + Environment.NewLine; });
            //StrongReferenceMessenger.Default.Register<string, string>(this, "ClientErrorEvent",
            //    (sender, errorMessage) =>
            //    {
            //        MyClientNetLogModel.Log = DateTime.Now + "ClientErrorEvent" + errorMessage + Environment.NewLine;
            //    });


            //StrongReferenceMessenger.Default.Register<string, string>(this, "ServerStatus",
            //    (sender, status) => { MyServerNetLogModel.Log = DateTime.Now + "ServerStatus" + status + Environment.NewLine; });
            //StrongReferenceMessenger.Default.Register<Tuple<Socket, byte[]>, string>(this, "ServerReceiveDataEvent",
            //    (sender, s) => { MyServerNetLogModel.HandlerReceiveData(s.Item1, s.Item2); });
            //StrongReferenceMessenger.Default.Register<Tuple<Socket, byte[]>, string>(this, "ServerSendDataEvent",
            //    (sender, s) => { MyServerNetLogModel.HandlerSendData(s.Item1, s.Item2); });
            //StrongReferenceMessenger.Default.Register<string, string>(this, "ServerErrorEvent",
            //    (sender, errorString) =>
            //    {
            //        MyServerNetLogModel.Log = DateTime.Now + "ServerErrorEvent" + errorString +
            //                                  Environment.NewLine;
            //    });
        }

        private void TcpServerHelper_StatusMsg(string status)
        {
            MyServerNetLogModel.Log = DateTime.Now + "ServerStatus" + status + Environment.NewLine;
        }

        private void TcpServerHelper_ErrorMsg(string errorString)
        {
            MyServerNetLogModel.Log = DateTime.Now + "ServerErrorEvent" + errorString +
                                          Environment.NewLine;
        }

        private void TcpServerHelper_AcceptNewClient(System.Net.Sockets.Socket obj)
        {
            MyServerNetLogModel.Log = DateTime.Now + "AcceptNewClient" + obj.RemoteEndPoint + Environment.NewLine;
        }

        private void TcpServerHelper_ReceiveBytes(System.Net.Sockets.Socket arg1, byte[] arg2)
        {
            MyServerNetLogModel.HandlerReceiveData(arg1, arg2);
        }

        private void TcpServerHelper_SendBytesToClient(System.Net.Sockets.Socket arg1, byte[] arg2)
        {
            MyServerNetLogModel.HandlerSendData(arg1, arg2);
        }
    }
}