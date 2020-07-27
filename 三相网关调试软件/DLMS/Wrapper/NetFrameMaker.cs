using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;



namespace 三相智慧能源网关调试软件.DLMS.Wrapper
{
    public class NetFrame
    {
        public byte[] Version { get; set; }
        public byte[] SourceAddress { get; set; }

        public byte[] DestAddress { get; set; }
    }

    public class NetFrameMaker : ObservableObject
    {
        private MyDLMSSettings MyDlmsSettings { get; set; }

        private NetFrame _netFrame;

        public NetFrameMaker(MyDLMSSettings settings)
        {
            MyDlmsSettings = settings;
            //DestAddress = BitConverter.GetBytes(settings.ServerAddress).Reverse().ToArray();
            //SourceAddress = BitConverter.GetBytes(settings.ClientAddress).Reverse().ToArray();
            //Version = new byte[] {0x00, 0x01};
           
        }


        private void PackagingDestinationAndSourceAddress(List<byte> bytes)
        {
            _netFrame = new NetFrame();
            _netFrame.Version = new byte[] { 0x00, 0x01 };
            _netFrame.SourceAddress = BitConverter.GetBytes( MyDlmsSettings .ClientAddress).Reverse().ToArray();
            _netFrame.DestAddress = BitConverter.GetBytes(MyDlmsSettings.ServerAddress).Reverse().ToArray();
            bytes.AddRange(_netFrame.Version);
            bytes.AddRange(_netFrame.SourceAddress);
            bytes.AddRange(_netFrame.DestAddress);
        }

        public byte[] AarqRequest()
        {
            List<byte> aarqList = new List<byte>();
            PackagingDestinationAndSourceAddress(aarqList);

            List<byte> appApduContentList = new List<byte>();

            #region ———application-context-name域 (application-context-name [1], OBJECT IDENTIFIER)

            //ApplicationContextName
            appApduContentList.AddRange(new ApplicationContextName().ToPduBytes());

            #endregion

            #region 认证功能单元的域的编码,只有在选择了身份验证功能单元时，才会出现以下字段。

            // bool flag = Hdlc46Frame.PasswordLvl1 == Hdlc46Frame.PasswordLvl.LLS;
            // bool flag = true;
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

                appApduContentList.AddRange(new CallingAuthenticationValue(MyDlmsSettings.PasswordHex).ToPduBytes());

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
            appApduContentList.AddRange(BitConverter.GetBytes(MyDlmsSettings.MaxReceivePduSize));

            appApduContentList.InsertRange(0, new byte[] {(byte) Command.Aarq, (byte) appApduContentList.Count});
            aarqList.AddRange(BitConverter.GetBytes((ushort) appApduContentList.Count).Reverse());
            aarqList.AddRange(appApduContentList);

            #endregion

            return aarqList.ToArray();
        }


        public byte[] BuildPduRequestBytes(byte[] pduBytes)
        {
            List<byte> getRequest = new List<byte>();
            PackagingDestinationAndSourceAddress(getRequest);
            List<byte> apduBytes = new List<byte>();
            apduBytes.AddRange(pduBytes);
            getRequest.AddRange(BitConverter.GetBytes((ushort) apduBytes.Count).Reverse());
            getRequest.AddRange(apduBytes);

            return getRequest.ToArray();
        }


        public byte[] ReleaseRequest()
        {
            List<byte> getRequest = new List<byte>();
            PackagingDestinationAndSourceAddress(getRequest);
            List<byte> apduBytes = new List<byte>();
            apduBytes.AddRange(new byte[] {(byte) Command.ReleaseRequest, 0x00});
            getRequest.AddRange(BitConverter.GetBytes((ushort) apduBytes.Count).Reverse());
            getRequest.AddRange(apduBytes);
            return getRequest.ToArray();
        }
    }
}