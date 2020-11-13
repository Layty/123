using System;

namespace CosemWebApi.Entities
{
    public class CosemObject
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int ClassId { get; set; }
        public string Obis { get; set; }

    }
}