using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using 三相智慧能源网关调试软件.DLMS;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.HDLC.Enums;

namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{
    public class DLMSSettingsViewModel : ObservableObject
    {
       
        public bool UseLogicalNameReferencing { get; set; }
        public Array StartProtocolArray => Enum.GetValues(typeof(StartProtocolType));
        public Array CommunicationTypeArray => Enum.GetValues(typeof(CommunicationType));
        public Array InterfaceTypeArray => Enum.GetValues(typeof(InterfaceType));
        public InterfaceType InterfaceType { get; set; } 

        public StartProtocolType StartProtocolType { get; set; } 
        public ConnectionState Connected { get; set; }

        /// <summary>
        /// 物理层通道类型
        /// </summary>
        public CommunicationType CommunicationType 
        {
            get => _communicationType;
            set
            {
                _communicationType = value;
                OnPropertyChanged();
            }
        }

        private CommunicationType _communicationType;

        [DefaultValue(6)]
        public byte DlmsVersion { get; set; }

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


        public byte[] PasswordHex { get; private set; }


        [Required]
        [StringLength(8, MinimumLength = 8)]
        public string PasswordString
        {
            get => _passwordString;
            set
            {
                _passwordString = value;
                PasswordHex = Encoding.Default.GetBytes(value);
                OnPropertyChanged();
            }
        }

        private string _passwordString ;

     


        public ushort ClientAddress
        {
            get => _clientAddress;
            set { _clientAddress = value; OnPropertyChanged(); }
        }
        private ushort _clientAddress;


     

        public ushort ServerAddress
        {
            get => _serverAddress;
            set { _serverAddress = value; OnPropertyChanged(); }
        }
        private ushort _serverAddress;

        public byte ServerAddressSize { get; set; } = 1;

        /// <summary>
        /// 最后执行的命令
        /// </summary>
        public Command LastCommand { get; set; }

        public uint BlockIndex
        {
            get;
            internal set;
        }
        public ushort BlockNumberAck
        {
            get;
            set;
        }
        public DLMSInfo DlmsInfo { get; set; }

        public DLMSInfo DlmsInfoFromMeter { get; set; }


        public string SystemTitle { get=> _systemTitle;
            set { _systemTitle = value; OnPropertyChanged(); }
        }
        private string _systemTitle;
        public DLMSSettingsViewModel()
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
            DlmsInfoFromMeter=new DLMSInfo();
            
            PasswordString = "33333333";
            CommunicationType = CommunicationType.SerialPort;
            InterfaceType = InterfaceType.HDLC;
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
                return Conformance.BlockTransferWithGetOrRead | Conformance.BlockTransferWithSetOrWrite | Conformance.BlockTransferWithAction | Conformance.MultipleReferences | Conformance.Get | Conformance.Set | Conformance.SelectiveAccess | Conformance.Action;
            }
            return Conformance.Read | Conformance.Write | Conformance.UnconfirmedWrite | Conformance.MultipleReferences | Conformance.InformationReport | Conformance.ParameterizedAccess;
        }
        public int NegotiateBaud
        {
            get => _negotiateBaud;
            set { _negotiateBaud = value; OnPropertyChanged(); }
        }
        private int _negotiateBaud;
        public int StartBaud
        {
            get => _startBaud;
            set { _startBaud = value; OnPropertyChanged(); }
        }
        private int _startBaud ;
    }
}