using System.Xml.Serialization;
using MyDlmsStandard.Common;

namespace MyDlmsStandard.ApplicationLay.Association
{
    public class MechanismName:IToPduBytes
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

        public byte[] ToPduBytes()
        {
            //var appApduContentList = new List<byte>();
            //appApduContentList.AddRange(new byte[]
            //{
            //    0x8B, //标签([11],IMPLICIT,Context -specific)的编码
            //    0x07 //标记组件的值域的长度的编码
            //});
            //appApduContentList.AddRange(new byte[]
            //{
            //    0x60,
            //    0x85,
            //    0x74,
            //    0x05,
            //    0x08,
            //    0x02,
            //    0x01 //OBJECTIDENTIFIER的值的编码: low-level-security-mechanism-name(1), high-level-security-mechanism-name(5)
            //});
       
           // return appApduContentList.ToArray();
            return ("8B"+ToPduStringInHex()).StringToByte();
        }
    }
}