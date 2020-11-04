using Microsoft.Toolkit.Mvvm.ComponentModel;
using 三相智慧能源网关调试软件.DLMS.Axdr;
using 三相智慧能源网关调试软件.DLMS.Wrapper;

namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{
    public class NetFrameMaker : ObservableObject
    {
        private DLMSSettingsViewModel DlmsSettingsViewModel { get; }

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


        public NetFrameMaker(DLMSSettingsViewModel settingsViewModel)
        {
            DlmsSettingsViewModel = settingsViewModel;
        }

        private void InitWrapperHeader()
        {
            NetFrame = new NetFrame()
            {
                Version = new AxdrUnsigned16("1"),
                SourceAddress = new AxdrUnsigned16((DlmsSettingsViewModel.ClientAddress.ToString("X4"))),
                DestAddress = new AxdrUnsigned16((DlmsSettingsViewModel.ServerAddress.ToString("X4"))),
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