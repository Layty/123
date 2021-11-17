using System;

namespace JobMaster.Models
{

    public class Energy
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public string EnergyData { get; set; }
        public string MeterId { get; set; }
    }
}