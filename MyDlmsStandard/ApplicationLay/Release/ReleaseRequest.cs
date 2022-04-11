using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.Association;
using MyDlmsStandard.ApplicationLay.Get;
using MyDlmsStandard.Ber;
using MyDlmsStandard.Common;
using System.Text;
using System.Xml.Serialization;

namespace MyDlmsStandard.ApplicationLay.Release
{
    public class ReleaseRequest : IDlmsCommand, IToPduStringInHex, IPduStringInHexConstructor
    {
        [XmlIgnore] public Command Command { get; } = Command.ReleaseRequest;
        public BerInteger Reason { get; set; }

        public UserInformation UserInformation { get; set; }



        public string ToPduStringInHex()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (Reason != null)
            {
                stringBuilder.Append("80");
                stringBuilder.Append(Reason.ToPduStringInHex());
            }

            if (UserInformation != null)
            {
                string str = UserInformation.ToPduBytes().ByteToString();
                stringBuilder.Append(str);
            }

            return "62" + (stringBuilder.Length / 2).ToString("X2") + stringBuilder.ToString();
        }
        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            if (string.IsNullOrEmpty(pduStringInHex))
            {
                return false;
            }
            string command = pduStringInHex.Substring(0, 2);
            if (command != "62")
            {
                return false;
            }
            return true;
        }

        //        public void TransferInitiateReqToUiClean()
        //        {
        //            if (InitiateRequest != null)
        //            {
        //                TransferInitiateReqToUi(InitiateRequest.ToPduStringInHex());
        //            }
        //        }

        //        public void TransferInitiateReqToUiCipher(string irTag, string cipherText)
        //        {
        //            BerOctetString berOctetString = new BerOctetString();
        //            berOctetString.Value = cipherText;
        //            TransferInitiateReqToUi(irTag + berOctetString.ToPduStringInHex());
        //        }

        //        private void TransferInitiateReqToUi(string irText)
        //        {
        //            BerOctetString berOctetString = new BerOctetString();
        //            berOctetString.Value = irText;
        //            string str = berOctetString.ToPduStringInHex();
        //            UserInformation = new BerOctetString();
        //            UserInformation.Value = "04" + str;
        //        }
    }
}