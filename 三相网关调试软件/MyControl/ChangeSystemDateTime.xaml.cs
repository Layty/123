using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

namespace 三相智慧能源网关调试软件.MyControl
{  /// <summary>
   /// 
   /// </summary>
    public struct SYSTEMTIME
    {
        public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfWeek;
        public ushort wDay;
        public ushort wHour;
        public ushort wMinute;
        public ushort wSecond;
        public ushort wMilliseconds;

        /// <summary>
        /// 从System.DateTime转换。
        /// </summary>
        /// <param name="time">System.DateTime类型的时间。</param>
        public void FromDateTime(DateTime time)
        {
            wYear = (ushort)time.Year;
            wMonth = (ushort)time.Month;
            wDayOfWeek = (ushort)time.DayOfWeek;
            wDay = (ushort)time.Day;
            wHour = (ushort)time.Hour;
            wMinute = (ushort)time.Minute;
            wSecond = (ushort)time.Second;
            wMilliseconds = (ushort)time.Millisecond;
        }
        /// <summary>
        /// 转换为System.DateTime类型。
        /// </summary>
        /// <returns></returns>
        public DateTime ToDateTime()
        {
            return new DateTime(wYear, wMonth, wDay, wHour, wMinute, wSecond, wMilliseconds);
        }
        /// <summary>
        /// 静态方法。转换为System.DateTime类型。
        /// </summary>
        /// <param name="time">SYSTEMTIME类型的时间。</param>
        /// <returns></returns>
        public static DateTime ToDateTime(SYSTEMTIME time)
        {
            return time.ToDateTime();
        }
    }
    public class Win32API
    {
        [DllImport("Kernel32.dll")]
        public static extern bool SetLocalTime(ref SYSTEMTIME Time);
        [DllImport("Kernel32.dll")]
        public static extern void GetLocalTime(ref SYSTEMTIME Time);
    }
    /// <summary>
    /// ChangeSystemDateTime.xaml 的交互逻辑
    /// </summary>
    public partial class ChangeSystemDateTime : Window
    {
        public ChangeSystemDateTime()
        {
            InitializeComponent();
            DatePicker.SelectedDate = DateTime.Now;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //取得当前系统时间
            DateTime t = DateTime.Now;

            if (Calendar.SelectedDate != null)
            {
                t = (DateTime)Calendar.SelectedDate;
            }
            //在当前时间上加上一周
            //    t = t.AddDays(7);
            //转换System.DateTime到SYSTEMTIME
            SYSTEMTIME st = new SYSTEMTIME();
            st.FromDateTime(t);
            //调用Win32 API设置系统时间
            var boo = Win32API.SetLocalTime(ref st);
            //显示当前时间

        }
    }
}
