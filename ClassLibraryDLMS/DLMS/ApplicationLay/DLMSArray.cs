using System.Text;
using ClassLibraryDLMS.DLMS.ApplicationLay.ApplicationLayEnums;
using ClassLibraryDLMS.DLMS.Common;

namespace ClassLibraryDLMS.DLMS.ApplicationLay
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
    public class DLMSArray : IToPduStringInHex, IPduStringInHexConstructor
    {
        private DataType DataType { get; set; } = DataType.Array;
        public DLMSDataItem[] Items { get; set; }


        public string ToPduStringInHex()
        {
            string str = "01";
            string str2 = (Items.Length <= 127)
                ? Items.Length.ToString("X2")
                : ((Items.Length > 255) ? ("82" + Items.Length.ToString("X4")) : ("81" + Items.Length.ToString("X2")));
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
                if (!Items[i].PduStringInHexConstructor(ref pduStringInHex))
                {
                    return false;
                }
            }

            return true;
        }
    }
}