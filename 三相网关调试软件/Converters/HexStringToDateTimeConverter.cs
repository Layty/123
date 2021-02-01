using System;
using System.Globalization;
using MyDlmsStandard.ApplicationLay.CosemObjects;
using 三相智慧能源网关调试软件.Common;

namespace 三相智慧能源网关调试软件.Converters
{
    public class HexStringToDateTimeConverter : BaseConverter<HexStringToDateTimeConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var clock = new CosemClock();
            string str = value.ToString();
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
            return value;
        }
    }
}