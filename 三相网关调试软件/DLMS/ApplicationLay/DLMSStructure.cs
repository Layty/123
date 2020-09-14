using System.Collections.Generic;
using System.Text;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS.Common;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay
{
    public class DlmsStructure:IToPduBytes,IPduBytesToConstructor
    {
        public DLMSDataItem[] Items { get;set; }

        public DlmsStructure()
        {
            
        }
        public DlmsStructure(DLMSDataItem[] items)
        {
            Items = items;
        }
        public byte[] ToPduBytes()
        {
            List<byte> list=new List<byte>();
            list.Add(0x02);
            if (Items.Length>127)
            {
                list.Add(0x00);
                list.Add(0x82);
            }
            else
            {
                list.Add((byte)Items.Length);
            }

            foreach (var dlmsDataItem in Items)
            {
                list.AddRange(dlmsDataItem.ToPduBytes());
            }
            return list.ToArray();
        }

        public bool PduBytesToConstructor(byte[] pduBytes)
        {
            var pduStringInHex = pduBytes.ByteToString("");
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
            return str + str2 + stringBuilder.ToString();
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