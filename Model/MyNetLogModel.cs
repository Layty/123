using System.Text;
using GalaSoft.MvvmLight;

namespace 三相智慧能源网关调试软件.Model
{
    public class MyNetLogModel : ObservableObject
    {
        public StringBuilder NetLogStringBuilder = new StringBuilder();

        public string Log
        {
            get => NetLogStringBuilder.ToString();

            set
            {
                if (NetLogStringBuilder.Length > _keepMaxSendAndReceiveDataLength)
                {
                    NetLogStringBuilder.Clear();
                }

                NetLogStringBuilder.Append(value);
                RaisePropertyChanged();
            }
        }

        public int KeepMaxSendAndReceiveDataLength
        {
            get => _keepMaxSendAndReceiveDataLength;
            set
            {
                _keepMaxSendAndReceiveDataLength = value;
                RaisePropertyChanged();
            }
        }

        private int _keepMaxSendAndReceiveDataLength = 5000;


        public void ClearBuffer()
        {
            NetLogStringBuilder.Clear();
            Log = string.Empty;
        }
    }
}