using System.Collections.Generic;

namespace MyDlmsNetCore.ApplicationLay.Association
{
    public class CallingAuthenticationValue:IToPduBytes
    {
        private byte[] passwordHex;

        public CallingAuthenticationValue()
        {
            
        }
        public CallingAuthenticationValue(byte[] passwordHex)
        {
            this.passwordHex = passwordHex;
        }

        public byte[] ToPduBytes()
        {
            List<byte> appApduContentList = new List<byte>();
            appApduContentList.Add((byte)TranslatorGeneralTags.CallingAuthentication); //标签([12],EXPLICIT, Context-specific)的编码
            appApduContentList.Add(0x0A); //标记组件的值域的长度的编码
            appApduContentList.AddRange(new byte[]
            {
                0x80, //Authentication-value(charstring[0]IM- PLICITGraphicString)选项的编码
                0x08 //Authentication-value值 域 长 度 的 编 码 (8 字节)
            });
            appApduContentList.AddRange(passwordHex);
            return appApduContentList.ToArray();
        }
    }
}