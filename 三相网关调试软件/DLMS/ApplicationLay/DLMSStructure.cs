using System.Collections.Generic;
using System.Text;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS.Common;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay
{
//    public class DlmsCompactArray:IToPduStringInHex,IPduStringInHexConstructor
//    {
//        public DLMSDataItem[] Items { get; set; }
//        [XmlIgnore] public DLMSDataItem ContentDescription { get; set; }
//
//        public string ToPduStringInHex()
//        {
//            if (ContentDescription == null)
//            {
//                if (Items == null || Items.Length == 0)
//                {
//                    return "";
//                }
//                ContentDescription = Items[0];
//            }
//            StringBuilder stringBuilder = new StringBuilder();
//            stringBuilder.Append("13");
//            stringBuilder.Append(GetContentDescription(ContentDescription));
//            StringBuilder stringBuilder2 = new StringBuilder();
//            DLMSDataItem[] array = Items;
//            foreach (DLMSDataItem dlmsDataItem in array)
//            {
//                stringBuilder2.Append(dlmsDataItem.ToPduStringInHexWithinCompactArray());
//            }
//            int qty = stringBuilder2.ToString().Length / 2;
//            stringBuilder.Append(MyConvert.EncodeVarLength(qty));
//            stringBuilder.Append(stringBuilder2.ToString());
//            return stringBuilder.ToString();
//        }
//
//        public bool PduStringInHexConstructor(ref string pduStringInHex)
//        {
//            throw new System.NotImplementedException();
//        }
//    }

    public class DlmsStructure : IToPduBytes, IPduBytesToConstructor
    {
        public DLMSDataItem[] Items { get; set; }

        public DlmsStructure()
        {
        }

        public DlmsStructure(DLMSDataItem[] items)
        {
            Items = items;
        }

        public byte[] ToPduBytes()
        {
            List<byte> list = new List<byte>();
            list.Add(0x02);
            if (Items.Length > 127)
            {
                list.Add(0x00);
                list.Add(0x82);
            }
            else
            {
                list.Add((byte) Items.Length);
            }

            foreach (var dlmsDataItem in Items)
            {
                list.AddRange(MyConvert.OctetStringToByteArray(dlmsDataItem.ToPduStringInHex()));
            }

            return list.ToArray();
        }

        public bool PduBytesToConstructor(byte[] pduBytes)
        {
            var pduStringInHex = pduBytes.ByteToString();
            int num = MyConvert.DecodeVarLength(ref pduStringInHex);
            Items = new DLMSDataItem[num];
            for (int i = 0; i < num; i++)
            {
                Items[i] = new DLMSDataItem();
            }

            DLMSDataItem[] array = Items;
            foreach (DLMSDataItem dlmsDataItem in array)
            {
                if (!dlmsDataItem.PduStringInHexConstructor(ref pduStringInHex))
                {
                    return false;
                }
            }

            return true;
        }

        public string ToPduStringInHex()
        {
            string str = "02";
            string str2 = (Items.Length > 127) ? ("82" + Items.Length.ToString("X4")) : Items.Length.ToString("X2");
            StringBuilder stringBuilder = new StringBuilder();
            DLMSDataItem[] array = Items;
            foreach (DLMSDataItem dlmsDataItem in array)
            {
                stringBuilder.Append(dlmsDataItem.ToPduStringInHex());
            }

            return str + str2 + stringBuilder;
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            int num = MyConvert.DecodeVarLength(ref pduStringInHex);
            Items = new DLMSDataItem[num];
            for (int i = 0; i < num; i++)
            {
                Items[i] = new DLMSDataItem();
            }

            DLMSDataItem[] array = Items;
            foreach (DLMSDataItem dlmsDataItem in array)
            {
                if (!dlmsDataItem.PduStringInHexConstructor(ref pduStringInHex))
                {
                    return false;
                }
            }

            return true;
        }
    }
}