using MyDlmsStandard.Ber;
using System.Text;
using System.Xml.Serialization;

namespace MyDlmsStandard.ApplicationLay.Association
{
    public class CalledApTitle : IToPduStringInHex
    {
        [XmlIgnore] public TranslatorGeneralTags TranslatorGeneralTags => TranslatorGeneralTags.AssociationResult;
        public string Value { get; set; }

        public string ToPduStringInHex()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("A6");

            BerOctetString berOctetString2 = new BerOctetString();
            berOctetString2.Value = Value;
            stringBuilder.Append(berOctetString2.ToPduStringInHex());

            return stringBuilder.ToString();
        }
    }
}