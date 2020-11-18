using System.Collections.Generic;
using System.Xml.Serialization;

namespace MyDlmsStandard.ApplicationLay.Association
{
    public class SenderACSERequirements : IToPduBytes
    {
        [XmlAttribute] public string Value { get; set; } = "1";

        public byte[] ToPduBytes()
        {
            List<byte> list = new List<byte>();
            list.AddRange(new byte[]
            {
                0x8A, //acse-requirements 域 ([10],IMPLICIT, Context-specific)的标签的编码
                0x02, //标记组件的值域的长度的编码
                0x07, //BITSTRING 的 最 后 字 节 未 使 用 比 特 数 的编码
                0x80 //认证功能单元(0)的编码 注:需要重点关注,不同客户机之间的比特 数的编码可能会有所不同,但在 COSEM 语 境中,只有 BIT0设置为1(基于标识认证功 能单元的要求)
            });
            return list.ToArray();
        }
    }
}