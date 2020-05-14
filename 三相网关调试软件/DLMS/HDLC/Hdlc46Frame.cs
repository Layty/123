using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using GalaSoft.MvvmLight;
using 三相智慧能源网关调试软件.DLMS.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.CosemObjects;
using 三相智慧能源网关调试软件.DLMS.HDLC.Enums;

namespace 三相智慧能源网关调试软件.DLMS.HDLC
{
    public class Hdlc46Frame : ObservableObject
    {
        public int SplitBitFlagByte
        {
            get => _splitBitFlagByte;
            set
            {
                _splitBitFlagByte = value;
                bool flag = value == 1;
                if (flag)
                {
                    _splitBitFlag = true;
                }
                else
                {
                    bool flag2 = value == 0;
                    if (flag2)
                    {
                        _splitBitFlag = false;
                    }
                }
            }
        }


        public bool SplitBitFlag
        {
            get => _splitBitFlag;
            set
            {
                _splitBitFlag = value;
                _splitBitFlagByte = (value ? 1 : 0);
            }
        }


        public ushort FrameLength
        {
            set
            {
                bool flag = value <= 2047;
                if (flag)
                {
                    _framelength = value;
                }
            }
        }

        private byte[] _frameformatfield;

        public byte[] FrameFormatField
        {
            get => _frameformatfield;
            set { _frameformatfield = value; }
        }

        /// <summary>
        /// 1/2/4个字节
        /// </summary>
        public byte[] DestAddress
        {
            get => _destAddressForFrame;
            set
            {
                if (value.Length != 1 && value.Length != 2 && value.Length != 4)
                {
                    throw new Exception("目的地址字节长度不符合1/2/4字节");
                }

                DestAddrSize = (byte) value.Length;
                _destAddressForFrame = value;
            }
        }

        public byte DestAddrSize { get; set; } = 1;

        private byte[] _destAddressForFrame;

        public ushort LogicAddr
        {
            get => _upperAddrForFrame;
            set => _upperAddrForFrame = (ushort) ((value << 1) + 1);
        }


        public byte SourceAddress
        {
            get => _sourceAddress;
            set => _sourceAddress = (byte) ((value << 1) + 1);
        }


        public byte ControlField { get; set; }


        public Hdlc46Frame(byte[] destAddress, string password, byte sourceAddress)
        {
            CurrentReceiveSequenceNumber = 0;
            CurrentSendSequenceNumber = 0;
            DestAddress = destAddress;
            SourceAddress = sourceAddress;
            LlcHeadFrameBytes = HdlcLlc.LLCSendBytes;
            PasswordLvl1 = PasswordLvl.LLS;
            Password = password;

            Apdu = new APDU();

            Info = new HDLCInfo();
        }


        [Required]
        [StringLength(8, MinimumLength = 8)]
        public string Password { get; set; }


        public byte[] HexPassword => Encoding.Default.GetBytes(Password);


        public string AuthenticationKey { get; set; }


        public int CurrentReceiveSequenceNumber
        {
            get => _currentReceiveSequenceNumber;
            set
            {
                bool flag = value >= 8;
                _currentReceiveSequenceNumber = flag ? 0 : value;
                RaisePropertyChanged();
            }
        }


        public int CurrentSendSequenceNumber
        {
            get => _currentSendSequenceNumber;
            set
            {
                bool flag = value >= 8;
                _currentSendSequenceNumber = flag ? 0 : value;
                RaisePropertyChanged();
            }
        }


        internal const byte HdlcFrameStartEnd = 0x7E;

        public const byte FrameType = 160;

        /// <summary>
        /// Size of Server address.
        /// </summary>
        public byte ServerAddressSize { get; set; }

        private int _splitBitFlagByte;


        private bool _splitBitFlag;


        public byte HeightBit = 7;


        private ushort _framelength;


        private ushort _upperAddrForFrame;

        public ushort PhysicalAddr;

        private byte _sourceAddress;


        public byte[] Hcs = new byte[2];


        public byte[] Fcs = new byte[2];


        public HDLCInfo Info { get; set; }


        public byte[] LlcHeadFrameBytes;


        public PasswordLvl PasswordLvl1;


        private int _currentReceiveSequenceNumber;

        private int _currentSendSequenceNumber;


        private ushort _maxReceivePduSize = 65535;

        public ushort MaxReceivePduSize
        {
            get => _maxReceivePduSize;
            set
            {
                _maxReceivePduSize = value;
                RaisePropertyChanged();
            }
        }

        private byte _invokeId;

        public byte InvokeId
        {
            get => _invokeId;
            set
            {
                _invokeId = value;
                RaisePropertyChanged();
            }
        }


        public APDU Apdu;


        public enum PasswordLvl
        {
            NO,
            LLS,
            HLS,
        }

        private Command _lastCommand;

        public Command LastCommand
        {
            get => _lastCommand;
            set
            {
                _lastCommand = value;
                RaisePropertyChanged();
            }
        }


        public class APDU : IToPduBytes
        {
            public Command Command { get; set; }
            public Invoke_Id_And_Priority InvokeIdAndPriority { get; set; }
            public CosemMethodDescriptor CosemMethodDescriptor { get; set; }
            public DLMSDataItem DlmsDataItem { get; set; }

            public APDU()
            {
                InvokeIdAndPriority = new Invoke_Id_And_Priority(1, ServiceClass.Confirmed, Priority.High);
            }


            public byte[] ToPduBytes()
            {
                throw new NotImplementedException();
            }
        }
    }
}