using System.Collections.Generic;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay
{
    /*
        联系邮箱：694965217@qq.com
        创建时间：2020/06/03 15:37:45
        主要用途：
        更改记录：
    */
    public class CallingAuthenticationValue:IToPduBytes
    {
        private byte[] passwordHex; 
        public CallingAuthenticationValue(byte[] passwordHex)
        {
            this.passwordHex = passwordHex;
        }
        public byte[] ToPduBytes()
        {
            List<byte> appApduContentList = new List<byte>();
            appApduContentList.Add(0xAC); //标签([12],EXPLICIT, Context-specific)的编码
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