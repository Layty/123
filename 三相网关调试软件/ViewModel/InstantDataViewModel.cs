using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using LiveCharts;
using LiveCharts.Wpf;
using Newtonsoft.Json;
using 三相智慧能源网关调试软件.Model;
using 三相智慧能源网关调试软件.Properties;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class InstantDataViewModel : ViewModelBase
    {
        public DispatcherTimer Timer = new DispatcherTimer();


        public int Interval
        {
            get => _interval;
            set
            {
                _interval = value;
                Timer.Interval = new TimeSpan(0, 0, 0, Interval);
                RaisePropertyChanged();
            }
        }

        private int _interval;

        private int _maxShowCount;

        public int MaxShowCount
        {
            get => _maxShowCount;
            set
            {
                _maxShowCount = value;
                RaisePropertyChanged();
            }
        }


        private ObservableCollection<double> _field;

        public ObservableCollection<double> DataSelect
        {
            get => _field;
            set
            {
                _field = value;
                RaisePropertyChanged();
            }
        }


        private SeriesCollection _seriesCollection;

        public SeriesCollection SeriesCollection
        {
            get => _seriesCollection;
            set
            {
                _seriesCollection = value;
                RaisePropertyChanged();
            }
        }


        private LineSeries _lineSeries;

        public LineSeries LineSeries
        {
            get => _lineSeries;
            set
            {
                _lineSeries = value;
                RaisePropertyChanged();
            }
        }

        public List<string> Labels { get; set; }


        private RelayCommand<string> _selectCommand;

        public RelayCommand<string> SelectCommand
        {
            get => _selectCommand;
            set
            {
                _selectCommand = value;
                RaisePropertyChanged();
            }
        }


        public InstantDataViewModel()
        {
            if (IsInDesignMode)
            {
                InstantDataModels = new ObservableCollection<InstantDataModel>
                {
                    new InstantDataModel { DateTime="11",Ia = 1, Ib = 2, Ic = 3}
                };
            }
            else
            {
                InstantDataModels = new ObservableCollection<InstantDataModel>
                {
                    new InstantDataModel {DateTime="11",Ia = 1, Ib = 2, Ic = 3}
                };
                InstantCommandString = Settings.Default.CmdStr1;
                RequestInstantDataCommand = new RelayCommand<string>(SendMsg);
                Interval = Settings.Default.InstantDataRefreshInterval;
                Timer.Interval = new TimeSpan(0, 0, 0, Interval);

                Timer.Tick += Timer_Tick;
                Messenger.Default.Register<byte[]>(this, "ReceiveDataEvent", (ENetClientHelper_ReceiveData));

                MaxShowCount = 10;
                ChartValues = new ChartValues<double>();
                //实例化一条折线图
                LineSeries = new LineSeries {Title = "Ia", Values = ChartValues};
                SeriesCollection = new SeriesCollection {LineSeries};
                //SelectCommand = new RelayCommand<int>(Select);


                ClearCommand = new RelayCommand(() => { InstantDataModels.Clear(); });
                Labels = new List<string>();
             
                random = new Random();
            }
        }

        private RelayCommand _clearCommand;

        public RelayCommand ClearCommand
        {
            get { return _clearCommand; }
            set
            {
                _clearCommand = value;
                RaisePropertyChanged();
            }
        }


        public void Select(int index)
        {
            foreach (var instantDataModel in InstantDataModels)
            {
                DataSelect.Add(instantDataModel.Ia);
            }
        }

        private ChartValues<double> _chartValues;

        public ChartValues<double> ChartValues
        {
            get { return _chartValues; }
            set
            {
                _chartValues = value;
                RaisePropertyChanged();
            }
        }

        private Random random;

        private void ENetClientHelper_ReceiveData(byte[] bytes)
        {
            string str = Encoding.Default.GetString(bytes);
            try
            {
                var data = JsonConvert.DeserializeObject<InstantDataModel>(str);
                if (data == null)
                {
                    return;
                }

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    data.DateTime = DateTime.Now.ToString();
                    InstantDataModels.Add(data);
                    Labels.Add(DateTime.Now.ToString(CultureInfo.InvariantCulture));
                    var t = random.Next(-10, 10);
                    ChartValues.Add(t);
                    if (Labels.Count >= MaxShowCount + 1)
                    {
                        for (int i = 0; i < Labels.Count - MaxShowCount + 1; i++)
                        {
                            Labels.RemoveAt(0);
                            ChartValues.RemoveAt(0);
                        }
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine( e.Message);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            RequestInstantDataCommand.Execute(InstantCommandString);
        }

        private string _instantCommandString;

        public string InstantCommandString
        {
            get { return _instantCommandString; }
            set
            {
                _instantCommandString = value;
                RaisePropertyChanged();
            }
        }


        private ObservableCollection<InstantDataModel> _instantDataModels;

        public ObservableCollection<InstantDataModel> InstantDataModels
        {
            get { return _instantDataModels; }
            set
            {
                _instantDataModels = value;
                RaisePropertyChanged();
            }
        }

        private bool _isAutoRefreshData;

        public bool IsAutoRefreshData
        {
            get => _isAutoRefreshData;
            set
            {
                _isAutoRefreshData = value;
                if (value)
                {
                    Timer.Start();
                }
                else
                {
                    Timer.Stop();
                }

                RaisePropertyChanged();
            }
        }

        private RelayCommand<string> _requestInstantDataCommand;

        public RelayCommand<string> RequestInstantDataCommand
        {
            get => _requestInstantDataCommand;
            set
            {
                _requestInstantDataCommand = value;
                RaisePropertyChanged();
            }
        }

        ENetClientHelper eNet = ServiceLocator.Current.GetInstance<ENetClientHelper>();

        public void SendMsg(string commandString)
        {
            Task.Run(() => { eNet.SendDataToServer(commandString); });
        }
    }
}