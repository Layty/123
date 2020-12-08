using System;
using System.Text;
using MyDlmsStandard.Ber;

namespace MyDlmsStandard.ApplicationLay.Association
{
    public class ResultSourceDiagnostic 
    {
        public BerInteger AcseServiceUser { get; set; }
        public BerInteger AcseServiceProvider { get; set; }

        public string GetDiagnosticMessage()
        {
            if (AcseServiceUser != null)
            {
                switch (Convert.ToInt32(AcseServiceUser.Value, 16))
                {
                    case 0:
                        return "";
                    case 1:
                        return "No reason given";
                    case 2:
                        return "Application context name not supported";
                    case 3:
                        return "Calling AP title not recognized";
                    case 4:
                        return "Calling AP invocation identifier not recognized";
                    case 5:
                        return "Calling AE qualifier not recognized";
                    case 6:
                        return "Calling AE invocation identifier not recognized";
                    case 7:
                        return "Called AP title not recognized";
                    case 8:
                        return "Called AP invocation identifier not recognized";
                    case 9:
                        return "Called AE qualifier not recognized";
                    case 10:
                        return "Called AE invocation identifier not recognized";
                    case 11:
                        return "Authentication mechanism name not recognised";
                    case 12:
                        return "Authentication mechanism name required";
                    case 13:
                        return "Authentication failure";
                    case 14:
                        return "Authentication required";
                    default:
                        return "Other reason";
                }
            }

            if (AcseServiceProvider != null)
            {
                switch (Convert.ToInt32(AcseServiceProvider.Value, 16))
                {
                    case 0:
                        return "";
                    case 1:
                        return "No reason given";
                    case 2:
                        return "No common acse version";
                    default:
                        return "Other reason";
                }
            }

            return "";
        }


        public string ToPduStringInHex()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (AcseServiceUser != null)
            {
                stringBuilder.Append("A1");
                stringBuilder.Append("03");
                stringBuilder.Append("02");
                stringBuilder.Append(AcseServiceUser.ToPduStringInHex());
                return stringBuilder.Length.ToString("X2") + stringBuilder.ToString();
            }
            if (AcseServiceProvider != null)
            {
                stringBuilder.Append("A2");
                stringBuilder.Append("03");
                stringBuilder.Append("02");
                stringBuilder.Append(AcseServiceProvider.ToPduStringInHex());
                return stringBuilder.Length.ToString("X2") + stringBuilder.ToString();
            }
            return "";
        }
        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            string value = pduStringInHex.Substring(0, 2);
            int num = Convert.ToInt32(value, 16);
            if (num * 2 + 2 > pduStringInHex.Length)
            {
                return false;
            }
            pduStringInHex = pduStringInHex.Substring(2);
            if (pduStringInHex.StartsWith("A10302"))
            {
                pduStringInHex = pduStringInHex.Substring(6);
                AcseServiceUser = new BerInteger();
                return AcseServiceUser.PduStringInHexConstructor(ref pduStringInHex);
            }
            if (pduStringInHex.StartsWith("A20302"))
            {
                pduStringInHex = pduStringInHex.Substring(6);
                AcseServiceProvider = new BerInteger();
                return AcseServiceProvider.PduStringInHexConstructor(ref pduStringInHex);
            }
            return false;
        }

    }
}