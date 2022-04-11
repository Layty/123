using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.Association;
using MyDlmsStandard.ApplicationLay.Get;
using MyDlmsStandard.Ber;
using System;
using System.Xml.Serialization;

namespace MyDlmsStandard.ApplicationLay.Release
{
    public class ReleaseResponse : IDlmsCommand, IPduStringInHexConstructor
    {//00 01 00 01 00 01 00 05 63 03 80 01 00
     //		<ReleaseResponse>
     //  <Reason Value = "Normal" />
     //</ ReleaseResponse >

        [XmlIgnore] public Command Command { get; } = Command.ReleaseResponse;
        public BerInteger Reason { get; set; }
        public InitiateResponse UserInformation { get; set; }


        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            if (string.IsNullOrEmpty(pduStringInHex))
            {
                return false;
            }

            string command = pduStringInHex.Substring(0, 2);
            if (command != "63")
            {
                return false;
            }
            int num = Convert.ToInt32(pduStringInHex.Substring(2, 2), 16);
            if (num * 2 + 2 > pduStringInHex.Length)
            {
                return false;
            }
            string pduStringInHex2 = pduStringInHex.Substring(4, num * 2);
            pduStringInHex = pduStringInHex.Substring(4 + num * 2);
            while (pduStringInHex2.Length > 0)
            {
                switch (Convert.ToInt32(pduStringInHex2.Substring(0, 2), 16) & 0x1F)
                {
                    case 0:
                        pduStringInHex2 = pduStringInHex2.Substring(2);
                        Reason = new BerInteger();
                        if (!Reason.PduStringInHexConstructor(ref pduStringInHex2))
                        {
                            return false;
                        }
                        break;
                    case 30:
                        {
                            pduStringInHex2 = pduStringInHex2.Substring(2);
                            BerOctetString berOctetString = new BerOctetString();
                            if (!berOctetString.PduStringInHexConstructor(ref pduStringInHex2))
                            {
                                return false;
                            }
                            if (berOctetString.Value.StartsWith("04"))
                            {
                                string pduStringInHex3 = berOctetString.Value.Substring(2);
                                berOctetString = new BerOctetString();
                                if (!berOctetString.PduStringInHexConstructor(ref pduStringInHex3))
                                {
                                    return false;
                                }
                                pduStringInHex3 = berOctetString.Value;
                                UserInformation = new InitiateResponse();
                                if (!UserInformation.PduStringInHexConstructor(ref pduStringInHex3))
                                {
                                    return false;
                                }
                                break;
                            }
                            return false;
                        }
                }
            }
            return true;

        }
    }
}