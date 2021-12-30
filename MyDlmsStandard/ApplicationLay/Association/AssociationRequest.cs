using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Ber;
using MyDlmsStandard.Common;
using System;
using System.Text;
using System.Xml.Serialization;


namespace MyDlmsStandard.ApplicationLay.Association
{
    /// <summary>
    /// 协商请求-AARQ_0x60
    /// </summary>
    public class AssociationRequest : IToPduStringInHex
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
            SenderACSERequirements = new BerBitString()
            {
                Value = "1"
            };
            AuthenticationValue = new AuthenticationValue(passWorld)
            { CharString = new BerGraphicString() { Value = passWorld.ByteToString() } };
            CallingApTitle = new CallingAPTitle(systemTitle);
            InitiateRequest = new InitiateRequest(maxReceivePduSize, dlmsVersion, conformance);
            UserInformation = new BerOctetString() { Value = InitiateRequest.ToPduBytes().ByteToString() };
        }


        public string ToPduStringInHex()
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (ProtocolVersion != null)
            {
                stringBuilder.Append("80" + ProtocolVersion.ToPduStringInHex());
            }

            if (ApplicationContextName != null)
            {
                stringBuilder.Append("A1" + ApplicationContextName.ToPduStringInHex());
            }

            if (CallingApTitle != null)
            {
                stringBuilder.Append(CallingApTitle.ToPduBytes().ByteToString());
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
                stringBuilder.Append(InitiateRequest.ToPduBytes().ByteToString());
                //                string str = UserInformation.ToPduStringInHex();
                //                stringBuilder.Append("BE" + str);
            }

            BerOctetString berOctetString3 = new BerOctetString();
            berOctetString3.Value = stringBuilder.ToString();
            return "60" + berOctetString3.ToPduStringInHex();
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        { //采用最简单的判断60
            string a = pduStringInHex.Substring(0, 2);
            if (a != "60")
            {
                return false;
            }
            //int num = Convert.ToInt32(pduStringInHex.Substring(2, 2), 16);
            //if (num < 16 || num * 2 + 2 > pduStringInHex.Length)
            //{
            //    return false;
            //}
            //pduStringInHex = pduStringInHex.Substring(4);
            //while (!string.IsNullOrEmpty(pduStringInHex))
            //{
            //    a = pduStringInHex.Substring(0, 2);
            //    pduStringInHex = pduStringInHex.Substring(2);
            //    switch (Convert.ToInt32(a, 16) & 0x1F)
            //    {
            //        case 0:
            //            ProtocolVersion = new BerBitString();
            //            if (!ProtocolVersion.PduStringInHexConstructor(ref pduStringInHex))
            //            {
            //                return false;
            //            }
            //            break;
            //        case 1:
            //            ApplicationContextName = new ApplicationContextName();
            //            if (!ApplicationContextName.PduStringInHexConstructor(ref pduStringInHex))
            //            {
            //                return false;
            //            }
            //            break;
            //        //case 2:
            //        //    result = new BerInteger();
            //        //    if (!pduStringInHex.StartsWith("0302"))
            //        //    {
            //        //        return false;
            //        //    }
            //        //    pduStringInHex = pduStringInHex.Substring(4);
            //        //    if (!result.PduStringInHexContructor(ref pduStringInHex))
            //        //    {
            //        //        return false;
            //        //    }
            //        //    break;
            //        //case 3:
            //        //    resultSourceDiagnostic = new ResultSourceDiagnostic();
            //        //    if (!resultSourceDiagnostic.PduStringInHexContructor(ref pduStringInHex))
            //        //    {
            //        //        return false;
            //        //    }
            //        //    break;
            //        //case 4:
            //        //    pduStringInHex = pduStringInHex.Substring(2);
            //        //    if (pduStringInHex.StartsWith("04"))
            //        //    {
            //        //        pduStringInHex = pduStringInHex.Substring(2);
            //        //        respondingAPTitle = new BerOctetString();
            //        //        if (!respondingAPTitle.PduStringInHexContructor(ref pduStringInHex))
            //        //        {
            //        //            return false;
            //        //        }
            //        //        break;
            //        //    }
            //        //    return false;
            //        case 8:
            //            SenderACSERequirements = new BerBitString();
            //            if (!SenderACSERequirements.PduStringInHexConstructor(ref pduStringInHex))
            //            {
            //                return false;
            //            }
            //            break;
            //        case 9:
            //            MechanismName = new MechanismName();
            //            if (!MechanismName.PduStringInHexConstructor(ref pduStringInHex))
            //            {
            //                return false;
            //            }
            //            break;
            //        case 10:
            //            AuthenticationValue = new AuthenticationValue();
            //            if (!AuthenticationValue.PduStringInHexConstructor(ref pduStringInHex))
            //            {
            //                return false;
            //            }
            //            break;
            //        case 30:
            //            UserInformation = new BerOctetString();
            //            if (!UserInformation.PduStringInHexConstructor(ref pduStringInHex))
            //            {
            //                return false;
            //            }
            //            break;
            //        default:
            //            return false;
            //    }

            //}

            return true;
        }
    }
}