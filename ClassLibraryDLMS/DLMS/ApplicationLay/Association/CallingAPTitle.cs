using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace ClassLibraryDLMS.DLMS.ApplicationLay.Association
{
    /// <summary>
    /// Calling-AP-Title(carries the System title)
    /// </summary>
    public class CallingAPTitle : IToPduBytes, IPduBytesToConstructor
    {
        [XmlAttribute] public string Value { get; set; } = "00000000000000000000";

        public CallingAPTitle()
        {
            
        }

        public CallingAPTitle(string systemTitle)
        {
            Value = systemTitle;
        }
        public byte[] ToPduBytes()
        {
            List<byte> list2 = new List<byte>();
            list2.Add(0x04);
            list2.Add((byte) Value.StringToByte().Length);
            list2.AddRange(Value.StringToByte());

            List<byte> list = new List<byte>();
            list.Add(0xA6);
            list.Add((byte) list2.Count);
            list.AddRange(list2);
            return list.ToArray();
        }

        public bool PduBytesToConstructor(byte[] pduBytes)
        {
            var len = pduBytes[0];
            if (len != (pduBytes.Length - 1))
            {
                return false;
            }

            Value = pduBytes.Skip(1).ToArray().ByteToString().Trim();
            return true;
        }
    }
}