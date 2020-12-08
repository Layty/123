using System;
using System.Linq;
using MyDlmsStandard.Common;

namespace MyDlmsStandard.HDLC
{
    /// <summary>
    /// HDLC地址，4字节，分高字节和低字节，长度可变，
    /// 源地址固定一个字节，目的地址1~4个字节
    /// 客户机端地址应固定为单一字节表示。扩展寻址的使用把客户机地址的范围限制在128。
    /// HDLC扩展寻址机制应适用于上述两种地址域。这种地址扩展规定了可变长度的地址域。
    /// </summary>
    public struct AAddress
    {
        public ushort Upper;

        public ushort Lower;

        public int Size;

        public byte[] ToPdu()
        {
            byte[] array = new byte[Size];
            switch (Size)
            {
                case 1:
                    array[0] = (byte) ((Upper << 1) | 1);
                    break;
                case 2:
                    array[0] = (byte) (Upper << 1);
                    array[1] = (byte) ((Lower << 1) | 1);
                    break;
                case 4:
                    array[0] = (byte) (Upper >> 7 << 1);
                    array[1] = (byte) ((Upper & 0x7F) << 1);
                    array[2] = (byte) (Lower >> 7 << 1);
                    array[3] = (byte) (((Lower & 0x7F) << 1) | 1);
                    break;
                default:
                    array = new byte[0];
                    break;
            }

            return array;
        }

        public static AAddress FromPdu(byte[] pdu, ref int index)
        {
            AAddress result = default(AAddress);
            result.Size = 1;
            int num = index;
            while ((pdu[num++] & 1) == 0)
            {
                result.Size++;
            }

            switch (result.Size)
            {
                case 1:
                    result.Upper = (ushort) (pdu[index] >> 1);
                    break;
                case 2:
                    result.Upper = (ushort) (pdu[index] >> 1);
                    result.Lower = (ushort) (pdu[index + 1] >> 1);
                    break;
                case 4:
                    result.Upper = (ushort) ((pdu[index] >> 1 << 7) | (pdu[index + 1] >> 1));
                    result.Lower = (ushort) ((pdu[index + 2] >> 1 << 7) | (pdu[index + 3] >> 1));
                    break;
            }

            index += result.Size;
            return result;
        }
    }

    public class HDLCFrameFormatField
    {
        /// <summary>
        /// 由于使用的是类型3,format type 恒定为0b1010
        /// </summary>
        public const ushort FrameType = 0b1010;

        public bool SplitBit = false;


        public ushort FrameLengthSubField { get; set; } = 0;

        public string ToHexPdu()
        {
            ushort splitBitValue = 0;

            if (SplitBit)
            {
                splitBitValue = 1 << 12;
            }

            var data = (ushort) (FrameType << 12) + splitBitValue + FrameLengthSubField;
            return data.ToString("X4");
        }

        public bool ParsePdu()
        {
            ///
            return true;
        }
    }

    /// <summary>
    /// 该区域长度为1byte，用于指示命令和回应的类型，同时还包含了数据帧的帧序号
    /// </summary>
    public class HDLCControlField
    {
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

        private int _currentReceiveSequenceNumber;

        private int _currentSendSequenceNumber;
    }

    public class Hdlc46Frame
    {
        internal const byte HdlcFrameStartEnd = 0x7E;
        public HDLCFrameFormatField FrameFormatField;

        public AAddress DestAddress1;

        public AAddress SourceAddress1;
        public HDLCControlField HdlcControlField { get; set; }


        public byte[] GetFrameFormatField(int count)
        {
            FrameFormatField=new HDLCFrameFormatField(){FrameLengthSubField = (ushort)count};
            return FrameFormatField.ToHexPdu().StringToByte();
            //   return new byte[] {0xA0, Convert.ToByte(count)};
        }


        public Hdlc46Frame(byte destAddress1, byte sourceAddress1)
        {
            FrameFormatField = new HDLCFrameFormatField();
            DestAddress1 = new AAddress() {Size = 1, Upper = destAddress1};
            SourceAddress1 = new AAddress() {Upper = sourceAddress1, Size = 1};
            HdlcControlField = new HDLCControlField()
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
            FrameFormatField = new HDLCFrameFormatField();
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