using System.Linq;
using System.Text;
using System.Xml.Serialization;
using MyDlmsStandard.Common;

namespace MyDlmsStandard.ApplicationLay.Association
{
    public class ApplicationContextName :IPduStringInHexConstructor
    {
        [XmlAttribute] public string Value { get; set; } = "LN";
        [XmlAttribute] public TranslatorGeneralTags TranslatorGeneralTags => TranslatorGeneralTags.ApplicationContextName;
        public bool CipherSupported { get; set; }
        public string ToPduStringInHex()
        {
            StringBuilder stringBuilder=new StringBuilder();
  
            if (Value.ToUpper() == "LN")
            {
                if (CipherSupported)
                {
                    stringBuilder.Append( "09060760857405080103");
                }
                stringBuilder.Append ("09060760857405080101");
            }
            if (Value.ToUpper() == "SN")
            {
                if (CipherSupported)
                {
                    stringBuilder.Append  ("09060760857405080104");
                }
                stringBuilder.Append  ("09060760857405080102");
            }

            return stringBuilder.ToString();
        }
        

        public bool PduBytesToConstructor(byte[] pduBytes)
        {
            if (pduBytes.Length < 10)
            {
                return false;
            }

            if (pduBytes[0] != 0xA1) return false;
            var text = pduBytes.Skip(1).ToArray().ByteToString();
            if (text.StartsWith("090607608574050801"))
            {
                text = text.Substring(18, 2);
                switch (text)
                {
                    case "01":
                    case "03":
                        Value = "LN";
                        CipherSupported = (text == "03");
                        break;
                    case "02":
                    case "04":
                        Value = "SN";
                        CipherSupported = (text == "04");
                        break;
                    default:
                        return false;
                }

//                pduStringInHex = pduStringInHex.Substring(20);
                return true;
            }

            return false;
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            if (pduStringInHex.Length < 20)
            {
                return false;
            }
            string text = pduStringInHex.Substring(0, 20);
            if (text.StartsWith("090607608574050801"))
            {
                text = text.Substring(18, 2);
                switch (text)
                {
                    case "01":
                    case "03":
                        Value = "LN";
                        CipherSupported = (text == "03");
                        break;
                    case "02":
                    case "04":
                        Value = "SN";
                        CipherSupported = (text == "04");
                        break;
                    default:
                        return false;
                }
                pduStringInHex = pduStringInHex.Substring(20);
                return true;
            }
            return false;
        }
    }
}