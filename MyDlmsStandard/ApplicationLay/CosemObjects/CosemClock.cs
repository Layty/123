using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;


namespace MyDlmsStandard.ApplicationLay.CosemObjects
{
    public class CosemClock : CosemObject, IDlmsBase
    {
        public int TimeZone
        {
            get => _timeZone;
            set
            {
                _timeZone = value;
                OnPropertyChanged();
            }
        }

        private int _timeZone;


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


        public string Time
        {
            get => _time;
            set
            {
                _time = value;
                OnPropertyChanged();
            }
        }

        private string _time;

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
                throw new Exception(e.Message);
            }

            return this._dateTime;
        }


        public byte[] GetDateTimeBytes()
        {
            byte[] yearbyte = BitConverter.GetBytes(Year).Reverse().ToArray();
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

        public CosemClock(DateTime dateTime)
        {
            LogicalName = "0.0.1.0.0.255";
            ClassId = MyConvert.GetClassIdByObjectType(ObjectType.Clock);
            this.Year = (short)dateTime.Year;
            this.Month = (byte)dateTime.Month;
            this.Day = dateTime.Day;
            this.Hour = (byte)dateTime.Hour;
            this.Minute = (byte)dateTime.Minute;
            this.Second = (byte)dateTime.Second;
            this.DayOfWeek = (byte)dateTime.DayOfWeek;
        }

        public bool DlmsClockParse(byte[] dateTimeBytes)
        {
            bool result = false;
            try
            {
                byte[] yearstr = dateTimeBytes.Take(2).ToArray();
                this.Year = BitConverter.ToInt16(yearstr.Reverse<byte>().ToArray<byte>(), 0);
                this.Month = dateTimeBytes[2];
                this.Day = (int)dateTimeBytes[3];
                this.DayOfWeek = dateTimeBytes[4];
                this.Hour = dateTimeBytes[5];
                this.Minute = dateTimeBytes[6];
                this.Second = dateTimeBytes[7];
                string tp = string.Concat(new string[]
             {
                this.Year.ToString().PadLeft(4, '0'),
                this.Month.ToString().PadLeft(2, '0'),
                this.Day.ToString().PadLeft(2, '0'),
                this.Hour.ToString().PadLeft(2, '0'),
                this.Minute.ToString().PadLeft(2, '0'),
                this.Second.ToString().PadLeft(2, '0')
             });
                string dateTimeformat = "yyyyMMddHHmmss";
                this._dateTime = DateTime.ParseExact(tp, dateTimeformat, CultureInfo.CurrentCulture);
                result = true;
            }
            catch (Exception e)
            {
                result = false;
                //throw new Exception(e.Message);
            }

            return result;
        }


        public CosemClock(byte[] dateTimeBytes)
        {
            try
            {
                byte[] yearstr = dateTimeBytes.Take(2).ToArray<byte>();
                this.Year = BitConverter.ToInt16(yearstr.Reverse<byte>().ToArray<byte>(), 0);
                this.Month = dateTimeBytes[2];
                this.Day = (int)dateTimeBytes[3];
                this.DayOfWeek = dateTimeBytes[4];
                this.Hour = dateTimeBytes[5];
                this.Minute = dateTimeBytes[6];
                this.Second = dateTimeBytes[7];
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        public static DateTime FirstDayOfMonth(DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day);
        }


        public void SetLastDayOfMonth()
        {
            DateTime dt = _dateTime.AddDays((double)(1 - this._dateTime.Day)).AddMonths(1).AddDays(-1.0);
            Iec62056DateTime1(dt);
        }


        private void Iec62056DateTime1(DateTime dateTime)
        {
            this.Year = (short)dateTime.Year;
            this.Month = (byte)dateTime.Month;
            this.Day = dateTime.Day;
            this.Hour = (byte)dateTime.Hour;
            this.Minute = (byte)dateTime.Minute;
            this.Second = (byte)dateTime.Second;
            this.DayOfWeek = (byte)dateTime.DayOfWeek;
        }


        public static DateTime FirstDayOfPreviousMonth(DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day).AddMonths(-1);
        }


        public static DateTime LastDayOfPreviousMonth(DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day).AddDays(-1.0);
        }


        public CosemClock.Date date;


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

        public CosemClock()
        {
            LogicalName = "0.0.1.0.0.255";
            ClassId = MyConvert.GetClassIdByObjectType(ObjectType.Clock);

        }

        public CosemClock(string logicalName)
        {
            LogicalName = logicalName;
            ClassId = MyConvert.GetClassIdByObjectType(ObjectType.Clock);
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
            new string[]
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

        public int AttributeCount => 9;


        public int MethodCount => 6;


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

        public CosemAttributeDescriptor GetTimeAttributeDescriptor() => GetCosemAttributeDescriptor(2);

        public CosemAttributeDescriptor GetTimeZoneAttributeDescriptor() => GetCosemAttributeDescriptor(3);

        public CosemAttributeDescriptor GetStatusAttributeDescriptor() => GetCosemAttributeDescriptor(4);

    }
}