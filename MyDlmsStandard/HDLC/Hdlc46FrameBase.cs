using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyDlmsStandard.HDLC
{

    public class Hdlc46FrameBase : IPduStringInHexConstructor, IToPduStringInHex
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

        public byte[] Apdu { get; set; }


        public byte[] GetFrameFormatField(int count)
        {
            FrameFormatField = new HdlcFrameFormatField() { FrameLengthSubField = (ushort)count };
            return FrameFormatField.ToHexPdu().StringToByte();
        }

        public Hdlc46FrameBase(byte destAddress1, byte sourceAddress1, DLMSInfo dlmsInfo) : this(destAddress1,
            sourceAddress1)
        {
        }

        public Hdlc46FrameBase(ushort destAddress1, byte sourceAddress1, DLMSInfo dlmsInfo) : this(destAddress1,
            sourceAddress1)
        {
        }

        public Hdlc46FrameBase(byte destAddress1, byte sourceAddress1)
        {
            FrameFormatField = new HdlcFrameFormatField();
            DestAddress1 = new AAddress() { Size = 1, Upper = destAddress1 };
            SourceAddress1 = new AAddress() { Upper = sourceAddress1, Size = 1 };
            HdlcControlField = new HdlcControlField()
            {
                CurrentReceiveSequenceNumber = 0,
                CurrentSendSequenceNumber = 0,
            };
            CurrentReceiveSequenceNumber = 0;
            CurrentSendSequenceNumber = 0;
            LlcHeadFrameBytes = HdlcLlc.LLCSendBytes;
        }

        public Hdlc46FrameBase(ushort destAddress1, byte sourceAddress1)
        {
            FrameFormatField = new HdlcFrameFormatField();
            this.DestAddress1 = new AAddress { Size = 2, Upper = 0x01, Lower = destAddress1 }; //
            SourceAddress1 = new AAddress { Upper = sourceAddress1, Size = 1 };
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


        public void InitFrameSequenceNumber()
        {
            CurrentReceiveSequenceNumber = 0;
            CurrentSendSequenceNumber = 0;
        }

        public void IncreaseFrameSequenceNumber()
        {
            CurrentSendSequenceNumber++;
            CurrentReceiveSequenceNumber++;
        }

        public void PackagingDestinationAndSourceAddress(List<byte> bytes)
        {
            bytes.AddRange(DestAddress1.ToPdu());
            bytes.AddRange(SourceAddress1.ToPdu());
        }

        public void PackingHcs(List<byte> bytes)
        {
            Hcs = DataCheck.CRC16_CCITT(bytes.ToArray(), bytes.Count);
            bytes.AddRange(Hcs);
        }

        public void PackingFcs_And_FrameStartEnd(List<byte> bytes)
        {
            PackingFcs(bytes);
            PackingFrameStartEnd(bytes);
        }

        private void PackingFcs(List<byte> bytes)
        {
            Fcs = DataCheck.CRC16_CCITT(bytes.ToArray(), bytes.Count);
            bytes.AddRange(Fcs);
        }

        private void PackingFrameStartEnd(List<byte> bytes)
        {
            bytes.Insert(0, HdlcFrameStartEnd); //添加帧头
            bytes.Add(HdlcFrameStartEnd); //添加帧尾
        }

        public virtual byte[] ToPduBytes()
        {
            List<byte> list = new List<byte>();
            PackagingDestinationAndSourceAddress(list);
            int ctr = ((CurrentReceiveSequenceNumber << 1) + 1 << 4) +
                      (CurrentSendSequenceNumber << 1);
            list.Add((byte)ctr);
            byte count = (byte)(2 + DestAddress1.Size + 1 + 1 + 2 + 3 + Apdu.Length + 2);
            list.InsertRange(0, GetFrameFormatField(count));
            PackingHcs(list);
            list.AddRange(LlcHeadFrameBytes);

            list.AddRange(Apdu);

            PackingFcs_And_FrameStartEnd(list);
            IncreaseFrameSequenceNumber();
            return list.ToArray();
        }

        /// <summary>
        /// 解析UA帧
        /// </summary>
        /// <param name="replyData"></param>
        /// <returns></returns>
        public bool ParseUaResponse(byte[] replyData)
        {
            if (replyData.Length == 0)
            {
                return false;
            }

            if (replyData.First() != 0x7E)
            {
                return false;
            }

            if (replyData.Last() != 0x7E)
            {
                return false;
            }

            //TODO  要根据源地址和目的地址的字节数来取
            //当源地址和目的地址均为1时replyData[5] == 115
            //replyData[?] == 115;
            return true;
        }
        /// <summary>
        /// 进入基表升级模式
        /// </summary>
        /// <param name="id">256进入自定义升级模式</param>
        /// <returns></returns>
        public byte[] SetEnterUpGradeMode(ushort id)
        {
            //7E A0 10 03 03 32 8F 31 E6 E6 00 FF 10   01 00   C4 08 7E 
            List<byte> enterUpgradeMode = new List<byte>();
            PackagingDestinationAndSourceAddress(enterUpgradeMode);
            int ctr = ((CurrentReceiveSequenceNumber << 1) + 1 << 4) +
                      (CurrentSendSequenceNumber << 1);
            IncreaseFrameSequenceNumber();
            enterUpgradeMode.Add((byte)ctr);
            byte count = (byte)(2 + DestAddress1.Size + 2 + 3 + 2 + 2 + 2 + 2);

            enterUpgradeMode.InsertRange(0, GetFrameFormatField(count));
            PackingHcs(enterUpgradeMode);
            enterUpgradeMode.AddRange(LlcHeadFrameBytes);

            enterUpgradeMode.AddRange(new byte[] { 0xFF, 0x10 });
            enterUpgradeMode.AddRange(BitConverter.GetBytes(id).Reverse().ToArray());

            PackingFcs_And_FrameStartEnd(enterUpgradeMode);
            return enterUpgradeMode.ToArray();
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {//7E A0 19 03 03 32 EC C8 E6 E6 00 C0 01 C1 00 01 00 00 60 32 15 FF 02 00 62 C4 7E
            if (pduStringInHex.Length == 0)
            {
                return false;
            }
            // TODO:对帧序号校验？

            var pduAndFcsBytes = pduStringInHex.StringToByte().Skip(11).ToArray();
            Apdu = pduAndFcsBytes.Take(pduAndFcsBytes.Length - 3).ToArray();
            return true;
        }

        public string ToPduStringInHex()
        {
            return ToPduBytes().ByteToString();
        }
    }
}