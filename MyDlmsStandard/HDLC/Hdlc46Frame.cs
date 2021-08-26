using MyDlmsStandard.Common;

namespace MyDlmsStandard.HDLC
{
    public class Hdlc46Frame
    {
        /// <summary>
        /// 起始和结束帧
        /// </summary>
        internal const byte HdlcFrameStartEnd = 0x7E;

        public HdlcFrameFormatField FrameFormatField;

        /// <summary>
        /// 目的地址
        /// </summary>
        public AAddress DestAddress1;

        /// <summary>
        /// 源地址
        /// </summary>
        public AAddress SourceAddress1;

        /// <summary>
        /// HDLC控制码
        /// </summary>
        public HdlcControlField HdlcControlField { get; set; }


        public byte[] GetFrameFormatField(int count)
        {
            FrameFormatField = new HdlcFrameFormatField() {FrameLengthSubField = (ushort) count};
            return FrameFormatField.ToHexPdu().StringToByte();
            //   return new byte[] {0xA0, Convert.ToByte(count)};
        }


        public Hdlc46Frame(byte destAddress1, byte sourceAddress1)
        {
            FrameFormatField = new HdlcFrameFormatField();
            DestAddress1 = new AAddress() {Size = 1, Upper = destAddress1};
            SourceAddress1 = new AAddress() {Upper = sourceAddress1, Size = 1};
            HdlcControlField = new HdlcControlField()
            {
                CurrentReceiveSequenceNumber = 0,
                CurrentSendSequenceNumber = 0,
            };
            CurrentReceiveSequenceNumber = 0;
            CurrentSendSequenceNumber = 0;
            LlcHeadFrameBytes = HdlcLlc.LLCSendBytes;
        }

        public Hdlc46Frame(ushort destAddress1, byte sourceAddress1)
        {
            FrameFormatField = new HdlcFrameFormatField();
            this.DestAddress1 = new AAddress {Size = 2, Upper = 0x01, Lower = destAddress1}; //
            SourceAddress1 = new AAddress {Upper = sourceAddress1, Size = 1};
            CurrentReceiveSequenceNumber = 0;
            CurrentSendSequenceNumber = 0;
            LlcHeadFrameBytes = HdlcLlc.LLCSendBytes;
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


        private int _currentSendSequenceNumber;

        public int CurrentReceiveSequenceNumber
        {
            get => _currentReceiveSequenceNumber;
            set
            {
                bool flag = value >= 8;
                _currentReceiveSequenceNumber = flag ? 0 : value;
            }
        }

        private int _currentReceiveSequenceNumber;

        public byte[] Hcs = new byte[2];
        public byte[] Fcs = new byte[2];

        public byte[] LlcHeadFrameBytes;
    }
}