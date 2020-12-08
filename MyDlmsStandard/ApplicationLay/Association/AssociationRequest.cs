using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Axdr;
using MyDlmsStandard.Ber;


namespace MyDlmsStandard.ApplicationLay.Association
{
    public class AssociationRequest
    {
        [XmlIgnore] public Command Command => Command.Aarq;
        public BerBitString ProtocolVersion { get; set; }
        public ApplicationContextName ApplicationContextName { get; set; }
     
        public CallingAPTitle CallingApTitle { get; set; }
        public BerBitString SenderACSERequirements { get; set; }
        public MechanismName MechanismName { get; set; }
        public AuthenticationValue AuthenticationValue { get; set; }

        public BerOctetString UserInformation { get; set; }
        public InitiateRequest InitiateRequest { get; set; }

        public AssociationRequest()
        {
        }

        public AssociationRequest(byte[] passWorld, ushort maxReceivePduSize, byte dlmsVersion, string systemTitle,
            Conformance conformance)
        {
            ApplicationContextName = new ApplicationContextName();
            MechanismName = new MechanismName();
            SenderACSERequirements = new BerBitString();
            AuthenticationValue = new AuthenticationValue(passWorld);
            CallingApTitle = new CallingAPTitle(systemTitle);
            InitiateRequest = new InitiateRequest(maxReceivePduSize, dlmsVersion, conformance);
        }


  

        public string ToPduStringInHex()
        {
            StringBuilder stringBuilder=new StringBuilder();

            if (ProtocolVersion != null)
            {
                stringBuilder.Append("80" + ProtocolVersion.ToPduStringInHex());
            }
            if (ApplicationContextName != null)
            {
                stringBuilder.Append("A1" + ApplicationContextName.ToPduStringInHex());
            }
           
          
            if (SenderACSERequirements != null)
            {
                stringBuilder.Append("8A" + SenderACSERequirements.ToPduStringInHex());
            }
            if (MechanismName != null)
            {
                stringBuilder.Append("8B" + MechanismName.ToPduStringInHex());
            }
            if (AuthenticationValue != null)
            {
                stringBuilder.Append("AC" + AuthenticationValue.ToPduStringInHex());
            }
            if (UserInformation != null)
            {
                string str = UserInformation.ToPduStringInHex();
                stringBuilder.Append("BE" + str);
            }
            BerOctetString berOctetString3 = new BerOctetString();
            berOctetString3.Value = stringBuilder.ToString();
            return "60" + berOctetString3.ToPduStringInHex();
        }
    }
}