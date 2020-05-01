using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gurux.DLMS;
using Gurux.DLMS.Enums;
using 三相智慧能源网关调试软件.DLMS.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.CosemObjects;
using 三相智慧能源网关调试软件.DLMS.OBIS;
using Command = 三相智慧能源网关调试软件.DLMS.HDLC.Enums.Command;
using DataType = 三相智慧能源网关调试软件.DLMS.ApplicationLayEnums.DataType;

namespace 三相智慧能源网关调试软件.DLMS.HDLC
{
    public class HdlcFrameMaker
    {
        public GXDLMSSettings settings = new GXDLMSSettings();


        public Hdlc46Frame Hdlc46Frame { get; set; }

        public HdlcFrameMaker(byte[] destAddr, string password, byte sourceAddr)
        {
            Hdlc46Frame = new Hdlc46Frame(destAddr, password, sourceAddr);
        }

        public HdlcFrameMaker(MyDLMSSettings settings)
        {
        }

        public bool Connected { get; set; }


        public byte[] SNRMRequest(bool snrmContainInfoFlag = true)
        {
            InitFrameSequenceNumber();
            List<byte> snrmFrame = new List<byte>();
            PackagingDestinationAndSourceAddress(snrmFrame);
            Hdlc46Frame.LastCommand = Command.Snrm;
            snrmFrame.Add((byte) Command.Snrm);
            byte[] snrmInfo = new byte[] { };
            byte hcs = 0;
            if (snrmContainInfoFlag)
            {
                hcs = 2;
                snrmInfo = Hdlc46Frame.Info.GetSnrmInfo();
            }

            //不包含头尾2个0x7E
            //FrameFormatField=2，,1/2/4个字节的目的地址字节数，  command=1个字节， hcs字节=2，snrminfo=12, 2个字节FCS,
            int count = 2 + Hdlc46Frame.DestAddr.Length + 1 + 1 + hcs + snrmInfo.Length + 2;

            Hdlc46Frame.FrameFormatField = new byte[]
            {
                160,
                Convert.ToByte(count)
            };
            snrmFrame.InsertRange(0, Hdlc46Frame.FrameFormatField);

            if (snrmContainInfoFlag)
            {
                PackingHcs(snrmFrame);
                snrmFrame.AddRange(Hdlc46Frame.Info.GetSnrmInfo());
            }

            PackingFcs_And_FrameStartEnd(snrmFrame);

            return snrmFrame.ToArray();
        }

