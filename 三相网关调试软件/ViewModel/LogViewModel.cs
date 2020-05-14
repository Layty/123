using System;
using System.Text;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using 三相智慧能源网关调试软件.Model;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class LogViewModel : ViewModelBase
    {
        public LogViewModel()
        {
            if (IsInDesignMode)
            {
                MyNetLog = new MyNetLogModel {Log = "1231222222222222222222222222222222222222223"};
            }
            else
            {
                MyNetLog = new MyNetLogModel();
                ClearBufferCommand = new RelayCommand(() => { MyNetLog.ClearBuffer(); });
                Messenger.Default.Register<byte[]>(this, "SendDataEvent",
                    sendData => { MyNetLog.Log = DateTime.Now + "=>" + Encoding.Default.GetString(sendData); });
                Messenger.Default.Register<byte[]>(this, "ReceiveDataEvent",
                    receiveData => { MyNetLog.Log = DateTime.Now + "<=" + Encoding.Default.GetString(receiveData); });
                Messenger.Default.Register<string>(this, "ENetErrorEvent",
                    errorMessage => { MyNetLog.Log = DateTime.Now + "ErrorEvent" + errorMessage + Environment.NewLine; });
                Messenger.Default.Register<string>(this, "Status",
                    status => { MyNetLog.Log = DateTime.Now +"Status" + status + Environment.NewLine; }
                );
            }
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