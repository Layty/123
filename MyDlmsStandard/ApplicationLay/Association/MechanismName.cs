using System;
using System.Xml.Serialization;

namespace MyDlmsStandard.ApplicationLay.Association
{
    public class MechanismName
    {
        [XmlAttribute] public string Value { get; set; } = "LS1";

        public string ToPduStringInHex()
        {
            string a = Value.ToUpper();
            if (a == "LS0")
            {
                return "";
            }
            if (a == "LS1")
            {
                return "0760857405080201";
            }
            if (a == "LS2")
            {
                return "0760857405080202";
            }
            if (a == "LS3")
            {
                return "0760857405080203";
            }
            if (a == "LS4")
            {
                return "0760857405080204";
            }
            if (a == "LS5")
            {
                return "0760857405080205";
            }
            return "";
        }
        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            if (pduStringInHex.Length < 16)
            {
                return false;
            }
            if (pduStringInHex.StartsWith("07608574050802"))
            {
                pduStringInHex = pduStringInHex.Substring(14);
                int num = Convert.ToInt32(pduStringInHex.Substring(0, 2), 16);
                pduStringInHex = pduStringInHex.Substring(2);
                Value = "LS" + num.ToString();
                return true;
            }
            return false;
        }
    }
}