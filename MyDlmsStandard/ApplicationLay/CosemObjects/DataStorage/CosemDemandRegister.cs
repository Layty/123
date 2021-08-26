using System;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Axdr;
using MyDlmsStandard.Common;

namespace MyDlmsStandard.ApplicationLay.CosemObjects.DataStorage
{
    public class CosemDemandRegister : CosemExtendedRegister
    {
        /// <summary>
        /// 提供当前值(运行需量),从start_time 时间开始的累计值除以number_of_ period×speriod 的电能值。
        /// 值的数据类型依logical_name 所定义的具体实例而定,也可由制造商选定。
        /// 数据类型的选择应使其与logical_name一起能够准确解析值的含义。
        /// 假如其他能量型的量也在同时测量,则其他的方法也可采用(例如计算电压或 电流的平均值)。
        /// </summary>
        public DlmsDataItem CurrentAverageValue
        {
            get => _currentAverageValue;
            set
            {
                _currentAverageValue = value;
                OnPropertyChanged();
            }
        }

        private DlmsDataItem _currentAverageValue;

        /// <summary>
        /// 提供累计的能量值(在最后的number_of_periods×period)除以number_of _periods ×period。
        /// 计算中不考虑当前(未终止)期间的电能。
        /// 如果测量的不是电能,还可以采用其他计算方法(例如计算电压或电流的平均 值)。
        /// </summary>
        public DlmsDataItem LastAverageValue
        {
            get => _lastAverageValue;
            set
            {
                _lastAverageValue = value;
                OnPropertyChanged();
            }
        }

        private DlmsDataItem _lastAverageValue;

        /// <summary>
        /// 属性7
        /// </summary>
        public AxdrOctetString StartTimeCurrent
        {
            get => _startTimeCurrent;
            set
            {
                _startTimeCurrent = value;
                OnPropertyChanged();
            }
        }

        private AxdrOctetString _startTimeCurrent;

        /// <summary>
        /// 连续2次更新last_average_value之间的间隔时间。
        /// number_of_periods× period 是需量计算的分母)。
        /// 数据类型为double-long-unsigned,测量周期以秒为单位。
        /// </summary>
        public AxdrIntegerUnsigned32 Period
        {
            get => _period;
            set
            {
                _period = value;
                OnPropertyChanged();
            }
        }

        private AxdrIntegerUnsigned32 _period;

        /// <summary>
        /// 用于计算last_average_value的周期数。
        /// number_of_periods>=1
        /// ———number_of_periods>1 指 示last_average_value 表 示 “sliding demand”;
        /// ———number_of_periods=1 指 示last_average_value 表 示 “block demand”。
        /// </summary>
        public AxdrIntegerUnsigned16 NumberOfPeriods
        {
            get => _numberOfPeriods;
            set
            {
                _numberOfPeriods = value;
                OnPropertyChanged();
            }
        }

        private AxdrIntegerUnsigned16 _numberOfPeriods;

        public override CosemAttributeDescriptor ScalarUnitAttributeDescriptor => GetCosemAttributeDescriptor(4);

        public override CosemAttributeDescriptor StatusAttributeDescriptor => GetCosemAttributeDescriptor(5);

        public override CosemAttributeDescriptor CaptureTimeAttributeDescriptor => GetCosemAttributeDescriptor(6);

        public CosemAttributeDescriptor StartTimeCurrentAttributeDescriptor => GetCosemAttributeDescriptor(7);
        public CosemAttributeDescriptor PeriodAttributeDescriptor => GetCosemAttributeDescriptor(8);

        public CosemAttributeDescriptor NumberOfPeriodsAttributeDescriptor => GetCosemAttributeDescriptor(9);


        public CosemMethodDescriptor NextPeriodMethodDescriptor => GetCosemMethodDescriptor(2);

        public CosemDemandRegister(string logicName) : base(logicName)
        {
            Version = 0;
            LogicalName = logicName;
            ClassId = MyConvert.GetClassIdByObjectType(ObjectType.DemandRegister);
        }

        public void Reset(DlmsDataItem data)
        {
            CurrentAverageValue = new DlmsDataItem();
            LastAverageValue = new DlmsDataItem();
            //CaptureTime=DateTime.Now();
            // StartTimeCurrent == DateTime.Now();
            base.Reset(data);
        }

        public void Next_period()
        {
            CaptureTime = new AxdrOctetString(); //赋值为当前时间触发方法的实际时间
            StartTimeCurrent = new AxdrOctetString(); //赋值为当前时间触发方法的实际时间

            LastAverageValue = CurrentAverageValue;
            CurrentAverageValue = new DlmsDataItem(); //赋值为零
        }
    }
}