using System;

namespace MyDlmsStandard.HDLC
{
    public class Hdlc46Frame 
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

        public byte[] FrameFormatField { get; set; }

        public byte[] GetFrameFormatField(int count)
        {
            return new byte[]{0xA0, Convert.ToByte(count) };
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
        public Hdlc46Frame(ushort destAddress, byte sourceAddress)
        {
            CurrentReceiveSequenceNumber = 0;
            CurrentSendSequenceNumber = 0;
            var d =(byte) ((destAddress << 1) + 1);
            DestAddress =new byte[]{ d };  
            SourceAddress = sourceAddress;
            LlcHeadFrameBytes = HdlcLlc.LLCSendBytes;
        }
        
        public string AuthenticationKey { get; set; }

        public int CurrentReceiveSequenceNumber
        {
            get => _currentReceiveSequenceNumber;
            set
            {
                bool flag = value >= 8;
                _currentReceiveSequenceNumber = flag ? 0 : value;
              
            }
        }


        public int CurrentSendSequenceNumber
        {
            get => _currentSendSequenceNumber;
            set
            {
                bool flag = value >= 8;
                _currentSendSequenceNumber = flag ? 0 : value;
          
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

        public byte[] LlcHeadFrameBytes;


        private int _currentReceiveSequenceNumber;

        private int _currentSendSequenceNumber;


    }
}