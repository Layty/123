using Microsoft.Toolkit.Mvvm.ComponentModel;
using 三相智慧能源网关调试软件.DLMS.Axdr;
using 三相智慧能源网关调试软件.DLMS.Wrapper;

namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{
    public class NetFrameMaker : ObservableObject
    {
        private DlmsSettingsViewModel DlmsSettingsViewModel { get; }

        public NetFrame NetFrame
        {
            get => _netFrame;
            set
            {
                _netFrame = value;
                OnPropertyChanged();
            }
        }

        private NetFrame _netFrame;


        public NetFrameMaker(DlmsSettingsViewModel settingsViewModel)
        {
            DlmsSettingsViewModel = settingsViewModel;
        }

        private void InitWrapperHeader()
        {
            NetFrame = new NetFrame()
            {
                Version = new AxdrIntegerUnsigned16("1"),
                SourceAddress = new AxdrIntegerUnsigned16((DlmsSettingsViewModel.ClientAddress.ToString("X4"))),
                DestAddress = new AxdrIntegerUnsigned16((DlmsSettingsViewModel.ServerAddress.ToString("X4"))),
            };
        }

        public byte[] InvokeApdu(byte[] apduBytes)
        {
            InitWrapperHeader();
            NetFrame.DLMSApduDataBytes = apduBytes;
            return NetFrame.ToPduBytes();
        }
    }
}