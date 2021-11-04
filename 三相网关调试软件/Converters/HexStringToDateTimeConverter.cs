using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.CosemObjects;
using MyDlmsStandard.BusinessDefine;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using 三相智慧能源网关调试软件.Common;

namespace 三相智慧能源网关调试软件.Converters
{
    public class IndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            ListBoxItem item = (ListBoxItem)value;
            ListBox listView = ItemsControl.ItemsControlFromItemContainer(item) as ListBox;
            int index = listView.ItemContainerGenerator.IndexFromContainer(item) + 1;
            return index.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class HexStringToDateTimeConverter : BaseConverter<HexStringToDateTimeConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var t = value as DlmsDataItem;
            if (t == null)
            {
                return value;
            }

            if (t.ValueName == "Clock  time")
            {
                var clock = new CosemClock();
                string str = t.ValueString.ToString();
                try
                {
                    var b = clock.DlmsClockParse(str.StringToByte());
                    if (b)
                    {
                        return clock.ToString();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                return str;
            }

            var name = t.ValueName.Replace(" ", "").ToLower();
            if (name.Contains("eventcode"))
            {
                try
                {
                    var standardEventCode = (StandardEventCode)int.Parse(t.ValueString);
                    return t?.ValueString + ":" + standardEventCode.ToString();
                }
                catch (Exception e)
                {
                    return t?.ValueString;
                }
            }

            return t?.ValueString;
        }
    }
}