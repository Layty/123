using System.Text;
using MySerialPortMaster;
using 三相智慧能源网关调试软件.DLMS.Common;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay
{
    //public class DLMSCompactArray:IToPduStringInHex,IPduStringInHexConstructor
    //{
    //    public DLMSDataItem[] Items { get; set; }
    //    public DLMSDataItem ContentDescription { get; set; }
    //    public string ToPduStringInHex()
    //    {
    //        if (ContentDescription == null)
    //        {
    //            if (Items == null || Items.Length == 0)
    //            {
    //                return "";
    //            }
    //            ContentDescription = Items[0];
    //        }
    //        StringBuilder stringBuilder = new StringBuilder();
    //        stringBuilder.Append("13");
    //        stringBuilder.Append(GetContentDescription(ContentDescription));
    //        StringBuilder stringBuilder2 = new StringBuilder();
    //        DlmsDataItem[] array = items;
    //        foreach (DlmsDataItem dlmsDataItem in array)
    //        {
    //            stringBuilder2.Append(dlmsDataItem.ToPduStringInHexWithinCompactArray());
    //        }
    //        int qty = stringBuilder2.ToString().Length / 2;
    //        stringBuilder.Append(MyConvert.EncodeVarLength(qty));
    //        stringBuilder.Append(stringBuilder2.ToString());
    //        return stringBuilder.ToString();
    //    }

    //    public bool PduStringInHexConstructor(ref string pduStringInHex)
    //    {
    //        throw new System.NotImplementedException();
    //    }
    //}
    public class DLMSArray:IToPduBytes,IToPduStringInHex,IPduStringInHexConstructor
    {
        public DLMSDataItem[] items { get; set; }

        public byte[] ToPduBytes()
        {
            string str = "01";
            string str2 = (items.Length <= 127) ? items.Length.ToString("X2") : ((items.Length > 255) ? ("82" + items.Length.ToString("X4")) : ("81" + items.Length.ToString("X2")));
            StringBuilder stringBuilder = new StringBuilder();
            DLMSDataItem[] array = items;
            foreach (DLMSDataItem dlmsDataItem in array)
            {
                stringBuilder.Append(dlmsDataItem.ToPduBytes());
            }
            return (str + str2 + stringBuilder.ToString()).StringToByte();
        }

        public string ToPduStringInHex()
        {
            string str = "01";
            string str2 = (items.Length <= 127) ? items.Length.ToString("X2") : ((items.Length > 255) ? ("82" + items.Length.ToString("X4")) : ("81" + items.Length.ToString("X2")));
            StringBuilder stringBuilder = new StringBuilder();
            DLMSDataItem[] array = items;
            foreach (DLMSDataItem dlmsDataItem in array)
            {
                stringBuilder.Append(dlmsDataItem.ToPduBytes());
            }
            return (str + str2 + stringBuilder.ToString());
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            int num = MyConvert.DecodeVarLength(ref pduStringInHex);
            items = new DLMSDataItem[num];
            for (int i = 0; i < num; i++)
            {
                items[i] = new DLMSDataItem();
                if (!items[i].PduStringInHexConstructor(ref pduStringInHex))
                {
                    return false;
                }
            }
            return true;
        }
    }
}