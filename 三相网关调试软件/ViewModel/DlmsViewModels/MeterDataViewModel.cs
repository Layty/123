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
        public string ImportActiveEnergyTotal { get; set; }
        public string ImportActiveEnergyT1 { get; set; }
        public string ImportActiveEnergyT2 { get; set; }
        public string ImportActiveEnergyT3 { get; set; }
        public string ImportActiveEnergyT4 { get; set; }
        public string ExportActiveEnergyTotal { get; set; }
        public string ImportReactiveEnergyTotal { get; set; }
        public string ExportReactiveEnergyTotal { get; set; }
    }
    public class PowerCaptureObjects
    {
        public DateTime DateTime { get; set; }
        public string ImportActivePowerTotal { get; set; }
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