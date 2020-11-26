using System.Linq;
using System.Xml.Serialization;
using MyDlmsNetCore.ApplicationLay.ApplicationLayEnums;
using MyDlmsNetCore.Ber;

namespace MyDlmsNetCore.ApplicationLay.Association
{
    public class AssociationResponse : IPduBytesToConstructor
    {
      [XmlIgnore] public Command Command { get; set; } = Command.Aare;

        public BerBitString ProtocolVersion { get; set; }
        public ApplicationContextName ApplicationContextName { get; set; }
        public AssociationResult AssociationResult { get; set; }
      
    
        public ResultSourceDiagnostic ResultSourceDiagnostic { get; set; }
        public BerOctetString RespondingAPTitle { get;set; }
        public BerBitString ResponderAcseRequirements { get; set; }
        public MechanismName MechanismName { get; set; }
        public CallingAuthenticationValue RespondingAuthenticationValue { get; set; }

        public InitiateResponse InitiateResponse { get; set; }
       // public BerOctetString UserInformation { get; set; }
        public bool PduBytesToConstructor(byte[] pduBytes)
        {
            if (pduBytes[0] != (byte) Command.Aare)
            {
                return false;
            }

            if (pduBytes[1] < 16 || pduBytes[1] + 1 > pduBytes.Length)
            {
                return false;
            }

            var data = pduBytes.Skip(2).ToArray();
            while (data.Length != 0)
            {
                var sw = data[0] & 0x1F;
                switch (sw)
                {
                    case 0:
                        
                        break;
                    case 1:
                        ApplicationContextName = new ApplicationContextName();
                        
                        if (!ApplicationContextName.PduBytesToConstructor(data.Take(11).ToArray()))
                        {
                            return false;
                        }

                        data = data.Skip(11).ToArray();
                        break;
                    case 2:
                        AssociationResult = new AssociationResult();
                        if (!AssociationResult.PduBytesToConstructor(data.Take(5).ToArray()))
                        {
                            return false;
                        }

                        data = data.Skip(5).ToArray();
                        break;
                    case 3:
                        ResultSourceDiagnostic=new ResultSourceDiagnostic();
                        if (!ResultSourceDiagnostic.PduBytesToConstructor(data.Take(7).ToArray()))
                        {
                            return false;
                        }
                        data = data.Skip(7).ToArray();
                        break;
                    case 30:
                        
                        InitiateResponse=new InitiateResponse();
                         if (!InitiateResponse.PduBytesToConstructor(data))
                        {
                            return false;
                        }
                        data = new byte[]{};
                        break;
                        
                }
            }

//            using (StringWriter stringWriter = new StringWriter())
//            {
//                XmlSerializer xmlSerializer = new XmlSerializer(typeof(AssociationResponse));
//                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
//                ns.Add("", "");
//                xmlSerializer.Serialize(stringWriter, this, ns);
//                var loggg = ServiceLocator.Current.GetInstance<DMLSXMLLog>();
//                loggg.XmlLog = stringWriter.ToString();
//            }
            return true;
        }
    }
}