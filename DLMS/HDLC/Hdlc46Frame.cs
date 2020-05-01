using System;
using System.Text;
using GalaSoft.MvvmLight;
using Gurux.DLMS;
using 三相智慧能源网关调试软件.DLMS.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.CosemObjects;
using Command = 三相智慧能源网关调试软件.DLMS.HDLC.Enums.Command;

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
        public byte[] DestAddr
        {
            get => _destAddrForFrame;
            set => _destAddrForFrame = value;
        }

        public byte DestAddrSize { get; set; } = 1;

        private byte[] _destAddrForFrame;

        public ushort LogicAddr
        {
            get => _upperAddrForFrame;
            set { _upperAddrForFrame = (ushort) ((value << 1) + 1); }
        }


        public byte SourceAddr
        {
            get => _sourceAddrForFrame;
            set { _sourceAddrForFrame = (byte) ((value << 1) + 1); }
        }


        public byte ControlField { get; set; }



        public Hdlc46Frame(byte[] destAddr, string password, byte sourceAddr)
        {
            CurrentReceiveSequenceNumber = 0;
            CurrentSendSequenceNumber = 0;
            DestAddr = destAddr;
            SourceAddr = sourceAddr;
            LlcHeadFrameBytes = HdlcLlc.LLCSendBytes;
            PasswordLvl1 = PasswordLvl.LLS;
            Password = password;

            Apdu = new APDU();

            Info = new HDLCInfo();
        }

      

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

   
    

        internal const byte HDLCFrameStartEnd = 0x7E;

        public const byte FrameType = 160;

        /// <summary>
        /// Size of Server address.
        /// </summary>
        public byte ServerAddressSize
        {
            get;
            set;
        }
        private int _splitBitFlagByte;


        private bool _splitBitFlag;


        public byte HeightBit = 7;


        private ushort _framelength;


        private ushort _upperAddrForFrame;

        public ushort PhysicalAddr;

        private byte _sourceAddrForFrame;


        public byte[] Hcs = new byte[2];


        public byte[] Fcs = new byte[2];


        public HDLCInfo Info { get; set; }


        public byte[] LlcHeadFrameBytes;


        public PasswordLvl PasswordLvl1;


        private int _currentReceiveSequenceNumber;

        private int _currentSendSequenceNumber;

        private ushort _maxReceivePduSize;

        public ushort MaxReceivePduSize
        {
            get { return _maxReceivePduSize; }
            set { _maxReceivePduSize = value; RaisePropertyChanged(); }
        }

        private byte _invokeId;

        public byte InvokeId
        {
            get { return _invokeId; }
            set { _invokeId = value; RaisePropertyChanged(); }
        }


        public APDU Apdu;


        public enum PasswordLvl
        {
            HLS,

            LLS,

            NO
        }
        private Command _lastCommand;

        public Command LastCommand
        {
            get { return _lastCommand; }
            set { _lastCommand = value; RaisePropertyChanged(); }
        }


        public class APDU : IToPduBytes
        {
            public Command Command { get; set; }
            public Invoke_Id_And_Priority InvokeIdAndPriority { get; set; }
            public CosemMethodDescriptor CosemMethodDescriptor { get; set; }
            public DLMSDataItem DlmsDataItem { get; set; }

            public APDU()
            {
                InvokeIdAndPriority = new Invoke_Id_And_Priority(1, ServiceClass.Confirmed, Priority.High, 0);
            }


            public byte[] ToPduBytes()
            {
                throw new System.NotImplementedException();
            }
        }
    }
}