        public byte[] DisconnectRequest()
        {
            List<byte> disConnect = new List<byte>();
            PackagingDestinationAndSourceAddress(disConnect);
            Hdlc46Frame.LastCommand = Command.DisconnectRequest;
            disConnect.Add((byte) Command.DisconnectRequest);
            byte len = (byte) (disConnect.Count + 2 + 2); //FCS=2,FrameFormatField=2 ,目的地址=1/2/4，源地址=1
            Hdlc46Frame.FrameFormatField = new byte[]
            {
                160,
                Convert.ToByte(len)
            };
            disConnect.InsertRange(0, Hdlc46Frame.FrameFormatField);
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
            //ApplicationContextName
            appApduContentList.Add(0xA1); //标签([1],Context-specific)的编码
            appApduContentList.Add(0x09); //标记组件值域长度的编码
            appApduContentList
                .Add(0x06); //appApduContentList.Add((byte)BerType.ObjectIdentifier); //application-context-name(OBJECTIDEN- TIFIER,Universal)选项的编码
            appApduContentList.Add(0x07); //对象标识符的值域的长度的编码
            appApduContentList.AddRange(new byte[]
            {
                0x60,
                0x85,
                0x74,
                0x05,
                0x08,
                0x01,
                0x01 //0x01,0x03
            }); //对象标识符的值的编码
            bool flag = Hdlc46Frame.PasswordLvl1 == Hdlc46Frame.PasswordLvl.LLS;
            if (flag)
            {
                //认证功能单元域的编码
                appApduContentList.AddRange(new byte[]
                {
                    0x8A, //acse-requirements 域 ([10],IMPLICIT, Context-specific)的标签的编码
                    0x02, //标记组件的值域的长度的编码
                    0x07, //BITSTRING 的 最 后 字 节 未 使 用 比 特 数 的编码
                    0x80 //认证功能单元(0)的编码 注:需要重点关注,不同客户机之间的比特 数的编码可能会有所不同,但在 COSEM 语 境中,只有 BIT0设置为1(基于标识认证功 能单元的要求)
                });
                appApduContentList.AddRange(new byte[]
                {
                    0x8B, //标签([11],IMPLICIT,Context -specific)的编码
                    7, //标记组件的值域的长度的编码
                });
                appApduContentList.AddRange(new byte[]
                {
                    0x60,
                    0x85,
                    0x74,
                    0x05,
                    0x08,
                    0x01,
                    0x01
                }); //OBJECTIDENTIFIER的值的编码: low-level-security-mechanism-name(1), high-level-security-mechanism-name(5)
                appApduContentList.Add(0xAC); //标签([12],EXPLICIT, Context-specific)的编码
                appApduContentList.Add(0x0A); //标记组件的值域的长度的编码
                appApduContentList.AddRange(new byte[]
                {
                    0x80, //Authentication-value(charstring[0]IM- PLICITGraphicString)选项的编码
                    0x08 //Authentication-value值 域 长 度 的 编 码 (8 字节)
                });
                appApduContentList.AddRange(Hdlc46Frame.HexPassword);
            }

            //UserInformation
            appApduContentList.Add(0xBE); //标签([30],Context-specific,Constructed) 的编码
            appApduContentList.Add(0x10); //标记组件值域长度的编码 
            appApduContentList.Add(0x04); //user-information(OCTET STRING,Uni- versal)选项的编码 BerType.OctetString
            appApduContentList.Add(0x0E); // OCTETSTRING 值 域 长 度(14octets)的 编码
            appApduContentList.AddRange(new byte[]
            {
                1, //DLMSAPDU 选项(InitiateRequest)的标签的编码
                0,
                0,
                0,
                6, //值为6,一个 Unsigned8的 A-XDR编码是它本身的值
                0x5F, //95
                0x1F, //31
                0x04, //contents域的8位元组(4)的长度的编码
                0, //BITSTRING最后字节未使用的比特数的编码
                0, 0x7E, 0x1F, //定长 BITSTRING的值的编码
                /*  0x04, 0xB0*/ //值为 0x04B0,一个 Unsigned16的编码是它本身的值
            }); //user-information:xDLMS InitiateRequestAPDU   0,  0 = 0x04,0xB0值为 0x04B0,一个 Unsigned16的编码是它本身的值
            appApduContentList.AddRange(BitConverter.GetBytes(Hdlc46Frame.MaxReceivePduSize));
            byte count = (byte) (2 + Hdlc46Frame.DestAddr.Length + 1 + 1 + 2 + 3 + 1 + 1 + appApduContentList.Count +
                                 2);
            Hdlc46Frame.FrameFormatField = new byte[]
            {
                160,
                Convert.ToByte(count)
            };
            arrqListBytes.InsertRange(0, Hdlc46Frame.FrameFormatField);

            PackingHcs(arrqListBytes);
            arrqListBytes.AddRange(Hdlc46Frame.LlcHeadFrameBytes);

            Hdlc46Frame.LastCommand = Command.Aarq;
            arrqListBytes.Add((byte) Command.Aarq);
            arrqListBytes.Add((byte) appApduContentList.Count);
            arrqListBytes.AddRange(appApduContentList);


            PackingFcs_And_FrameStartEnd(arrqListBytes);
            IncreaseFrameSequenceNumber();
            return arrqListBytes.ToArray();
        }

