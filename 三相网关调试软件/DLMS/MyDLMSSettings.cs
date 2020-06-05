using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using GalaSoft.MvvmLight;
using 三相智慧能源网关调试软件.DLMS.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.HDLC.Enums;


namespace 三相智慧能源网关调试软件.DLMS
{
    public class MyDLMSSettings : ViewModelBase
    {

        public Array StartProtocolArray => Enum.GetValues(typeof(StartProtocolType));
        public Array CommunicationTypeArray => Enum.GetValues(typeof(CommunicationType));
        public Array InterfaceTypeArray => Enum.GetValues(typeof(InterfaceType));
        public InterfaceType InterfaceType { get; set; } = InterfaceType.HDLC;

        public StartProtocolType StartProtocolType { get; set; } = StartProtocolType.DLMS;
        public ConnectionState Connected { get; set; }


        public CommunicationType CommunicationType 
        {
            get => _communicationType;
            set
            {
                _communicationType = value;
                RaisePropertyChanged();
            }
        }

        private CommunicationType _communicationType = CommunicationType.SerialPort;

        public byte DLMSVersion { get; set; } = 6;

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
        private byte _invokeId = 0x1;

        /// <summary>
        /// Invoke ID.
        /// </summary>
        internal uint LongInvokeId = 0x1;

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

        public Priority Priority { get; }
        public ServiceClass ServiceClass { get; }
        public ushort MaxServerPDUSize { get; }

        /// <summary>
        /// 当建立连接时，客户端告诉它想要使用什么类型的服务。
        /// </summary>
        public Conformance ProposedConformance { get; }

        /// <summary>
        /// 服务器告诉什么功能是可用的，客户端就会知道
        /// </summary>
        internal Conformance NegotiatedConformance = 0;

        public bool AutoIncreaseInvokeID { get; set; }


        public byte[] PasswordHex => Encoding.Default.GetBytes(_passwordString);


        [Required]
        [StringLength(8, MinimumLength = 8)]
        public string PasswordString
        {
            get => _passwordString;
            set
            {
                _passwordString = value;
                RaisePropertyChanged();
            }
        }

        private string _passwordString = "33333333";


        public byte ClientAddress { get; set; } = 1;
        public byte ServerAddress { get; set; } = 1;
        public byte ServerAddressSize { get; set; } = 1;

        /// <summary>
        /// 最后执行的命令
        /// </summary>
        public Command LastCommand { get; set; }

        public DLMSInfo DlmsInfo { get; set; }


        public MyDLMSSettings()
        {
            DLMSVersion = 6;
            InvokeId = 0x1;
            Priority = Priority.High;
            ServiceClass = ServiceClass.Confirmed;
            MaxServerPDUSize = MaxReceivePduSize = DefaultMaxReceivePduSize;
            ProposedConformance = (Conformance) 0;
            DlmsInfo = new DLMSInfo();
        }

        //public MyDLMSSettings(InterfaceType interfaceType, StartProtocolType startProtocolType = StartProtocolType.DLMS)
        //{
        //    InterfaceType = interfaceType;
        //    StartProtocolType = startProtocolType;
        //    CommunicationType = CommunicationType.SerialPort;
        //    DLMSVersion = 6;
        //    InvokeId = 0x1;
        //    Priority = Priority.High;
        //    ServiceClass = ServiceClass.Confirmed;
        //    MaxServerPDUSize = MaxReceivePduSize = DefaultMaxReceivePduSize;
        //    ProposedConformance = (Conformance) 0;
        //    DlmsInfo = new DLMSInfo();
        //}

        public int RequestBaud { get; set; }
    }
}