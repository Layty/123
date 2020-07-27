using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using GalaSoft.MvvmLight;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.HDLC.Enums;
using DataType = 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums.DataType;

namespace 三相智慧能源网关调试软件.DLMS
{
    public class DLMSCommon
    {
        public static DataType GetDLMSDataType(Type type)
        {
            if (type == null)
            {
                return DataType.NullData;
            }
            if (type == typeof(int))
            {
                return DataType.Int32;
            }
            if (type == typeof(uint))
            {
                return DataType.UInt32;
            }
            if (type == typeof(string))
            {
                return DataType.String;
            }
            if (type == typeof(byte))
            {
                return DataType.UInt8;
            }
            if (type == typeof(sbyte))
            {
                return DataType.Int8;
            }
            if (type == typeof(short))
            {
                return DataType.Int16;
            }
            if (type == typeof(ushort))
            {
                return DataType.UInt16;
            }
            if (type == typeof(long))
            {
                return DataType.Int64;
            }
            if (type == typeof(ulong))
            {
                return DataType.UInt64;
            }
            if (type == typeof(float))
            {
                return DataType.Float32;
            }
            if (type == typeof(double))
            {
                return DataType.Float64;
            }
            //if (type == typeof(DateTime) || type == typeof(GXDateTime))
            //{
            //    return DataType.DateTime;
            //}
            //if (type == typeof(GXDate))
            //{
            //    return DataType.Date;
            //}
            //if (type == typeof(GXTime))
            //{
            //    return DataType.Time;
            //}
            if (type == typeof(bool))
            {
                return DataType.Boolean;
            }
            if (type == typeof(byte[]))
            {
                return DataType.OctetString;
            }
            //if (type == typeof(GXStructure))
            //{
            //    return DataType.Structure;
            //}
            //if (type == typeof(GXArray) || type == typeof(object[]))
            //{
            //    return DataType.Array;
            //}
            //if (type == typeof(GXEnum) || type.IsEnum)
            //{
            //    return DataType.Enum;
            //}
            //if (type == typeof(GXBitString))
            //{
            //    return DataType.BitString;
            //}
            //if (type == typeof(GXByteBuffer))
            //{
            //    return DataType.OctetString;
            //}
            throw new Exception("Failed to convert data type to DLMS data type. Unknown data type.");
        }
    }
    public class MyDLMSSettings : ViewModelBase
    {
        public bool UseLogicalNameReferencing { get; set; }
        public Array StartProtocolArray => Enum.GetValues(typeof(StartProtocolType));
        public Array CommunicationTypeArray => Enum.GetValues(typeof(CommunicationType));
        public Array InterfaceTypeArray => Enum.GetValues(typeof(InterfaceType));
        public InterfaceType InterfaceType { get; set; } 

        public StartProtocolType StartProtocolType { get; set; } 
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

        private CommunicationType _communicationType;

        public byte DlmsVersion { get; set; }

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

        public Priority Priority { get; set; }
        public Array PriorityArray { get; set; } = Enum.GetValues(typeof(Priority));
        public ServiceClass ServiceClass { get; set; }
        public Array ServiceClassArray { get; set; } = Enum.GetValues(typeof(ServiceClass));
        public ushort MaxServerPDUSize { get; }

        /// <summary>
        /// 当建立连接时，客户端告诉它想要使用什么类型的服务。
        /// </summary>
        public Conformance ProposedConformance { get; set; }

        public Array ProposedConformanceArray { get; set; } = Enum.GetValues(typeof(Conformance));
        /// <summary>
        /// 服务器告诉什么功能是可用的，客户端就会知道
        /// </summary>
        internal Conformance NegotiatedConformance = 0;

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
                RaisePropertyChanged();
            }
        }

        private string _passwordString ;

        public byte ClientAddress { get; set; } = 1;
        public byte ServerAddress { get; set; } = 1;
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

        public int RequestBaud { get; set; }
        public MyDLMSSettings()
        {
            UseLogicalNameReferencing = true;
            DlmsVersion = 6;
            InvokeId = 0x1;
            Priority = Priority.High;
            ServiceClass = ServiceClass.Confirmed;
            MaxServerPDUSize = MaxReceivePduSize = DefaultMaxReceivePduSize;
            ProposedConformance = (Conformance) 0x7E1F;
            DlmsInfo = new DLMSInfo();
            PasswordString = "33333333";
            CommunicationType = CommunicationType.SerialPort;
            InterfaceType = InterfaceType.HDLC;
            StartProtocolType = StartProtocolType.DLMS;
        }

      
    }
}