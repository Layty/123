using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using DLMSMaster.ApplicationLay.Enums;

namespace 三相智慧能源网关调试软件.DLMS.CosemObjects
{
    public class DLMSClock : DLMSObject
    {
        public  string LogicalName { get; set; } = "0.0.1.0.0.255";
        public  ObjectType ObjectType { get; set; } = ObjectType.Clock;
        public int TimeZone { get; set; }
        public short Year { get; set; }
        public ClockStatus Status { get; set; }
        public int Deviation { get; set; }


        private sbyte daylightSavingsDeviation;

        private sbyte daylightSavingsDeviationToSet;

        private bool daylightSavingsEnabled;

        public byte Yearhighbyte { get; set; }

        public byte Yearlowbyte { get; set; }

        public byte Month { get; set; }


        public int Day { get; set; }


        public byte DayOfWeek { get; set; }

        public byte Hour { get; set; }


        public byte Minute { get; set; }


        public byte Second { get; set; }


        public byte Hundredths { get; set; }


        public string time { get; set; }


        public override string ToString()
        {
            string dateTimeformat = "yyyyMMddHHmmss";
            string tp = string.Concat(new string[]
            {
                this.Year.ToString().PadLeft(4, '0'),
                this.Month.ToString().PadLeft(2, '0'),
                this.Day.ToString().PadLeft(2, '0'),
                this.Hour.ToString().PadLeft(2, '0'),
                this.Minute.ToString().PadLeft(2, '0'),
                this.Second.ToString().PadLeft(2, '0')
            });
            this._dateTime = DateTime.ParseExact(tp, dateTimeformat, CultureInfo.CurrentCulture);
            this.time = this._dateTime.ToString("yyyy-MM-dd dddd HH:mm:ss");
            return this.time;
        }


        public DateTime ToDateTime()
        {
            try
            {
                string formatDateTime = "yyyyMMddHHmmss";
                string tp = string.Concat(new string[]
                {
                    this.Year.ToString().PadLeft(4, '0'),
                    this.Month.ToString().PadLeft(2, '0'),
                    this.Day.ToString().PadLeft(2, '0'),
                    this.Hour.ToString().PadLeft(2, '0'),
                    this.Minute.ToString().PadLeft(2, '0'),
                    this.Second.ToString().PadLeft(2, '0')
                });
                this._dateTime = DateTime.ParseExact(tp, formatDateTime, CultureInfo.CurrentCulture);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return this._dateTime;
        }


        public byte[] GetDateTimeBytes()
        {
            byte[] yearbyte = BitConverter.GetBytes(this.Year).Reverse<byte>().ToArray<byte>();
            byte monthbyte = this.Month;
            byte daybyte = Convert.ToByte(this.Day);
            bool flag = this.DayOfWeek == 0;
            byte weekbyte;
            if (flag)
            {
                weekbyte = 7;
            }
            else
            {
                weekbyte = Convert.ToByte(this.DayOfWeek);
            }

            byte hourbyte = Convert.ToByte(this.Hour);
            byte minbyte = Convert.ToByte(this.Minute);
            byte secondbyte = Convert.ToByte(this.Second);
            List<byte> list = new List<byte>();
            list.AddRange(yearbyte);
            list.Add(monthbyte);
            list.Add(daybyte);
            list.Add(weekbyte);
            list.Add(hourbyte);
            list.Add(minbyte);
            list.Add(secondbyte);
            List<byte> list2 = list;
            byte[] array = new byte[4];
            array[1] = 128;
            list2.AddRange(array);
            return list.ToArray();
        }

        // Token: 0x060003E9 RID: 1001 RVA: 0x000230A8 File Offset: 0x000212A8
        public DLMSClock(DateTime dateTime)
        {
            this.Year = (short) dateTime.Year;
            this.Month = (byte) dateTime.Month;
            this.Day = dateTime.Day;
            this.Hour = (byte) dateTime.Hour;
            this.Minute = (byte) dateTime.Minute;
            this.Second = (byte) dateTime.Second;
            this.DayOfWeek = (byte) dateTime.DayOfWeek;
        }

        public bool DlmsClockParse(byte[] dateTimeBytes)
        {
            bool result = false;
            try
            {
                byte[] yearstr = dateTimeBytes.Take(2).ToArray<byte>();
                this.Year = BitConverter.ToInt16(yearstr.Reverse<byte>().ToArray<byte>(), 0);
                this.Month = dateTimeBytes[2];
                this.Day = (int) dateTimeBytes[3];
                this.DayOfWeek = dateTimeBytes[4];
                this.Hour = dateTimeBytes[5];
                this.Minute = dateTimeBytes[6];
                this.Second = dateTimeBytes[7];
                result = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }

            return result;
        }

        public DLMSClock()
        {
        }

        public DLMSClock(byte[] dateTimeBytes)
        {
            try
            {
                byte[] yearstr = dateTimeBytes.Take(2).ToArray<byte>();
                this.Year = BitConverter.ToInt16(yearstr.Reverse<byte>().ToArray<byte>(), 0);
                this.Month = dateTimeBytes[2];
                this.Day = (int) dateTimeBytes[3];
                this.DayOfWeek = dateTimeBytes[4];
                this.Hour = dateTimeBytes[5];
                this.Minute = dateTimeBytes[6];
                this.Second = dateTimeBytes[7];
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }


        public static DateTime FirstDayOfMonth(DateTime datetime)
        {
            return datetime.AddDays((double) (1 - datetime.Day));
        }


        public void SetLastDayOfMonth()
        {
            DateTime dt = _dateTime.AddDays((double) (1 - this._dateTime.Day)).AddMonths(1).AddDays(-1.0);
            Iec62056DateTime1(dt);
        }


        private void Iec62056DateTime1(DateTime dateTime)
        {
            this.Year = (short) dateTime.Year;
            this.Month = (byte) dateTime.Month;
            this.Day = dateTime.Day;
            this.Hour = (byte) dateTime.Hour;
            this.Minute = (byte) dateTime.Minute;
            this.Second = (byte) dateTime.Second;
            this.DayOfWeek = (byte) dateTime.DayOfWeek;
        }


        public static DateTime FirstDayOfPreviousMonth(DateTime datetime)
        {
            return datetime.AddDays((double) (1 - datetime.Day)).AddMonths(-1);
        }


        public static DateTime LastDayOfPrdviousMonth(DateTime datetime)
        {
            return datetime.AddDays((double) (1 - datetime.Day)).AddDays(-1.0);
        }


        public static byte[] GetDataContent(byte[] bytes, out bool result)
        {
            byte[] date = bytes;
            result = false;
            try
            {
                byte len = bytes[19];
                date = bytes.Skip(20).Take((int) (len - 2)).ToArray<byte>();
                result = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return date;
        }


        public DLMSClock.Date date;

      


        private DateTime _dateTime;


        public struct Date
        {
            public byte Yearhighbyte { get; set; }


            public byte Yearlowbyte { get; set; }


            public byte Month { get; set; }


            public byte Dayofmonth { get; set; }


            public Dayofweek Dayofweek { get; set; }
        }


        public enum Dayofweek : byte
        {
            None,

            Monday,

            Tuesday,

            Wednesday,

            Thursday,

            Friday,

            Saturday,

            Sunday,

            NotSpecified = 255
        }
    }
}