using System.Collections.Generic;
using 三相智慧能源网关调试软件.DLMS.HDLC;

namespace 三相智慧能源网关调试软件.DLMS
{
    class ApplicationContextName : IToPduBytes
    {
        public string Value { get; set; } = "LN";
        public bool CipherSupported { get; set; }

        public byte[] ToPduBytes()
        {
            List<byte> appApduContentList = new List<byte>();
            appApduContentList.Add(0xA1); //标签([1],Context-specific)的编码
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

        //public string ToPduStringInHex()
        //{
        //    if (Value.ToUpper() == "LN")
        //    {
        //        if (CipherSupported)
        //        {
        //            return "09060760857405080103";
        //        }
        //        return "09060760857405080101";
        //    }
        //    if (Value.ToUpper() == "SN")
        //    {
        //        if (CipherSupported)
        //        {
        //            return "09060760857405080104";
        //        }
        //        return "09060760857405080102";
        //    }
        //    return "";
        //}
    }
}