        public byte[] RLRQRequest()
        {
            List<byte> rlrqListBytes = new List<byte>();
            PackagingDestinationAndSourceAddress(rlrqListBytes);
            int ctr = ((Hdlc46Frame.CurrentReceiveSequenceNumber << 1) + 1 << 4) +
                      (Hdlc46Frame.CurrentSendSequenceNumber << 1);

            rlrqListBytes.Add((byte) ctr);
            byte count = (byte) (2 + Hdlc46Frame.DestAddr.Length + 1 + 1 + 2 + 3 + 1 + 1 + 3 + 2);
            Hdlc46Frame.FrameFormatField = new byte[]
            {
                160,
                Convert.ToByte(count)
            };
            rlrqListBytes.InsertRange(0, Hdlc46Frame.FrameFormatField);
            PackingHcs(rlrqListBytes);
            rlrqListBytes.AddRange(Hdlc46Frame.LlcHeadFrameBytes);
            rlrqListBytes.Add((byte) Command.ReleaseRequest); //
            rlrqListBytes.AddRange(new byte[]
            {
                3, //len
                128,
                1,
                0
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
            byte len = (byte) (listUi.Count + 4); //4=FCS+帧头帧尾
            Hdlc46Frame.FrameFormatField = new byte[]
            {
                160,
                Convert.ToByte(len)
            };
            listUi.InsertRange(0, Hdlc46Frame.FrameFormatField);
            PackingFcs_And_FrameStartEnd(listUi);
            return listUi.ToArray();
        }

        public byte[] GetRequest(DLMSObject cosem, byte attrIndex)
        {
            List<byte> getRequest = new List<byte>();
            PackagingDestinationAndSourceAddress(getRequest); //源地址固定一个字节，目的地址1~4个字节
            int ctr = ((Hdlc46Frame.CurrentReceiveSequenceNumber << 1) + 1 << 4) +
                      (Hdlc46Frame.CurrentSendSequenceNumber << 1);
            IncreaseFrameSequenceNumber();
            getRequest.Add((byte) ctr);
            //2个起始帧字节,目的地址长度，1个源地址长度+ 1字节ctr  +2个字节Hcs，3个字节LLCHead 13字节+2字节FCS
            byte count = (byte) (2 + Hdlc46Frame.DestAddr.Length + 1 + 1 + 2 + 3 + 13 + 2);
            Hdlc46Frame.FrameFormatField = new byte[]
            {
                160,
                Convert.ToByte(count)
            };
            getRequest.InsertRange(0, Hdlc46Frame.FrameFormatField);

            PackingHcs(getRequest);

            getRequest.AddRange(Hdlc46Frame.LlcHeadFrameBytes); //3字节

            #region 13字节Apdu

            getRequest.Add((byte) Command.GetRequest); // get - request[192] GET - Request,
            //  getRequest.AddRange(new byte[] {0x01, 0x40});//
            getRequest.Add((byte) GetRequestType.Normal);
            getRequest.Add(Hdlc46Frame.Apdu.InvokeIdAndPriority.InvokeIdAndPriority);
            getRequest.AddRange(BitConverter.GetBytes((ushort) cosem.ObjectType).Reverse()); //ClassId为两个字节
            getRequest.AddRange(OBISHelper.ObisStringToBytes(cosem.LogicalName));
            getRequest.Add(attrIndex);
            getRequest.Add(cosem.Version);

            #endregion

            PackingFcs_And_FrameStartEnd(getRequest);
            return getRequest.ToArray();
        }

        public byte[] SetRequest(DLMSObject cosem, byte index, DLMSDataItem dataItem
        )
        {
            List<byte> setRequest = new List<byte>();
            PackagingDestinationAndSourceAddress(setRequest);
            int ctr = ((Hdlc46Frame.CurrentReceiveSequenceNumber << 1) + 1 << 4) +
                      (Hdlc46Frame.CurrentSendSequenceNumber << 1);
            IncreaseFrameSequenceNumber();
            setRequest.Add((byte) ctr);
            byte count;
            switch (dataItem.DataType)
            {
                case DataType.OctetString:
                    count = (byte) (2 + Hdlc46Frame.DestAddr.Length + 1 + 1 + 2 + 3 + 13 + dataItem.ValueBytes.Length +
                                    2 +
                                    2); //octetSting 多一个长度
                    break;
                default:
                    count = (byte) (2 + Hdlc46Frame.DestAddr.Length + 1 + 2 + 3 + 13 + dataItem.ValueBytes.Length + 2 +
                                    2); //octetSting 多一个长度
                    break;
            }


            Hdlc46Frame.FrameFormatField = new byte[]
            {
                (0xA0),
                Convert.ToByte(count)
            };
            setRequest.InsertRange(0, Hdlc46Frame.FrameFormatField);
            PackingHcs(setRequest);
            setRequest.AddRange(Hdlc46Frame.LlcHeadFrameBytes);

            setRequest.Add((byte) Command.SetRequest); //set - request[193] SET - Request,
            setRequest.Add((byte) SetRequestType.Normal);


            setRequest.Add(Hdlc46Frame.Apdu.InvokeIdAndPriority.InvokeIdAndPriority);

            setRequest.AddRange(BitConverter.GetBytes((ushort) cosem.ObjectType).Reverse()); //ClassId
            setRequest.AddRange(OBISHelper.ObisStringToBytes(cosem.LogicalName));
            setRequest.Add(index); //属性
            setRequest.Add(cosem.Version); //版本


            setRequest.Add((byte) dataItem.DataType);
            //TO DO 
            if (dataItem.DataType == DataType.OctetString) //这里要重写判断机制，根据数据类型
            {
                setRequest.Add((byte) dataItem.ValueBytes.Length);
            }

            setRequest.AddRange(dataItem.ValueBytes);


            PackingFcs_And_FrameStartEnd(setRequest);
            return setRequest.ToArray();
        }

        public byte[] ActionRequest(DLMSObject cosem, byte index, DLMSDataItem dataItem)
        {
            List<byte> actionRequest = new List<byte>();
            PackagingDestinationAndSourceAddress(actionRequest);
            int ctr = ((Hdlc46Frame.CurrentReceiveSequenceNumber << 1) + 1 << 4) +
                      (Hdlc46Frame.CurrentSendSequenceNumber << 1);
            IncreaseFrameSequenceNumber();
            actionRequest.Add((byte) ctr);
            byte count;
            switch (dataItem.DataType)
            {
                case DataType.OctetString:
                    count = (byte) (2 + Hdlc46Frame.DestAddr.Length + 1 + 1 + 2 + 3 + 13 + dataItem.ValueBytes.Length +
                                    2 +
                                    2); //octetSting 多一个长度,结构不一样
                    break;
                default:
                    count = (byte) (2 + Hdlc46Frame.DestAddr.Length + 1 + 2 + 3 + 13 + dataItem.ValueBytes.Length + 2 +
                                    2);
                    break;
            }

            Hdlc46Frame.FrameFormatField = new byte[] {0xA0, count};
            actionRequest.InsertRange(0, Hdlc46Frame.FrameFormatField);
            PackingHcs(actionRequest);
            actionRequest.AddRange(Hdlc46Frame.LlcHeadFrameBytes);
            //pdu
            actionRequest.Add((byte) Command.MethodRequest);
            //TODO 根据请求类型需要做相关判断
            //ActionRequestType type = new ActionRequestType();
            //switch (type)
            //{
            //    case ActionRequestType.Normal:
            //        actionRequest.Add((byte)ActionRequestType.Normal);
            //        var cosemMethodDescriptorInner = new CosemMethodDescriptor(cosem, index);//

            //        actionRequest.AddRange(cosemMethodDescriptorInner.ToPduBytes());

                    
            //        actionRequest.AddRange(dataItem.ToPduBytes());
            //        break;
            //    case ActionRequestType.NextBlock: break;
            //    case ActionRequestType.WithList: break;
            //    case ActionRequestType.WithFirstBlock: break;
            //    case ActionRequestType.WithListAndFirstBlock: break;
            //    case ActionRequestType.WithBlock: break;
            //}
            actionRequest.Add((byte) ActionRequestType.Normal);

            actionRequest.Add(Hdlc46Frame.Apdu.InvokeIdAndPriority.InvokeIdAndPriority);


            var cosemMethodDescriptor = new CosemMethodDescriptor(cosem, index);//

            actionRequest.AddRange(cosemMethodDescriptor.ToPduBytes());

            actionRequest.AddRange(dataItem.ToPduBytes());

            PackingFcs_And_FrameStartEnd(actionRequest);
            return actionRequest.ToArray();
        }


        /// <summary>
        /// 进入升级模式
        /// </summary>
        /// <param name="inputBytes"></param>
        /// <returns></returns>
        public byte[] SetEnterUpGradeMode(ushort id)
        {
            //7E A0 10 03 03 32 8F 31 E6 E6 00 FF 10 01 00 C4 08 7E 
            List<byte> enterUpgradeMode = new List<byte>();
            PackagingDestinationAndSourceAddress(enterUpgradeMode);
            int ctr = ((Hdlc46Frame.CurrentReceiveSequenceNumber << 1) + 1 << 4) +
                      (Hdlc46Frame.CurrentSendSequenceNumber << 1);
            IncreaseFrameSequenceNumber();
            enterUpgradeMode.Add((byte) ctr);
            byte count = (byte) (2 + Hdlc46Frame.DestAddr.Length + 2 + 3 + 2 + 2 + 2 + 2);

            Hdlc46Frame.FrameFormatField = new byte[]
            {
                160,
                Convert.ToByte(count)
            };
            enterUpgradeMode.InsertRange(0, Hdlc46Frame.FrameFormatField);
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
            byte count = (byte) (2 + Hdlc46Frame.DestAddr.Length + 1 + 1 + 2);
            Hdlc46Frame.FrameFormatField = new byte[]
            {
                160,
                Convert.ToByte(count)
            };
            rr.InsertRange(0, Hdlc46Frame.FrameFormatField);
            PackingFcs_And_FrameStartEnd(rr);
            return rr.ToArray();
        }

        public byte[] ReceiveNotReady()
        {
            List<byte> rnrListBytes = new List<byte>();
            PackagingDestinationAndSourceAddress(rnrListBytes);
            int ctr = ((Hdlc46Frame.CurrentReceiveSequenceNumber << 1) + 1 << 4) + 5;
            rnrListBytes.Add((byte) ctr);
            byte count = (byte) (2 + Hdlc46Frame.DestAddr.Length + 1 + 1 + 2);
            Hdlc46Frame.FrameFormatField = new byte[]
            {
                160,
                Convert.ToByte(count)
            };
            rnrListBytes.InsertRange(0, Hdlc46Frame.FrameFormatField);
            PackingFcs_And_FrameStartEnd(rnrListBytes);
            return rnrListBytes.ToArray();
        }

        private void PackagingDestinationAndSourceAddress(List<byte> bytes)
        {
            bytes.AddRange(Hdlc46Frame.DestAddr);
            bytes.Add(Hdlc46Frame.SourceAddr);
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
            bytes.Insert(0, Hdlc46Frame.HDLCFrameStartEnd); //添加帧头
            bytes.Add(Hdlc46Frame.HDLCFrameStartEnd); //添加帧尾
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