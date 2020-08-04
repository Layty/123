﻿using System;
using System.Collections.Generic;
using System.Linq;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.HDLC
{
    public class HdlcFrameMaker
    {
        private readonly MyDLMSSettings _dlmsSettings;

        public  Hdlc46Frame Hdlc46Frame { get; set; }

        public HdlcFrameMaker(MyDLMSSettings dlmsSettings)
        {
            _dlmsSettings = dlmsSettings;
        }

        public byte[] SNRMRequest(bool snrmContainInfoFlag = true)
        {
            Hdlc46Frame = new Hdlc46Frame(_dlmsSettings.ServerAddress,
                _dlmsSettings.ClientAddress);
            InitFrameSequenceNumber();
            List<byte> snrmFrame = new List<byte>();
            PackagingDestinationAndSourceAddress(snrmFrame);
            _dlmsSettings.LastCommand = Command.Snrm;
            snrmFrame.Add((byte) Command.Snrm);
            byte[] snrmInfo = { };
            byte hcs = 0;
            if (snrmContainInfoFlag)
            {
                hcs = 2;
                snrmInfo = _dlmsSettings.DlmsInfo.GetSnrmInfo();
            }

            //不包含头尾2个0x7E
            //FrameFormatField=2，,1/2/4个字节的目的地址字节数，  command=1个字节， hcs字节=2，snrminfo=12, 2个字节FCS,
            int count = 2 + Hdlc46Frame.DestAddress.Length + 1 + 1 + hcs + snrmInfo.Length + Hdlc46Frame.Fcs.Length;
            snrmFrame.InsertRange(0, Hdlc46Frame.GetFrameFormatField(count));

            if (snrmContainInfoFlag)
            {
                PackingHcs(snrmFrame);
                snrmFrame.AddRange(_dlmsSettings.DlmsInfo.GetSnrmInfo());
            }

            PackingFcs_And_FrameStartEnd(snrmFrame);

            return snrmFrame.ToArray();
        }

        public byte[] DisconnectRequest()
        {
            List<byte> disConnect = new List<byte>();
            PackagingDestinationAndSourceAddress(disConnect);
            _dlmsSettings.LastCommand = Command.DisconnectRequest;
            disConnect.Add((byte) Command.DisconnectRequest);
            byte count = (byte) (disConnect.Count + 2 + 2); //FCS=2,FrameFormatField=2 ,目的地址=1/2/4，源地址=1
        
            disConnect.InsertRange(0, Hdlc46Frame.GetFrameFormatField(count));
            PackingFcs_And_FrameStartEnd(disConnect);
            InitFrameSequenceNumber();
            return disConnect.ToArray();
        }

        /// <summary>
        /// Application Association Request 应用连接请求    
        ///对应 Application Association Response 应用连接响应
        /// </summary>
        /// <returns></returns>
        public byte[] AarqRequest()
        {
            List<byte> arrqListBytes = new List<byte>();
            PackagingDestinationAndSourceAddress(arrqListBytes);
            int ctr = ((Hdlc46Frame.CurrentReceiveSequenceNumber << 1) + 1 << 4) +
                      (Hdlc46Frame.CurrentSendSequenceNumber << 1);
            arrqListBytes.Add((byte) ctr);

            List<byte> appApduContentList = new List<byte>();

            #region ———application-context-name域 (application-context-name [1], OBJECT IDENTIFIER)
            //ApplicationContextName
            appApduContentList.AddRange(new ApplicationContextName().ToPduBytes());
            #endregion

            #region 认证功能单元的域的编码,只有在选择了身份验证功能单元时，才会出现以下字段。

           // bool flag = _hdlc46Frame.PasswordLvl1 == _hdlc46Frame.PasswordLvl.LLS;
            if (true)
            {
                //认证功能单元域的编码

                #region sender_acse_requirements [10] IMPLICIT ACSE_requirements OPTIONAL,

                appApduContentList.AddRange(new byte[]
                {
                    0x8A, //acse-requirements 域 ([10],IMPLICIT, Context-specific)的标签的编码
                    0x02, //标记组件的值域的长度的编码
                    0x07, //BITSTRING 的 最 后 字 节 未 使 用 比 特 数 的编码
                    0x80 //认证功能单元(0)的编码 注:需要重点关注,不同客户机之间的比特 数的编码可能会有所不同,但在 COSEM 语 境中,只有 BIT0设置为1(基于标识认证功 能单元的要求)
                });

                #endregion

                #region mechanism_name[11] IMPLICIT Mechanism_name OPTIONAL,
                appApduContentList.AddRange(new MechanismName().ToPduBytes());
                #endregion

                #region calling_authentication_value[12] EXPLICIT Authentication_value OPTIONAL,
                appApduContentList.AddRange(new CallingAuthenticationValue(_dlmsSettings.PasswordHex).ToPduBytes());
                #endregion
            }

            #endregion

            #region user_information [30] EXPLICIT Association_information OPTIONAL

            //UserInformation
            appApduContentList.Add(0xBE); //标签([30],Context-specific,Constructed) 的编码
            appApduContentList.Add(0x10); //标记组件值域长度的编码 
            appApduContentList.Add(0x04); //user-information(OCTET STRING,Uni- versal)选项的编码 BerType.OctetString
            appApduContentList.Add(0x0E); // OCTETSTRING 值 域 长 度(14octets)的 编码

            appApduContentList.AddRange(new byte[]
            {
                1, //DLMSAPDU 选项(InitiateRequest)的标签的编码 DLMSAPDUCHOICE(InitiateRequest)的标签的编码
                0, //专用密钥组件(OCTETSTRINGOPTIONAL)的编码使用标志(FALSE,不存在)
                0, //response-allowed组件(BOOLEANDEFAULTTRUE)的编码 /使用标志(FALSE,默认值为 TRUE输送)
                0, //proposed-quality-of-service组 件 ([0]IMPLICITInteger8 OP- TIONAL)的编码 使用标记(FALSE,不出现)
                6, //值为6,一个 Unsigned8的 A-XDR编码是它本身的值  ProposedDlmsVersionNumber proposed-dlms-version-number Unsigned8,
            });

            appApduContentList.AddRange(new byte[]
            {
                0x5F,
                0x1F, //31  Conformance ::= [APPLICATION 31] BIT STRING --(SIZE(24)) //[APPLICATION31]标签的编码(ASN.1显示标签)b
                0x04, //contents域的8位元组(4)的长度的编码
                0x00, //BITSTRING最后字节未使用的比特数的编码
                0x00, 0x7F, 0x1F //定长 BITSTRING的值的编码  --ProposedConformance
                /*  0x04, 0xB0*/ //值为 0x04B0,一个 Unsigned16的编码是它本身的值  0x7E, 0x1F/7C FF
            }); //user-information:xDLMS InitiateRequestAPDU   0,  0 = 0x04,0xB0值为 0x04B0,一个 Unsigned16的编码是它本身的值
            appApduContentList.AddRange(BitConverter.GetBytes(_dlmsSettings.MaxReceivePduSize));

            #endregion

            byte count = (byte) (2 + Hdlc46Frame.DestAddress.Length + 1 + 1 + 2 + 3 + 1 + 1 + appApduContentList.Count +
                                 2);

            arrqListBytes.InsertRange(0, Hdlc46Frame.GetFrameFormatField(count));

            PackingHcs(arrqListBytes);
            arrqListBytes.AddRange(Hdlc46Frame.LlcHeadFrameBytes);

            _dlmsSettings.LastCommand = Command.Aarq;
            arrqListBytes.Add((byte) Command.Aarq);
            arrqListBytes.Add((byte) appApduContentList.Count);
            arrqListBytes.AddRange(appApduContentList);

            PackingFcs_And_FrameStartEnd(arrqListBytes);
            IncreaseFrameSequenceNumber();
            return arrqListBytes.ToArray();
        }

        public byte[] ReleaseRequest()
        {
            List<byte> rlrqListBytes = new List<byte>();
            PackagingDestinationAndSourceAddress(rlrqListBytes);
            int ctr = ((Hdlc46Frame.CurrentReceiveSequenceNumber << 1) + 1 << 4) +
                      (Hdlc46Frame.CurrentSendSequenceNumber << 1);

            rlrqListBytes.Add((byte) ctr);
            byte count = (byte) (2 + Hdlc46Frame.DestAddress.Length + 1 + 1 + 2 + 3 + 1 + 1 + 3 + 2);
         
            rlrqListBytes.InsertRange(0, Hdlc46Frame.GetFrameFormatField(count));
            PackingHcs(rlrqListBytes);
            rlrqListBytes.AddRange(Hdlc46Frame.LlcHeadFrameBytes);
            _dlmsSettings.LastCommand = Command.ReleaseRequest;
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
            _dlmsSettings.LastCommand = Command.UiCommand;
            byte count = (byte) (listUi.Count + 4); //4=FCS+帧头帧尾
            listUi.InsertRange(0, Hdlc46Frame.GetFrameFormatField(count));
            PackingFcs_And_FrameStartEnd(listUi);
            return listUi.ToArray();
        }

        public byte[] BuildPduRequestBytes(byte[] pduBytes)
        {
            List<byte> getRequest = new List<byte>();
            PackagingDestinationAndSourceAddress(getRequest); //源地址固定一个字节，目的地址1~4个字节
            int ctr = ((Hdlc46Frame.CurrentReceiveSequenceNumber << 1) + 1 << 4) +
                      (Hdlc46Frame.CurrentSendSequenceNumber << 1);
            IncreaseFrameSequenceNumber();
            getRequest.Add((byte)ctr);
            //2个起始帧字节,目的地址长度，1个源地址长度+ 1字节ctr  +2个字节Hcs，3个字节LLCHead 13字节+2字节FCS
            byte count = (byte)(2 + Hdlc46Frame.DestAddress.Length + 1 + 1 + 2 + 3 + pduBytes.Length + Hdlc46Frame.Fcs.Length);
       
            getRequest.InsertRange(0, Hdlc46Frame.GetFrameFormatField(count));
            PackingHcs(getRequest);
            getRequest.AddRange(Hdlc46Frame.LlcHeadFrameBytes); //3字节

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
            int ctr = ((Hdlc46Frame.CurrentReceiveSequenceNumber << 1) + 1 << 4) +
                      (Hdlc46Frame.CurrentSendSequenceNumber << 1);
            IncreaseFrameSequenceNumber();
            enterUpgradeMode.Add((byte) ctr);
            byte count = (byte) (2 + Hdlc46Frame.DestAddress.Length + 2 + 3 + 2 + 2 + 2 + 2);

            enterUpgradeMode.InsertRange(0, Hdlc46Frame.GetFrameFormatField(count));
            PackingHcs(enterUpgradeMode);
            enterUpgradeMode.AddRange(Hdlc46Frame.LlcHeadFrameBytes);

            enterUpgradeMode.AddRange(new byte[] {0xFF, 0x10});
            enterUpgradeMode.AddRange(BitConverter.GetBytes(id).Reverse().ToArray());

            PackingFcs_And_FrameStartEnd(enterUpgradeMode);
            return enterUpgradeMode.ToArray();
        }

        public byte[] ReceiveReady()
        {
            List<byte> rr = new List<byte>();
            PackagingDestinationAndSourceAddress(rr);
            int ctr = ((Hdlc46Frame.CurrentReceiveSequenceNumber << 1) + 1 << 4) + 1;
            rr.Add((byte) ctr);
            byte count = (byte) (2 + Hdlc46Frame.DestAddress.Length + 1 + 1 + 2);
            rr.InsertRange(0, Hdlc46Frame.GetFrameFormatField(count));
            PackingFcs_And_FrameStartEnd(rr);
            return rr.ToArray();
        }

        public byte[] ReceiveNotReady()
        {
            List<byte> rnrListBytes = new List<byte>();
            PackagingDestinationAndSourceAddress(rnrListBytes);
            int ctr = ((Hdlc46Frame.CurrentReceiveSequenceNumber << 1) + 1 << 4) + 5;
            rnrListBytes.Add((byte) ctr);
            byte count = (byte) (2 + Hdlc46Frame.DestAddress.Length + 1 + 1 + 2);
         
            rnrListBytes.InsertRange(0, Hdlc46Frame.GetFrameFormatField(count));
            PackingFcs_And_FrameStartEnd(rnrListBytes);
            return rnrListBytes.ToArray();
        }

        private void PackagingDestinationAndSourceAddress(List<byte> bytes)
        {
            bytes.AddRange(Hdlc46Frame.DestAddress);
            bytes.Add(Hdlc46Frame.SourceAddress);
        }

        private void PackingHcs(List<byte> bytes)
        {
            Hdlc46Frame.Hcs = DataCheck.CRC16_CCITT(bytes.ToArray(), bytes.Count);
            bytes.AddRange(Hdlc46Frame.Hcs);
        }

        private void PackingFcs(List<byte> bytes)
        {
            Hdlc46Frame.Fcs = DataCheck.CRC16_CCITT(bytes.ToArray(), bytes.Count);
            bytes.AddRange(Hdlc46Frame.Fcs);
        }

        private void PackingFrameStartEnd(List<byte> bytes)
        {
            bytes.Insert(0, Hdlc46Frame.HdlcFrameStartEnd); //添加帧头
            bytes.Add(Hdlc46Frame.HdlcFrameStartEnd); //添加帧尾
        }

        private void PackingFcs_And_FrameStartEnd(List<byte> bytes)
        {
            PackingFcs(bytes);
            PackingFrameStartEnd(bytes);
        }

        public void InitFrameSequenceNumber()
        {
            Hdlc46Frame.CurrentReceiveSequenceNumber = 0;
            Hdlc46Frame.CurrentSendSequenceNumber = 0;
        }

        public void IncreaseFrameSequenceNumber()
        {
            Hdlc46Frame.CurrentSendSequenceNumber++;
            Hdlc46Frame.CurrentReceiveSequenceNumber++;
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