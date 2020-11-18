using System;
using System.Collections.Generic;
using System.Linq;
using MyDlmsStandard.Ber;
using MyDlmsStandard.Common;

namespace MyDlmsStandard.ApplicationLay.Association
{
    public class ResultSourceDiagnostic : IToPduBytes, IPduBytesToConstructor

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

        public byte[] ToPduBytes()
        {
            List<byte> list = new List<byte>();
            if (AcseServiceUser != null)
            {
                list.AddRange(new byte[] {0xA1, 0x03, 0x02});
                list.AddRange(AcseServiceUser.ToPduStringInHex().StringToByte());
                list.Insert(0, (byte) list.Count);
            }

            if (AcseServiceProvider != null)
            {
                list.AddRange(new byte[] {0xA2, 0x03, 0x02});
                list.AddRange(AcseServiceProvider.ToPduStringInHex().StringToByte());
                list.Insert(0, (byte) list.Count);
            }

            return list.ToArray();
        }

        public bool PduBytesToConstructor(byte[] pduBytes)
        {
            if (pduBytes[0] != 0xA3) return false;
            if (pduBytes[1] > pduBytes.Length - 2) return false;
            pduBytes = pduBytes.Skip(2).ToArray();
            var pdustring = pduBytes.ToArray().ByteToString("");
            if (pdustring.StartsWith("A10302"))
            {
                var data= pduBytes.Skip(3).ToArray();
               var datastring = data.ByteToString("");
                AcseServiceUser =new BerInteger();
                return AcseServiceUser.PduStringInHexConstructor(ref datastring);
            }

            if (pdustring.StartsWith("A20302"))
            {
                var data = pduBytes.Skip(3).ToArray();
                var datastring = data.ByteToString("");
                AcseServiceUser = new BerInteger();
                return AcseServiceProvider.PduStringInHexConstructor(ref datastring);
            }

            return false;
        }
     
    }
}