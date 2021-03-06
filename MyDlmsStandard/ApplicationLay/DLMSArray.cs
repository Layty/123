using MyDlmsStandard.Annotations;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Common;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace MyDlmsStandard.ApplicationLay
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

    interface IDataType
    {
        DataType DataType { get; }
    }

    public class DLMSArray : IDataType, INotifyPropertyChanged
    {
        public DataType DataType { get; } = DataType.Array;

        public DlmsDataItem[] Items
        {
            get => _Items;
            set { _Items = value; OnPropertyChanged(); }
        }
        private DlmsDataItem[] _Items;



        public string ToPduStringInHex()
        {
            string str = "01";
            string str2 = Items.Length <= 127
                ? Items.Length.ToString("X2")
                : Items.Length > 255
                    ? "82" + Items.Length.ToString("X4")
                    : "81" + Items.Length.ToString("X2");
            StringBuilder stringBuilder = new StringBuilder();
            DlmsDataItem[] array = Items;
            foreach (DlmsDataItem dlmsDataItem in array)
            {
                stringBuilder.Append(dlmsDataItem.ToPduStringInHex());
            }

            return str + str2 + stringBuilder;
        }

        public bool PduStringInHexConstructor(ref string pduStringInHex)
        {
            if (pduStringInHex.Substring(0, 2) != "01")
            {
                return false;
            }

            pduStringInHex = pduStringInHex.Substring(2);
            int num = MyConvert.DecodeVarLength(ref pduStringInHex);
            Items = new DlmsDataItem[num];
            for (int i = 0; i < num; i++)
            {
                Items[i] = new DlmsDataItem();
                if (!Items[i].PduStringInHexConstructor(ref pduStringInHex))
                {
                    return false;
                }
            }

            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}