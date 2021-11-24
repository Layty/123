using Newtonsoft.Json;
using System;

namespace JobMaster.Models
{
    public class PowerCaptureObjects
    {
        public DateTime DateTime { get; set; }
        [JsonProperty("正向有功总功率")] public string ImportActivePowerTotal { get; set; }
        [JsonProperty("反向有功总功率")] public string ExportActivePowerTotal { get; set; }
        public string A相电压 { get; set; }
        public string B相电压 { get; set; }
        public string C相电压 { get; set; }
        public string A相电流 { get; set; }
        public string B相电流 { get; set; }
        public string C相电流 { get; set; }
    }
}