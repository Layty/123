using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using CommonServiceLocator;
using NLog;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.MyControl;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.Association
{
    internal enum TranslatorGeneralTags
    {
        ApplicationContextName = 161,
        NegotiatedQualityOfService = 48640,
        ProposedDlmsVersionNumber = 48641,
        ProposedMaxPduSize = 48642,
        ProposedConformance = 48643,
        VaaName = 48644,
        NegotiatedConformance = 48645,
        NegotiatedDlmsVersionNumber = 48646,
        NegotiatedMaxPduSize = 48647,
        ConformanceBit = 48648,
        ProposedQualityOfService = 48649,
        SenderACSERequirements = 138,
        ResponderACSERequirement = 136,
        RespondingMechanismName = 137,
        CallingMechanismName = 139,
        CallingAuthentication = 172,
        RespondingAuthentication = 0x80,
        AssociationResult = 162,
        ResultSourceDiagnostic = 163,
        ACSEServiceUser = 41729,
        ACSEServiceProvider = 41730,
        CallingAPTitle = 166,
        RespondingAPTitle = 164,
        DedicatedKey = 168,
        CallingAeInvocationId = 169,
        CalledAeInvocationId = 165,
        CallingAeQualifier = 167,
        CharString = 170,
        UserInformation = 171,
        RespondingAeInvocationId = 173
    }

    public class AssociationRequest : IToPduBytes
    {
        public ApplicationContextName ApplicationContextName { get; set; }
        public CallingAPTitle CallingAPTitle { get; set; }
        public SenderACSERequirements SenderACSERequirements { get; set; }
        public MechanismName MechanismName { get; set; }
        public CallingAuthenticationValue CallingAuthenticationValue { get; set; }

        public InitiateRequest InitiateRequest { get; set; }

        public AssociationRequest()
        {
        }

        public AssociationRequest(byte[] passWorld, ushort maxReceivePduSize, byte dlmsVersion,string systemTitle)
        {
            ApplicationContextName = new ApplicationContextName();
            CallingAPTitle = new CallingAPTitle(systemTitle);
            SenderACSERequirements = new SenderACSERequirements();
            MechanismName = new MechanismName();
            CallingAuthenticationValue = new CallingAuthenticationValue(passWorld);
            InitiateRequest = new InitiateRequest(maxReceivePduSize, dlmsVersion);
        }

        public AssociationRequest(MyDLMSSettings dlmsSettings)
        {
            ApplicationContextName = new ApplicationContextName();
            MechanismName = new MechanismName();
            SenderACSERequirements = new SenderACSERequirements();
            CallingAuthenticationValue = new CallingAuthenticationValue(dlmsSettings);
            CallingAPTitle = new CallingAPTitle(dlmsSettings.SystemTitle);
            InitiateRequest = new InitiateRequest(dlmsSettings);
        }

        private Logger Logger = LogManager.GetLogger("XML");

        public byte[] ToPduBytes()
        {
            List<byte> appApduAssociationRequest = new List<byte>();
            if (ApplicationContextName != null)
                appApduAssociationRequest.AddRange(ApplicationContextName.ToPduBytes());
            if (CallingAPTitle != null)
            {
                appApduAssociationRequest.AddRange(CallingAPTitle.ToPduBytes());
            }

            if (SenderACSERequirements != null)
            {
                appApduAssociationRequest.AddRange(SenderACSERequirements.ToPduBytes());
            }

            if (MechanismName != null)
            {
                appApduAssociationRequest.AddRange(MechanismName.ToPduBytes());
            }

            if (CallingAuthenticationValue != null)
            {
                appApduAssociationRequest.AddRange(CallingAuthenticationValue.ToPduBytes());
            }

            if (InitiateRequest != null)
            {
                appApduAssociationRequest.AddRange(InitiateRequest.ToPduBytes());
            }

            using (StringWriter stringWriter = new StringWriter())
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(AssociationRequest));
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                xmlSerializer.Serialize(stringWriter, this, ns);
                var loggg = ServiceLocator.Current.GetInstance<DMLSXMLLog>();
                loggg.XmlLog = stringWriter.ToString();
            }

            return appApduAssociationRequest.ToArray();
        }
    }


    public class ProposedConformance
    {
        public byte Value { get; set; }
    }

    public class ProposedDlmsVersionNumber
    {
        [XmlAttribute] public byte Value { get; set; }
    }

    public class SenderACSERequirements : IToPduBytes
    {
        [XmlAttribute] public string Value { get; set; } = "1";

        public byte[] ToPduBytes()
        {
            List<byte> list = new List<byte>();
            list.AddRange(new byte[]
            {
                0x8A, //acse-requirements 域 ([10],IMPLICIT, Context-specific)的标签的编码
                0x02, //标记组件的值域的长度的编码
                0x07, //BITSTRING 的 最 后 字 节 未 使 用 比 特 数 的编码
                0x80 //认证功能单元(0)的编码 注:需要重点关注,不同客户机之间的比特 数的编码可能会有所不同,但在 COSEM 语 境中,只有 BIT0设置为1(基于标识认证功 能单元的要求)
            });
            return list.ToArray();
        }
    }
}