using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.ObjectModel;

namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{
    public class Meter
    {
        public string MeterId { get; set; }

        //        public List<Energy> Energies { get; set; } = new List<Energy>();
        //        public List<Power> Powers { get; set; } = new List<Power>();
    }


    public class Energy
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public string EnergyData { get; set; }
        public string MeterId { get; set; }
    }

    public class EnergyCaptureObjects
    {
        public DateTime DateTime { get; set; }
        [JsonProperty("正向有功总")]
        public string ImportActiveEnergyTotal { get; set; }
        [JsonProperty("正向有功尖")]
        public string ImportActiveEnergyT1 { get; set; }
        [JsonProperty("正向有功峰")]
        public string ImportActiveEnergyT2 { get; set; }
        [JsonProperty("正向有功平")]
        public string ImportActiveEnergyT3 { get; set; }
        [JsonProperty("正向有功谷")]
        public string ImportActiveEnergyT4 { get; set; }
        [JsonProperty("反向有功总")]
        public string ExportActiveEnergyTotal { get; set; }
        [JsonProperty("正向无功总")]
        public string ImportReactiveEnergyTotal { get; set; }
        [JsonProperty("反向无功总")]
        public string ExportReactiveEnergyTotal { get; set; }
    }
    public class PowerCaptureObjects
    {
        public DateTime DateTime { get; set; }
        [JsonProperty("正向有功总功率")]
        public string ImportActivePowerTotal { get; set; }
        [JsonProperty("反向有功总功率")] 
        public string ExportActivePowerTotal { get; set; }
        public string A相电压 { get; set; }
        public string B相电压 { get; set; }
        public string C相电压 { get; set; }
        public string A相电流 { get; set; }
        public string B相电流 { get; set; }
        public string C相电流 { get; set; }
    }
    public class Power
    {
        public Guid Id { get; set; }

        public DateTime DateTime { get; set; }
        public string PowerData { get; set; }
        public string MeterId { get; set; }
    }


    public class MeterDataViewModel : ObservableObject
    {
        public RelayCommand GetCommand { get; set; }
        private ObservableCollection<Meter> _energyModels;

        public ObservableCollection<Meter> EnergyModels
        {
            get => _energyModels;
            set
            {
                _energyModels = value;
                OnPropertyChanged();
            }
        }

        public MeterDataViewModel()
        {
            GetCommand = new RelayCommand(() =>
            {
                var client = new RestClient($"{Properties.Settings.Default.WebApiUrl}/Meter");
                var request = new RestRequest(Method.GET);


                try
                {
                    IRestResponse response = client.Execute(request);
                    EnergyModels =
                        JsonConvert.DeserializeObject<ObservableCollection<Meter>>(response.Content);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
        }
    }
}