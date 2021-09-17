using Microsoft.Toolkit.Mvvm.ComponentModel;
using MyDlmsStandard.Axdr;
using MyDlmsStandard.Common;
using MyDlmsStandard.Wrapper;


namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{
    public class NetFrameMaker : ObservableObject
    {
        private DlmsSettingsViewModel DlmsSettingsViewModel { get; }

        public WrapperFrame WrapperFrame
        {
            get => _wrapperFrame;
            set
            {
                _wrapperFrame = value;
                OnPropertyChanged();
            }
        }

        private WrapperFrame _wrapperFrame;


        public NetFrameMaker(DlmsSettingsViewModel settingsViewModel)
        {
            DlmsSettingsViewModel = settingsViewModel;
        }

        private void InitWrapperHeader()
        {
            WrapperFrame = new WrapperFrame()
            {
                WrapperHeader = new WrapperHeader()
                {
                    Version = new AxdrIntegerUnsigned16("1"),
                    SourceAddress = new AxdrIntegerUnsigned16((DlmsSettingsViewModel.ClientAddress.ToString("X4"))),
                    DestAddress = new AxdrIntegerUnsigned16((DlmsSettingsViewModel.ServerAddress.ToString("X4"))),
                }
            };
        }

        public byte[] InvokeApdu(byte[] apduBytes)
        {
            InitWrapperHeader();
            WrapperFrame.WrapperBody.DataBytes = apduBytes;
            return WrapperFrame.ToPduStringInHex().StringToByte();
        }
    }
}