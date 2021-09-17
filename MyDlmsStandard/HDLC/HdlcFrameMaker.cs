using System;
using System.Collections.Generic;
using System.Linq;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;

namespace MyDlmsStandard.HDLC
{
    public class HdlcFrameMaker
    {
        private readonly ushort _serverAddress;
        private readonly byte _clientAddress;
        private readonly DLMSInfo _info;

        public Hdlc46FrameBase Hdlc46FrameBase { get; set; }
        public Command LastCommand { get; set; }

        public HdlcFrameMaker(ushort serverAddress, byte clientAddress, DLMSInfo info)
        {
            _serverAddress = serverAddress;
            _clientAddress = clientAddress;
            _info = info;
            Hdlc46FrameBase = new Hdlc46FrameBase((byte) _serverAddress,
                _clientAddress);
        }

        public HdlcFrameMaker(byte serverAddress, byte clientAddress, DLMSInfo info)
        {
            _serverAddress = serverAddress;
            _clientAddress = clientAddress;
            _info = info;
            Hdlc46FrameBase = new Hdlc46FrameBase((byte) _serverAddress,
                _clientAddress);
        }

       
     
        

        public byte[] DisconnectRequest()
        {
            List<byte> disConnect = new List<byte>();
            PackagingDestinationAndSourceAddress(disConnect);
            LastCommand = Command.DisconnectRequest;
            disConnect.Add((byte) Command.DisconnectRequest);
            byte count = (byte) (disConnect.Count + 2 + 2); //FCS=2,FrameFormatField=2 ,目的地址=1/2/4，源地址=1

            disConnect.InsertRange(0, Hdlc46FrameBase.GetFrameFormatField(count));
            PackingFcs_And_FrameStartEnd(disConnect);
            InitFrameSequenceNumber();
            return disConnect.ToArray();
        }


        public byte[] ReleaseRequest()
        {
            List<byte> rlrqListBytes = new List<byte>();
            PackagingDestinationAndSourceAddress(rlrqListBytes);
            int ctr = ((Hdlc46FrameBase.CurrentReceiveSequenceNumber << 1) + 1 << 4) +
                      (Hdlc46FrameBase.CurrentSendSequenceNumber << 1);

            rlrqListBytes.Add((byte) ctr);
            byte count = (byte) (2 + Hdlc46FrameBase.DestAddress1.Size + 1 + 1 + 2 + 3 + 1 + 1 + 3 + 2);

            rlrqListBytes.InsertRange(0, Hdlc46FrameBase.GetFrameFormatField(count));
            PackingHcs(rlrqListBytes);
            rlrqListBytes.AddRange(Hdlc46FrameBase.LlcHeadFrameBytes);
            LastCommand = Command.ReleaseRequest;
            rlrqListBytes.Add((byte) Command.ReleaseRequest); //
            rlrqListBytes.AddRange(new byte[]
            {
                3, //len
                0x80, //标签([0],IMPLICIT)编码    reason [0] IMPLICIT Release_request_reason OPTIONAL,
                1, //标记组件的值域的长度的编码
                0 //值(0,normal)的编码
            });
            PackingFcs_And_FrameStartEnd(rlrqListBytes);

            InitFrameSequenceNumber();

            return rlrqListBytes.ToArray();
        }

        public byte[] GetUiFrameBytes()
        {
            List<byte> listUi = new List<byte>();
            PackagingDestinationAndSourceAddress(listUi);
            listUi.Add((byte) Command.UiCommand); //19
            LastCommand = Command.UiCommand;
            byte count = (byte) (listUi.Count + 4); //4=FCS+帧头帧尾
            listUi.InsertRange(0, Hdlc46FrameBase.GetFrameFormatField(count));
            PackingFcs_And_FrameStartEnd(listUi);
            return listUi.ToArray();
        }

