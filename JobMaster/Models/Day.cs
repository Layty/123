using System;

namespace JobMaster.Models
{
    public class Day
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public string DayData { get; set; }
        public string MeterId { get; set; }
    }
}