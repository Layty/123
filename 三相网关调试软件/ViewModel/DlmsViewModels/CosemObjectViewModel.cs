using System;
using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Newtonsoft.Json;
using RestSharp;
using 三相智慧能源网关调试软件.Model;
using 三相智慧能源网关调试软件.View;

namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{
    public class CosemObjectViewModel : ObservableObject
    {
        public CosemObjectViewModel()
        {
            CurrentCosemObjectEdit = new CosemObjectEdit();
            CosemObjects = new ObservableCollection<CosemObjectEdit>();

            GetAllCosemObjectsCommand = new RelayCommand(() =>
            {
                var client = new RestClient($"{Properties.Settings.Default.WebApiUrl}/CosemObjects");
                var request = new RestRequest(Method.GET);


                try
                {
                    IRestResponse response = client.Execute(request);
                    CosemObjects =
                        JsonConvert.DeserializeObject<ObservableCollection<CosemObjectEdit>>(response.Content);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
            GetByObisCommand = new RelayCommand<string>(s =>
            {
                var client = new RestClient($"{Properties.Settings.Default.WebApiUrl}/CosemObjects/ByObis/{s}");
                var request = new RestRequest(Method.GET);
                try
                {
                    IRestResponse response = client.Execute(request);
                    CosemObjects.Clear();
                    var getCosemObject = JsonConvert.DeserializeObject<CosemObjectEdit>(response.Content);
                    if (getCosemObject != null)
                    {
                        CosemObjects.Add(getCosemObject);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
            GetByClassIdCommand = new RelayCommand<string>((s =>
            {
                var client = new RestClient($"{Properties.Settings.Default.WebApiUrl}/CosemObjects/ByClassId/{s}");
                var request = new RestRequest(Method.GET);
               
                try
                {
                    IRestResponse response = client.Execute(request);
                    CosemObjects.Clear();
                    CosemObjects =
                        JsonConvert.DeserializeObject<ObservableCollection<CosemObjectEdit>>(response.Content);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }));
            GetByNameCommand = new RelayCommand<string>((s =>
            {
                var client = new RestClient($"{Properties.Settings.Default.WebApiUrl}/CosemObjects/ByName/{s}");
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                try
                {
                    CosemObjects =
                        JsonConvert.DeserializeObject<ObservableCollection<CosemObjectEdit>>(response.Content);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }));
            AddCommand = new RelayCommand(() =>
            {
                try
                {
                    var client = new RestClient($"{Properties.Settings.Default.WebApiUrl}/CosemObjects/{CurrentCosemObjectEdit.Obis}");
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/json");
                    CurrentCosemObjectEdit.Id = Guid.NewGuid();
                    var str = JsonConvert.SerializeObject(CurrentCosemObjectEdit);
                    request.AddParameter("CurrentCosemObjectEdit", str, ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);
                    MessageBoxWindow message = new MessageBoxWindow();
                    message.Message = response.IsSuccessful ? "成功" : "失败";
                    message.ShowDialog();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
            UpdateCommand = new RelayCommand<CosemObjectEdit>((t) =>
            {
                try
                {
                    var client = new RestClient($"{Properties.Settings.Default.WebApiUrl}/CosemObjects/{t.Obis}");
                    var request = new RestRequest(Method.PUT);
                    request.AddHeader("Content-Type", "application/json");
                    var str = JsonConvert.SerializeObject(t);
                    request.AddParameter("Update", str, ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);
                    MessageBoxWindow message = new MessageBoxWindow();
                    message.Message = response.IsSuccessful ? "成功" : "失败";
                    message.ShowDialog();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
            DeleteCommand = new RelayCommand<CosemObjectEdit>(t =>
            {
                try
                {
                    var client = new RestClient($"{Properties.Settings.Default.WebApiUrl}/CosemObjects/{t.Obis}");
                    var request = new RestRequest(Method.DELETE);

                    IRestResponse response = client.Execute(request);
                    MessageBoxWindow message = new MessageBoxWindow();
                    message.Message = response.IsSuccessful ? "成功" : "失败";
                    message.ShowDialog();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
        }

        private void AddHeader(RestRequest request)
        {
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Accept-Encoding", "gzip, deflate");
            request.AddHeader("Host", "localhost:5000");
            request.AddHeader("Postman-Token",
                "1f01f478-7455-48a4-98ec-13c088abdfdf,b71b18e9-0c33-474e-af0c-ca9891227e00");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("User-Agent", "PostmanRuntime/7.15.2");
            request.AddHeader("Content-Type", "application/json");
        }

        public ObservableCollection<CosemObjectEdit> CosemObjects
        {
            get => _cosemObjects;
            set
            {
                _cosemObjects = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<CosemObjectEdit> _cosemObjects;


        public CosemObjectEdit CurrentCosemObjectEdit
        {
            get => _currentCosemObjectEdit;
            set
            {
                _currentCosemObjectEdit = value;
                OnPropertyChanged();
            }
        }

        private CosemObjectEdit _currentCosemObjectEdit;


        public RelayCommand AddCommand { get; set; }
        public RelayCommand<CosemObjectEdit> UpdateCommand { get; set; }
        public RelayCommand<CosemObjectEdit> DeleteCommand { get; set; }

        public RelayCommand<string> GetByObisCommand { get; set; }

        public RelayCommand<string> GetByNameCommand { get; set; }

        public RelayCommand<string> GetByClassIdCommand { get; set; }
        public RelayCommand GetAllCosemObjectsCommand { get; set; }
    }
}