using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Common;
using System.Text;

namespace MyDlmsStandard.ApplicationLay
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

    public class DlmsStructure : IPduStringInHexConstructor, IToPduStringInHex, IDataType
    {
        public DataType DataType { get; } = DataType.Structure;
        public DlmsDataItem[] Items { get; set; }

        public DlmsStructure()
        {
        }

        public DlmsStructure(DlmsDataItem[] items)
        {
            Items = items;
        }

        public string ToPduStringInHex()
        {
            string str = "02";
            string str2 = (Items.Length > 127) ? ("82" + Items.Length.ToString("X4")) : Items.Length.ToString("X2");
            StringBuilder stringBuilder = new StringBuilder();
            DlmsDataItem[] array = Items;
            foreach (DlmsDataItem dlmsDataItem in array)
            {
                stringBuilder.Append(dlmsDataItem.ToPduStringInHex());
            }

            return str + str2 + stringBuilder;
        }

        /// <summary>
        /// 包含类型数据类型
        /// </summary>
        /// <param name="pduStringInHex"></param>
        /// <returns></returns>
        public virtual bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            pduStringInHex = pduStringInHex.Substring(2);
            int num = MyConvert.DecodeVarLength(ref pduStringInHex);
            Items = new DlmsDataItem[num];
            for (int i = 0; i < num; i++)
            {
                Items[i] = new DlmsDataItem();
            }

            DlmsDataItem[] array = Items;
            foreach (DlmsDataItem dlmsDataItem in array)
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