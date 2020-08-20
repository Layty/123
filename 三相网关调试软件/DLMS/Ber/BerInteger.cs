using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;

namespace 三相智慧能源网关调试软件.DLMS.Ber
{
    public class BerInteger : IToPduBytes,IPduBytesToConstructor
    {
        [XmlAttribute] public string Value { get; set; }

        public string ToPduStringInHex()
        {
            if (Value.Length % 2 != 0)
            {
                return "";
            }

            return (Value.Length / 2).ToString("X2") + Value;
        }

        public bool PduStringInHexContructor(ref string pduStringInHex)
        {
            int num = Convert.ToInt32(pduStringInHex.Substring(0, 2), 16);
            if ((num + 1) * 2 > pduStringInHex.Length)
            {
                return false;
            }

            Value = pduStringInHex.Substring(2, num * 2);
            pduStringInHex = pduStringInHex.Substring((num + 1) * 2);
            return true;
        }

        public byte[] ToPduBytes()
        {
            List<byte> list = new List<byte>();
            var data = Value.StringToByte();
            list.Add((byte) data.Length);
            list.AddRange(data);
            return list.ToArray();
        }

        public bool PduBytesToConstructor(byte[] pduBytes)
        {
            if (pduBytes.Length<(pduBytes[0]+1))
            {
                return false;
            }

            Value = pduBytes.Skip(1).ToArray().ByteToString().Trim();
            return true;
        }
    }
}