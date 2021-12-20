using DotNetty.Transport.Channels;
using Prism.Mvvm;


namespace JobMaster.ViewModels
{
    public class MeterIdMatchSocketNew : BindableBase
    {
        public IChannelHandlerContext MySocket
        {
            get => _MySocket;
            set
            {
                _MySocket = value;
                RaisePropertyChanged();
            }
        }

        private IChannelHandlerContext _MySocket;

        public string IpString
        {
            get => _ipString;
            set
            {
                _ipString = value;
                RaisePropertyChanged();
            }
        }

        private string _ipString;

        public string MeterId
        {
            get => _meterId;
            set
            {
                _meterId = value;
                RaisePropertyChanged();
            }
        }

        private string _meterId;

        public bool IsCheck
        {
            get => _isCheck;
            set
            {
                _isCheck = value;
                RaisePropertyChanged();
            }
        }

        private bool _isCheck;
    }


}