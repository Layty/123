using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using ClassLibraryDLMS.DLMS.ApplicationLay.ApplicationLayEnums;

namespace ClassLibraryDLMS.DLMS.ApplicationLay.Association
{
    public class ApplicationContextName : IToPduBytes, IPduBytesToConstructor
    {
        [XmlAttribute] public string Value { get; set; } = "LN";
        public bool CipherSupported { get; set; }

        public byte[] ToPduBytes()
        {
            List<byte> appApduContentList = new List<byte>();
            appApduContentList.Add((byte) TranslatorGeneralTags.ApplicationContextName); //标签([1],Context-specific)的编码
            appApduContentList.Add(0x09); //标记组件值域长度的编码
            //  appApduContentList.Add(0x06); //appApduContentList.Add((byte)BerType.ObjectIdentifier); //application-context-name(OBJECTIDEN- TIFIER,Universal)选项的编码
            appApduContentList.Add((byte) BerType.ObjectIdentifier);
            appApduContentList.Add(0x07); //对象标识符的值域的长度的编码
            appApduContentList.AddRange(new byte[]
            {
                0x60,
                0x85,
                0x74,
                0x05,
                0x08,
                0x01,
                0x01 //0x01,0x03
            }); //对象标识符的值的编码
            return appApduContentList.ToArray();
        }


        public bool PduBytesToConstructor(byte[] pduBytes)
        {
            if (pduBytes.Length < 10)
            {
                return false;
            }

            if (pduBytes[0] != 0xA1) return false;
            var text = pduBytes.Skip(1).ToArray().ByteToString("");
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
    }
}