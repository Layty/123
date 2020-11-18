using System;
using System.Collections.Generic;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;

namespace MyDlmsStandard.ApplicationLay.Association
{
    public class InitiateRequest : IToPduBytes
    {
        public ProposedDlmsVersionNumber ProposedDlmsVersionNumber { get; set; }
        public Conformance ProposedConformance { get; set; }
        public ushort MaxReceivePduSize { get; set; }

        public InitiateRequest()
        {
        }

        public InitiateRequest(ushort maxReceivePduSize, byte proposedDlmsVersionNumber,
            Conformance proposedConformance)
        {
            ProposedDlmsVersionNumber = new ProposedDlmsVersionNumber();
            ProposedDlmsVersionNumber.Value = proposedDlmsVersionNumber;
            ProposedConformance = new Conformance();
            ProposedConformance = proposedConformance;
            MaxReceivePduSize = maxReceivePduSize;
        }


        public byte[] ToPduBytes()
        {
            List<byte> list = new List<byte>();
            list.Add(0xBE); //标签([30],Context-specific,Constructed) 的编码
            list.Add(0x10); //标记组件值域长度的编码 
            list.Add((byte) BerType.OctetString); //user-information(OCTET STRING,Uni- versal)选项的编码 BerType.OctetString
            list.Add(0x0E); // OCTETSTRING 值 域 长 度(14octets)的 编码

            list.AddRange(new byte[]
            {
                1, //DLMSAPDU 选项(InitiateRequest)的标签的编码 DLMSAPDUCHOICE(InitiateRequest)的标签的编码
                0, //专用密钥组件(OCTETSTRINGOPTIONAL)的编码使用标志(FALSE,不存在)
                0, //response-allowed组件(BOOLEANDEFAULTTRUE)的编码 /使用标志(FALSE,默认值为 TRUE输送)
                0, //proposed-quality-of-service组 件 ([0]IMPLICITInteger8 OP- TIONAL)的编码 使用标记(FALSE,不出现)
                6, //值为6,一个 Unsigned8的 A-XDR编码是它本身的值  ProposedDlmsVersionNumber proposed-dlms-version-number Unsigned8,
            });

            list.AddRange(new byte[]
            {
                0x5F,
                0x1F, //31  Conformance ::= [APPLICATION 31] BIT STRING --(SIZE(24)) //[APPLICATION31]标签的编码(ASN.1显示标签)b
                0x04, //contents域的8位元组(4)的长度的编码
                0x00, //BITSTRING最后字节未使用的比特数的编码
                0x00, 0x7F, 0x1F //定长 BITSTRING的值的编码  --ProposedConformance
                /*  0x04, 0xB0*/ //值为 0x04B0,一个 Unsigned16的编码是它本身的值  0x7E, 0x1F/7C FF
            }); //user-information:xDLMS InitiateRequestAPDU   0,  0 = 0x04,0xB0值为 0x04B0,一个 Unsigned16的编码是它本身的值
            list.AddRange(BitConverter.GetBytes(MaxReceivePduSize));
            return list.ToArray();
        }
    }
}