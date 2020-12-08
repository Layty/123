using System;
using System.Xml.Serialization;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Ber;

namespace MyDlmsStandard.ApplicationLay.Association
{
    public class AssociationResponse 
    {
      [XmlIgnore] public Command Command { get; set; } = Command.Aare;

        public BerBitString ProtocolVersion { get; set; }
        public ApplicationContextName ApplicationContextName { get; set; }
        public BerInteger AssociationResult { get; set; }
        
        public ResultSourceDiagnostic ResultSourceDiagnostic { get; set; }
        public BerOctetString RespondingAPTitle { get;set; }
        public BerBitString ResponderAcseRequirements { get; set; }
        public MechanismName MechanismName { get; set; }
        public AuthenticationValue RespondingAuthenticationValue { get; set; }

        public BerOctetString UserInformation { get; set; }
		public bool PduStringInHexConstructor(ref string pduStringInHex)
		{
			string a = pduStringInHex.Substring(0, 2);
			if (a != "61")
			{
				return false;
			}
			int num = Convert.ToInt32(pduStringInHex.Substring(2, 2), 16);
			if (num < 16 || num * 2 + 2 > pduStringInHex.Length)
			{
				return false;
			}
			pduStringInHex = pduStringInHex.Substring(4);
			while (!string.IsNullOrEmpty(pduStringInHex))
			{
				a = pduStringInHex.Substring(0, 2);
				pduStringInHex = pduStringInHex.Substring(2);
				switch (Convert.ToInt32(a, 16) & 0x1F)
				{
					case 0:
						ProtocolVersion = new BerBitString();
						if (!ProtocolVersion.PduStringInHexConstructor(ref pduStringInHex))
						{
							return false;
						}
						break;
					case 1:
						ApplicationContextName = new ApplicationContextName();
						if (!ApplicationContextName.PduStringInHexConstructor(ref pduStringInHex))
						{
							return false;
						}
						break;
					case 2:
                        AssociationResult = new BerInteger();
						if (!pduStringInHex.StartsWith("0302"))
						{
							return false;
						}
						pduStringInHex = pduStringInHex.Substring(4);
						if (!AssociationResult.PduStringInHexConstructor(ref pduStringInHex))
						{
							return false;
						}
						break;
					case 3:
						ResultSourceDiagnostic = new ResultSourceDiagnostic();
						if (!ResultSourceDiagnostic.PduStringInHexConstructor(ref pduStringInHex))
						{
							return false;
						}
						break;
					case 4:
						pduStringInHex = pduStringInHex.Substring(2);
						if (pduStringInHex.StartsWith("04"))
						{
							pduStringInHex = pduStringInHex.Substring(2);
							RespondingAPTitle = new BerOctetString();
							if (!RespondingAPTitle.PduStringInHexConstructor(ref pduStringInHex))
							{
								return false;
							}
							break;
						}
						return false;
					case 8:
						ResponderAcseRequirements = new BerBitString();
						if (!ResponderAcseRequirements.PduStringInHexConstructor(ref pduStringInHex))
						{
							return false;
						}
						break;
					case 9:
						MechanismName = new MechanismName();
						if (!MechanismName.PduStringInHexConstructor(ref pduStringInHex))
						{
							return false;
						}
						break;
					case 10:
						RespondingAuthenticationValue = new AuthenticationValue();
						if (!RespondingAuthenticationValue.PduStringInHexConstructor(ref pduStringInHex))
						{
							return false;
						}
						break;
					case 30:
						UserInformation = new BerOctetString();
						if (!UserInformation.PduStringInHexConstructor(ref pduStringInHex))
						{
							return false;
						}
						break;
					default:
						return false;
				}
			}
			return true;
		}
	}
}