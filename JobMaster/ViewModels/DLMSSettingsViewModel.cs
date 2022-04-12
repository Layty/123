using CommunityToolkit.Mvvm.ComponentModel;
using MyDlmsStandard;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.HDLC.Enums;

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JobMaster.ViewModels
{
    public partial class DlmsSettingsViewModel : ObservableValidator
    {
        public bool UseLogicalNameReferencing { get; set; }
        public Array StartProtocolArray => Enum.GetValues(typeof(StartProtocolType));
        public Array PhysicalChanelTypeArray => Enum.GetValues(typeof(PhysicalChanelType));
        public Array ProtocolInterfaceTypeArray => Enum.GetValues(typeof(ProtocolInterfaceType));


        [ObservableProperty]
        private ProtocolInterfaceType _protocolInterfaceType;

        public StartProtocolType StartProtocolType { get; set; }
        public ConnectionState Connected { get; set; }

        /// <summary>
        /// 物理层通道类型
        /// </summary>

        [ObservableProperty]
        private PhysicalChanelType physicalChanelType;

        [DefaultValue(6)] public byte DlmsVersion { get; set; }

        [DefaultValue(65535)]
        public ushort MaxReceivePduSize
        {
            get => MaxReceiveReceivePduSize;
            set
            {
                if (value < 64)
                {
                    throw new ArgumentOutOfRangeException("MaxReceivePDUSize");
                }

                MaxReceiveReceivePduSize = value;
            }
        }

        internal ushort MaxReceiveReceivePduSize;

        private const ushort DefaultMaxReceivePduSize = 0xFFFF;

        public ushort NegotiatedMaxPduSize { get; internal set; }

        private byte _invokeId = 0x1;


        public byte InvokeId
        {
            get => _invokeId;
            set
            {
                if (value > 0xF)
                {
                    throw new ArgumentException("Invalid InvokeID");
                }

                _invokeId = value;
            }
        }

        public Priority Priority { get; set; }
        public Array PriorityArray { get; set; } = Enum.GetValues(typeof(Priority));
        public ServiceClass ServiceClass { get; set; }
        public Array ServiceClassArray { get; set; } = Enum.GetValues(typeof(ServiceClass));
        public ushort MaxServerPDUSize { get; }

        /// <summary>
        /// 当建立连接时，客户端告诉它想要使用什么类型的服务。
        /// </summary>
        public Conformance ProposedConformance { get; set; }

        /// <summary>
        /// 服务器告诉什么功能是可用的，客户端就会知道
        /// </summary>
        internal Conformance NegotiatedConformance = 0;

        public Array ProposedConformanceArray { get; set; } = Enum.GetValues(typeof(Conformance));


        public bool AutoIncreaseInvokeID { get; set; }


        public byte[] PasswordHex => Encoding.Default.GetBytes(_passwordString);


        [Required]
        [StringLength(8, MinimumLength = 8)]
        public string PasswordString
        {
            get => _passwordString;
            set => SetProperty(ref _passwordString, value, true);
        }


        [AlsoNotifyChangeFor(nameof(PasswordHex))]
        private string _passwordString;


    
        [ObservableProperty]
        private ushort _clientAddress;



        [ObservableProperty]
        private ushort _serverAddress;

        public byte ServerAddressSize { get; set; } = 1;

        /// <summary>
        /// 最后执行的命令
        /// </summary>
        public Command LastCommand { get; set; }

        public uint BlockIndex { get; internal set; }
        public ushort BlockNumberAck { get; set; }
        public DLMSInfo DlmsInfo { get; set; }

        public DLMSInfo DlmsInfoFromMeter { get; set; }


       
        [ObservableProperty]
        private string _systemTitle;

        public DlmsSettingsViewModel()
        {
            UseLogicalNameReferencing = true;
            DlmsVersion = 6;
            InvokeId = 0x1;
            Priority = Priority.High;
            ServiceClass = ServiceClass.Confirmed;
            MaxServerPDUSize = MaxReceivePduSize = DefaultMaxReceivePduSize;
            // ProposedConformance = (Conformance) 0x7E1F;
            ProposedConformance = GetInitialConformance(true);

            DlmsInfo = new DLMSInfo();
            DlmsInfoFromMeter = new DLMSInfo();

            PasswordString = "33333333";
            PhysicalChanelType = PhysicalChanelType.SerialPort;
            ProtocolInterfaceType = ProtocolInterfaceType.HDLC;
            StartProtocolType = StartProtocolType.DLMS;
            ClientAddress = 1;
            ServerAddress = 1;
            SystemTitle = "00000000000000000000";
            NegotiateBaud = 9600;
            StartBaud = 300;
        }

        public static Conformance GetInitialConformance(bool useLogicalNameReferencing)
        {
            if (useLogicalNameReferencing)
            {
                return Conformance.BlockTransferWithGetOrRead | Conformance.BlockTransferWithSetOrWrite |
                       Conformance.BlockTransferWithAction | Conformance.MultipleReferences | Conformance.Get |
                       Conformance.Set | Conformance.SelectiveAccess | Conformance.Action;
            }

            return Conformance.Read | Conformance.Write | Conformance.UnconfirmedWrite |
                   Conformance.MultipleReferences | Conformance.InformationReport | Conformance.ParameterizedAccess;
        }

   
        [ObservableProperty]
        private int _negotiateBaud;

      
        [ObservableProperty]
        private int _startBaud;
    }
}