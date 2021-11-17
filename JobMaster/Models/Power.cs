using System;

namespace JobMaster.Models
{
    public class Power
    {
        public Guid Id { get; set; }

        public DateTime DateTime { get; set; }
        public string PowerData { get; set; }
        public string MeterId { get; set; }
    }
}