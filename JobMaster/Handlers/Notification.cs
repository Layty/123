using System;


namespace JobMaster.Handlers
{
    public class Notification
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public string NotifyData { get; set; }
        public string MeterId { get; set; }
    }
 
}