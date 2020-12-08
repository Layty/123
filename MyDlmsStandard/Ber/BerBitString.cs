using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MyDlmsStandard.Common;

namespace MyDlmsStandard.Ber
{
    public class BerGraphicString
    {
        [XmlAttribute]
        public string Value { get; set; }

        public string ToPduStringInHex()
        {
            if (Value.Length % 2 != 0)
            {
                return "";
            }
            return (Value.Length / 2).ToString("X2") + Value;
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            int num = Convert.ToInt32(pduStringInHex.Substring(0, 2), 16);
            if (num * 2 + 2 > pduStringInHex.Length)
            {
                return false;
            }
            pduStringInHex = pduStringInHex.Substring(2);
            Value = pduStringInHex.Substring(0, num * 2);
            pduStringInHex = pduStringInHex.Substring(num * 2);
            return true;
        }
    }
    public class BerBitString:IToPduStringInHex,IPduStringInHexConstructor
    {
        [XmlAttribute] public string Value { get; set; }

        public BerBitString()
        {
        }

        public BerBitString(string value)
        {
            Value = value;
        }

        public BerBitString(byte[] value)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte value2 in value)
            {
                DLMSCommon.ToBitString(stringBuilder, value2, 8);
            }

            Value = stringBuilder.ToString();
        }

        public BerBitString(byte[] value, int index, int count)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte value2 in value)
            {
                if (index != 0)
                {
                    index--;
                    continue;
                }

                if (count < 1)
                {
                    break;
                }

                DLMSCommon.ToBitString(stringBuilder, value2, count);
                count -= 8;
            }

            Value = stringBuilder.ToString();
        }

        public BerBitString(byte value, int count)
        {
            StringBuilder stringBuilder = new StringBuilder();
            DLMSCommon.ToBitString(stringBuilder, value, 8);
            if (count != 8)
            {
                stringBuilder.Remove(count, 8 - count);
            }

            Value = stringBuilder.ToString();
        }

        public string ToPduStringInHex()
        {
            List<byte> list = new List<byte>();
            byte[] array = new byte[8]
            {
                128,
                64,
                32,
                16,
                8,
                4,
                2,
                1
            };
            byte b = 0;
            int num = 0;
            foreach (var t in Value)
            {
                if (t == '0' || t == '1')
                {
                    if (num == 8)
                    {
                        list.Add(b);
                        b = 0;
                        num = 0;
                    }

                    if (t == '1')
                    {
                        b = (byte) (b | array[num]);
                    }

                    num++;
                    continue;
                }

                return null;
            }

            if (num > 0)
            {
                list.Add(b);
            }

            list.Insert(0, (byte) (list.Count + 1));
            list.Insert(1, (byte) (8 - num));
            return (list.ToArray().ByteToString());
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            string text = pduStringInHex;
            try
            {
                string text2 = pduStringInHex.Substring(0, 2);
                int num = Convert.ToInt32(text2, 16) - 1;
                string text3 = pduStringInHex.Substring(2, 2);
                int num2 = Convert.ToInt32(text3, 16);
                pduStringInHex = pduStringInHex.Substring(4);
                string text4 = pduStringInHex.Substring(0, num * 2);
                pduStringInHex = pduStringInHex.Substring(num * 2);
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < num; i++)
                {
                    string text5 = text4.Substring(i * 2, 2);
                    int num3 = Convert.ToInt32(text5, 16);
                    int num4 = 8;
                    if (i == num - 1)
                    {
                        num4 = 8 - num2;
                    }

                    while (num4 > 0)
                    {
                        stringBuilder.Append(((num3 & 0x80) != 0) ? "1" : "0");
                        num3 = (num3 & 0x7F) << 1;
                        num4--;
                    }
                }

                Value = stringBuilder.ToString();
                return true;
            }
            catch
            {
                pduStringInHex = text;
                return false;
            }
        }

      
    }
}