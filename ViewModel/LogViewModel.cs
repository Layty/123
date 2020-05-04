using System;
using System.Text;
using GalaSoft.MvvmLight;
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
                MyLog = new MyLogModel {CommandLog = ("1231222222222222222222222222222222222222223")};
            }
            else
            {
                MyLog = new MyLogModel();
                Messenger.Default.Register<byte[]>(this, "SendDataEvent", ENetClientHelper_SendData);
                Messenger.Default.Register<byte[]>(this, "ReceiveDataEvent", ENetClientHelper_ReceiveData);
                Messenger.Default.Register<string>(this, "ENetErrorEvent", (p => { MyLog.CommandLog += p; }));
                Messenger.Default.Register<string>(this, "Status",
                    (
                        p => { MyLog.CommandLog += DateTime.Now + p + Environment.NewLine; }
                    ));
            }
        }


        private void ENetClientHelper_ReceiveData(byte[] bytes)
        {
            MyLog.CommandLog += (DateTime.Now + "<=" + Encoding.Default.GetString(bytes) + Environment.NewLine);
        }

        private void ENetClientHelper_SendData(byte[] bytes)
        {
            MyLog.CommandLog += (DateTime.Now + "=>" + Encoding.Default.GetString(bytes) + Environment.NewLine);
        }

        private MyLogModel _myLog;

        public MyLogModel MyLog
        {
            get => _myLog;
            set
            {
                _myLog = value;
                RaisePropertyChanged();
            }
        }
    }
}