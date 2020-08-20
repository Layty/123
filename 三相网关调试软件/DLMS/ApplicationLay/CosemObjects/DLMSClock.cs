using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using 三相智慧能源网关调试软件.Annotations;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;

namespace 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects
{
    public class DLMSClock : DLMSObject ,IDLMSBase,INotifyPropertyChanged
    {



        public int TimeZone
        {
            get => _TimeZone;
            set { _TimeZone = value; OnPropertyChanged(); }
        }
        private int _TimeZone;


     
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


       


        public string  Time
        {
            get => _Time;
            set { _Time = value; OnPropertyChanged(); }
        }
        private string  _Time;

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
            this.Time = this._dateTime.ToString("yyyy-MM-dd dddd HH:mm:ss");
            return this.Time;
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
            LogicalName = "0.0.1.0.0.255";
            ObjectType = ObjectType.Clock;
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

        public DLMSClock()
        {
            LogicalName = "0.0.1.0.0.255";
            ObjectType = ObjectType.Clock;
        }
        public DLMSClock(string logicalName)
        {
            LogicalName = logicalName;
            ObjectType = ObjectType.Clock;
        }
        public string[] GetNames()
        {
            return new string[9]
            {
                LogicalName,
                "Time",
                "Time Zone",
                "Status",
                "Begin",
                "End",
                "Deviation",
                "Enabled",
                "Clock Base"
            };
        }
        public string[] GetNames1 =>
            new string[9]
            {
                LogicalName,
                "Time",
                "Time Zone",
                "Status",
                "Begin",
                "End",
                "Deviation",
                "Enabled",
                "Clock Base"
            };

        public int GetAttributeCount()
        {
            return 9;
        }

        public int GetMethodCount()
        {
            return 6;
        }

        public DataType GetDataType(int index)
        {
            switch (index)
            {
                case 1:
                    return DataType.OctetString;
                case 2:
                    return DataType.OctetString;
                case 3:
                    return DataType.Int16;
                case 4:
                    return DataType.UInt8;
                case 5:
                    return DataType.OctetString;
                case 6:
                    return DataType.OctetString;
                case 7:
                    return DataType.Int8;
                case 8:
                    return DataType.Boolean;
                case 9:
                    return DataType.Enum;
                default:
                    throw new ArgumentException("GetDataType failed. Invalid attribute index.");
            }

            
        }

        public AttributeDescriptor GetTimeAttributeDescriptor() => GetCosemAttributeDescriptor(2);
        public byte[] GetTime() => GetCosemAttributeDescriptor(2).ToPduBytes();
        public AttributeDescriptor GetTimeZoneAttributeDescriptor() => GetCosemAttributeDescriptor(3);
        public byte[] GetTimeZone() => GetCosemAttributeDescriptor(3).ToPduBytes();
        public AttributeDescriptor GetStatusAttributeDescriptor() => GetCosemAttributeDescriptor(4);
        public byte[] GetStatus() => GetCosemAttributeDescriptor(4).ToPduBytes();

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}