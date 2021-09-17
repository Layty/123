using System;
using System.Globalization;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.CosemObjects;
using 三相智慧能源网关调试软件.Common;
using 三相智慧能源网关调试软件.Model;

namespace 三相智慧能源网关调试软件.Converters
{
    public class HexStringToDateTimeConverter : BaseConverter<HexStringToDateTimeConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var t = value as DlmsDataItem;
            if (t == null)
            {
                return value;
            }

            if (t?.ValueName == "Clock  time")
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

            var name = t?.ValueName.Replace(" ", "").ToLower();
            if (name.Contains("eventcode"))
            {
                try
                {
                    var standardEventCode = (StandardEventCode) int.Parse(t.ValueString);
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