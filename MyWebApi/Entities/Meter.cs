using System;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities
{
    public class Meter
    {
        [MinLength(12)]
        [MaxLength(12)]
        [Key]
        [Required]
        public string MeterId { get; set; }

        //        public List<Energy> Energies { get; set; } = new List<Energy>();
        //        public List<Power> Powers { get; set; } = new List<Power>();
    }


    public class Energy
    {
        [Key] public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public string EnergyData { get; set; }
        public string MeterId { get; set; }
    }
    public class Day
    {
        [Key] public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public string DayData { get; set; }
        public string MeterId { get; set; }
    }

    public class Power
    {
        [Key] public Guid Id { get; set; }

        public DateTime DateTime { get; set; }
        public string PowerData { get; set; }
        public string MeterId { get; set; }
    }

    public class Notification {

        [Key] public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public string NotifyData { get; set; }
        public string MeterId { get; set; }
    }
}