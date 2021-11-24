using Newtonsoft.Json;
using System;

namespace JobMaster.Models
{
    public class EnergyCaptureObjects
    {
        public DateTime DateTime { get; set; }
        [JsonProperty("正向有功总")] public string ImportActiveEnergyTotal { get; set; }
        [JsonProperty("正向有功尖")] public string ImportActiveEnergyT1 { get; set; }
        [JsonProperty("正向有功峰")] public string ImportActiveEnergyT2 { get; set; }
        [JsonProperty("正向有功平")] public string ImportActiveEnergyT3 { get; set; }
        [JsonProperty("正向有功谷")] public string ImportActiveEnergyT4 { get; set; }
        [JsonProperty("反向有功总")] public string ExportActiveEnergyTotal { get; set; }
        [JsonProperty("正向无功总")] public string ImportReactiveEnergyTotal { get; set; }
        [JsonProperty("反向无功总")] public string ExportReactiveEnergyTotal { get; set; }
    }
}