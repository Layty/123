using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.Association;
using 三相智慧能源网关调试软件.DLMS.Wrapper;

namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{


    public class NetFrameMaker:ViewModelBase
    {
        private DLMSSettingsViewModel DlmsSettingsViewModel { get; set; }

        public NetFrame NetFrame
        {
            get => _netFrame;
            set { _netFrame = value; RaisePropertyChanged(); }
        }
        private NetFrame _netFrame;


        public NetFrameMaker(DLMSSettingsViewModel settingsViewModel)
        {
            DlmsSettingsViewModel = settingsViewModel;
          
            _netFrame = new NetFrame()
            {
                Version = new byte[] {0x00, 0x01},
                SourceAddress = BitConverter.GetBytes(DlmsSettingsViewModel.ClientAddress).Reverse().ToArray(),
                DestAddress = BitConverter.GetBytes(DlmsSettingsViewModel.ServerAddress).Reverse().ToArray(),
            };
        }


        public byte[] AarqRequest()
        {
            List<byte> aarq = new List<byte>();
            aarq.AddRange(new AssociationRequest(DlmsSettingsViewModel).ToPduBytes());
            aarq.InsertRange(0, new byte[] {(byte) Command.Aarq, (byte) aarq.Count});
            _netFrame = new NetFrame()
            {
                Version = new byte[] { 0x00, 0x01 },
                SourceAddress = BitConverter.GetBytes(DlmsSettingsViewModel.ClientAddress).Reverse().ToArray(),
                DestAddress = BitConverter.GetBytes(DlmsSettingsViewModel.ServerAddress).Reverse().ToArray(),
            };
            _netFrame.DLMSApduDataBytes = aarq.ToArray();
            return _netFrame.ToPduBytes();
        }


        public byte[] BuildPduRequestBytes(byte[] pduBytes)
        {
            _netFrame = new NetFrame()
            {
                Version = new byte[] { 0x00, 0x01 },
                SourceAddress = BitConverter.GetBytes(DlmsSettingsViewModel.ClientAddress).Reverse().ToArray(),
                DestAddress = BitConverter.GetBytes(DlmsSettingsViewModel.ServerAddress).Reverse().ToArray(),
            };
            _netFrame.DLMSApduDataBytes = pduBytes;
            return _netFrame.ToPduBytes();
        }


        public byte[] ReleaseRequest()
        {
            List<byte> alrq = new List<byte>();
            alrq.InsertRange(0, new byte[] {(byte) Command.ReleaseRequest, (byte) alrq.Count});
            _netFrame = new NetFrame()
            {
                Version = new byte[] { 0x00, 0x01 },
                SourceAddress = BitConverter.GetBytes(DlmsSettingsViewModel.ClientAddress).Reverse().ToArray(),
                DestAddress = BitConverter.GetBytes(DlmsSettingsViewModel.ServerAddress).Reverse().ToArray(),
            };
            _netFrame.DLMSApduDataBytes = alrq.ToArray();
            return _netFrame.ToPduBytes();
        }
    }
}