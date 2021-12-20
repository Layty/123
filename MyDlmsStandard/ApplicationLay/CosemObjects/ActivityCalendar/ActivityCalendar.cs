using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MyDlmsStandard.ApplicationLay.CosemObjects
{
    public class SeasonProfile
    {

    }
    public class WeekProfileTable { }
    public class DayProfileTable { }
    /// <summary>
    /// 活动日历 classid=20
    /// </summary>
    public class ActivityCalendar : CosemObject
    {
        public ObservableCollection<SeasonProfile> SeasonProfileActive { get; set; }
        public ObservableCollection<WeekProfileTable> WeekProfileTableActive { get; set; }
        public ObservableCollection<DayProfileTable> DayProfileTableActive { get; set; }
        public ActivityCalendar() 
        {
            ClassId = MyConvert.GetClassIdByObjectType(ObjectType.ActivityCalendar);
            this.LogicalName = "0.0.13.0.0.255"; 
        }
        public CosemAttributeDescriptor Getcalendar_name_activAttributeDescriptor()
        {
            return GetCosemAttributeDescriptor(2);
        }
        public CosemAttributeDescriptor Getseason_profile_activeAttributeDescriptor()
        {
            return GetCosemAttributeDescriptor(3);
        }
        public CosemAttributeDescriptor Getweek_profile_table_activeAttributeDescriptor()
        {
            return GetCosemAttributeDescriptor(4);
        }
        public CosemAttributeDescriptor Getday_profile_table_activeAttributeDescriptor()
        {
            return GetCosemAttributeDescriptor(5);
        }
        public CosemAttributeDescriptor Getcalendar_name_passiveAttributeDescriptor()
        {
            return GetCosemAttributeDescriptor(6);
        }
        public CosemAttributeDescriptor Getseason_profile_passiveAttributeDescriptor()
        {
            return GetCosemAttributeDescriptor(7);
        }
        public CosemAttributeDescriptor Getweek_profile_table_passiveAttributeDescriptor()
        {
            return GetCosemAttributeDescriptor(8);
        }
        public CosemAttributeDescriptor Getday_profile_table_passiveAttributeDescriptor()
        {
            return GetCosemAttributeDescriptor(9);
        }
    }
}