        public byte[] BuildPduRequestBytes(byte[] pduBytes)
        {
            List<byte> getRequest = new List<byte>();
            PackagingDestinationAndSourceAddress(getRequest); //源地址固定一个字节，目的地址1~4个字节
            int ctr = ((Hdlc46FrameBase.CurrentReceiveSequenceNumber << 1) + 1 << 4) +
                      (Hdlc46FrameBase.CurrentSendSequenceNumber << 1);
            IncreaseFrameSequenceNumber();
            getRequest.Add((byte) ctr);
            //2个起始帧字节,目的地址长度，1个源地址长度+ 1字节ctr  +2个字节Hcs，3个字节LLCHead 13字节+2字节FCS
            byte count = (byte) (2 + Hdlc46FrameBase.DestAddress1.Size + 1 + 1 + 2 + 3 + pduBytes.Length +
                                 Hdlc46FrameBase.Fcs.Length);

            getRequest.InsertRange(0, Hdlc46FrameBase.GetFrameFormatField(count));
            PackingHcs(getRequest);
            getRequest.AddRange(Hdlc46FrameBase.LlcHeadFrameBytes); //3字节

            #region 13字节Apdu

            getRequest.AddRange(pduBytes);

            #endregion

            PackingFcs_And_FrameStartEnd(getRequest);
            return getRequest.ToArray();
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
            int ctr = ((Hdlc46FrameBase.CurrentReceiveSequenceNumber << 1) + 1 << 4) +
                      (Hdlc46FrameBase.CurrentSendSequenceNumber << 1);
            IncreaseFrameSequenceNumber();
            enterUpgradeMode.Add((byte) ctr);
            byte count = (byte) (2 + Hdlc46FrameBase.DestAddress1.Size + 2 + 3 + 2 + 2 + 2 + 2);

            enterUpgradeMode.InsertRange(0, Hdlc46FrameBase.GetFrameFormatField(count));
            PackingHcs(enterUpgradeMode);
            enterUpgradeMode.AddRange(Hdlc46FrameBase.LlcHeadFrameBytes);

            enterUpgradeMode.AddRange(new byte[] {0xFF, 0x10});
            enterUpgradeMode.AddRange(BitConverter.GetBytes(id).Reverse().ToArray());

            PackingFcs_And_FrameStartEnd(enterUpgradeMode);
            return enterUpgradeMode.ToArray();
        }

        public byte[] ReceiveReady()
        {
            List<byte> rr = new List<byte>();
            PackagingDestinationAndSourceAddress(rr);
            int ctr = ((Hdlc46FrameBase.CurrentReceiveSequenceNumber << 1) + 1 << 4) + 1;
            rr.Add((byte) ctr);
            byte count = (byte) (2 + Hdlc46FrameBase.DestAddress1.Size + 1 + 1 + 2);
            rr.InsertRange(0, Hdlc46FrameBase.GetFrameFormatField(count));
            PackingFcs_And_FrameStartEnd(rr);
            return rr.ToArray();
        }

        public byte[] ReceiveNotReady()
        {
            List<byte> rnrListBytes = new List<byte>();
            PackagingDestinationAndSourceAddress(rnrListBytes);
            int ctr = ((Hdlc46FrameBase.CurrentReceiveSequenceNumber << 1) + 1 << 4) + 5;
            rnrListBytes.Add((byte) ctr);
            byte count = (byte) (2 + Hdlc46FrameBase.DestAddress1.Size + 1 + 1 + 2);

            rnrListBytes.InsertRange(0, Hdlc46FrameBase.GetFrameFormatField(count));
            PackingFcs_And_FrameStartEnd(rnrListBytes);
            return rnrListBytes.ToArray();
        }

        private void PackagingDestinationAndSourceAddress(List<byte> bytes)
        {
            bytes.AddRange(Hdlc46FrameBase.DestAddress1.ToPdu());
            bytes.AddRange(Hdlc46FrameBase.SourceAddress1.ToPdu());
        }

        private void PackingHcs(List<byte> bytes)
        {
            Hdlc46FrameBase.Hcs = DataCheck.CRC16_CCITT(bytes.ToArray(), bytes.Count);
            bytes.AddRange(Hdlc46FrameBase.Hcs);
        }

        private void PackingFcs(List<byte> bytes)
        {
            Hdlc46FrameBase.Fcs = DataCheck.CRC16_CCITT(bytes.ToArray(), bytes.Count);
            bytes.AddRange(Hdlc46FrameBase.Fcs);
        }

        private void PackingFrameStartEnd(List<byte> bytes)
        {
            bytes.Insert(0, Hdlc46FrameBase.HdlcFrameStartEnd); //添加帧头
            bytes.Add(Hdlc46FrameBase.HdlcFrameStartEnd); //添加帧尾
        }

        private void PackingFcs_And_FrameStartEnd(List<byte> bytes)
        {
            PackingFcs(bytes);
            PackingFrameStartEnd(bytes);
        }

        public void InitFrameSequenceNumber()
        {
            Hdlc46FrameBase.CurrentReceiveSequenceNumber = 0;
            Hdlc46FrameBase.CurrentSendSequenceNumber = 0;
        }

        public void IncreaseFrameSequenceNumber()
        {
            Hdlc46FrameBase.CurrentSendSequenceNumber++;
            Hdlc46FrameBase.CurrentReceiveSequenceNumber++;
        }

        internal static object GetHdlcAddress(int value, byte size)
        {
            //1个字节
            if (size < 2 && value < 128)
            {
                return (byte) ((value << 1) | 1);
            }

            //2个字节
            if (size < 4 && value < 16384)
            {
                return (ushort) (((value & 0x3F80) << 2) | ((value & 0x7F) << 1) | 1);
            }

            //4个字节
            if (value < 268435456)
            {
                return (uint) (((value & 0xFE00000) << 4) | ((value & 0x1FC000) << 3) | ((value & 0x3F80) << 2) |
                               ((value & 0x7F) << 1) | 1);
            }

            throw new ArgumentException("Invalid address.");
        }

       
    }
}