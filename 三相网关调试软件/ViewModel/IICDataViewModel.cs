using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.Model.IIC;

namespace 三相智慧能源网关调试软件.ViewModel
{
    /*
        联系邮箱：694965217@qq.com
        创建时间：2020/05/14 16:06:49
        主要用途：
        更改记录：
    */
    public class IicDataViewModel : ViewModelBase
    {
        public ObservableCollection<IicInstantData> InstantDataCollection
        {
            get => _instantDataCollection;
            set
            {
                _instantDataCollection = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<IicInstantData> _instantDataCollection;


        public ObservableCollection<IicEnergyData> CurrentEnergyDataCollection
        {
            get => _currentEnergyDataCollection;
            set
            {
                _currentEnergyDataCollection = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<IicEnergyData> _currentEnergyDataCollection;


        public ObservableCollection<IicEnergyData> Last1EnergyDataCollection
        {
            get => _last1EnergyDataCollection;
            set
            {
                _last1EnergyDataCollection = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<IicEnergyData> _last1EnergyDataCollection;


        public ObservableCollection<IicEnergyData> Last2EnergyDataCollection
        {
            get => _last2EnergyDataCollection;
            set
            {
                _last2EnergyDataCollection = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<IicEnergyData> _last2EnergyDataCollection;


        public ObservableCollection<IicCurrentDemandData> CurrentDemandDataCollection
        {
            get => _currentDemandDataCollection;
            set
            {
                _currentDemandDataCollection = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<IicCurrentDemandData> _currentDemandDataCollection;


        public ObservableCollection<IicLast1DemandData> Last1DemandDataCollection
        {
            get => _last1DemandDataCollection;
            set
            {
                _last1DemandDataCollection = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<IicLast1DemandData> _last1DemandDataCollection;

        public ObservableCollection<IicLast2DemandData> Last2DemandDataCollection
        {
            get => _last2DemandDataCollection;
            set
            {
                _last2DemandDataCollection = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<IicLast2DemandData> _last2DemandDataCollection;


        public ObservableCollection<IicHarmonicData> UaHarmonicDataCollection
        {
            get => _uaHarmonicDataCollection;
            set
            {
                _uaHarmonicDataCollection = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<IicHarmonicData> _uaHarmonicDataCollection;

        public ObservableCollection<IicHarmonicData> UbHarmonicDataCollection
        {
            get => _ubHarmonicDataCollection;
            set
            {
                _ubHarmonicDataCollection = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<IicHarmonicData> _ubHarmonicDataCollection;

        public ObservableCollection<IicHarmonicData> UcHarmonicDataCollection
        {
            get => _ucHarmonicDataCollection;
            set
            {
                _ucHarmonicDataCollection = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<IicHarmonicData> _ucHarmonicDataCollection;

        public ObservableCollection<IicHarmonicData> IaHarmonicDataCollection
        {
            get => _iaHarmonicDataCollection;
            set
            {
                _iaHarmonicDataCollection = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<IicHarmonicData> _iaHarmonicDataCollection;

        public ObservableCollection<IicHarmonicData> IbHarmonicDataCollection
        {
            get => _ibHarmonicDataCollection;
            set
            {
                _ibHarmonicDataCollection = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<IicHarmonicData> _ibHarmonicDataCollection;

        public ObservableCollection<IicHarmonicData> IcHarmonicDataCollection
        {
            get => _icHarmonicDataCollection;
            set
            {
                _icHarmonicDataCollection = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<IicHarmonicData> _icHarmonicDataCollection;

        public IicDataViewModel()
        {
            InstantDataCollection = new ObservableCollection<IicInstantData>();
            CurrentEnergyDataCollection = new ObservableCollection<IicEnergyData>();
            Last1EnergyDataCollection = new ObservableCollection<IicEnergyData>();
            Last2EnergyDataCollection = new ObservableCollection<IicEnergyData>();
            CurrentDemandDataCollection = new ObservableCollection<IicCurrentDemandData>();
            Last1DemandDataCollection = new ObservableCollection<IicLast1DemandData>();
            Last2DemandDataCollection = new ObservableCollection<IicLast2DemandData>();
            UaHarmonicDataCollection = new ObservableCollection<IicHarmonicData>();
            UbHarmonicDataCollection = new ObservableCollection<IicHarmonicData>();
            UcHarmonicDataCollection = new ObservableCollection<IicHarmonicData>();
            IaHarmonicDataCollection = new ObservableCollection<IicHarmonicData>();
            IbHarmonicDataCollection = new ObservableCollection<IicHarmonicData>();
            IcHarmonicDataCollection = new ObservableCollection<IicHarmonicData>();


            Messenger.Default.Register<byte[]>(this, "ReceiveDataEvent", HandlerData);
        }

        private void HandlerData(byte[] obj)
        {
            var stringData = Encoding.Default.GetString(obj);
            var bb = stringData.Split('\n');
            if (bb.Length == 3)
            {
                var bbb = bb[1].Replace('\r', ' ');
                if (bbb.Length != 0)
                {
                    var bytes = bbb.StringToByte();
                    if (bytes[0] == 0x80)
                    {
                        bool result;
                        if (bytes[1] == 0x01)
                        {
                            IicInstantData data = new IicInstantData();
                            result = data.ParseData(bbb);
                            if (result)
                            {
                                DispatcherHelper.CheckBeginInvokeOnUI(() => { InstantDataCollection.Add(data); });
                            }
                        }
                        else
                        {
                            if (bytes[1] == 0x10)
                            {
                                IicEnergyData iicCurrentEnergyData = new IicEnergyData();
                                result = iicCurrentEnergyData.ParseData(bytes);
                                if (result)
                                {
                                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                    {
                                        CurrentEnergyDataCollection.Add(iicCurrentEnergyData);
                                    });
                                }
                            }
                            else if (bytes[1] == 0x11)
                            {
                                IicEnergyData iicLast1EnergyData = new IicEnergyData();
                                result = iicLast1EnergyData.ParseData(bytes);
                                if (result)
                                {
                                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                    {
                                        Last1EnergyDataCollection.Add(iicLast1EnergyData);
                                    });
                                }
                            }
                            else if (bytes[1] == 0x12)
                            {
                                IicEnergyData iicLast2EnergyData = new IicEnergyData();
                                result = iicLast2EnergyData.ParseData(bytes);
                                if (result)
                                {
                                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                    {
                                        Last2EnergyDataCollection.Add(iicLast2EnergyData);
                                    });
                                }
                            }
                            else if (bytes[1] == 0x20)
                            {
                                IicCurrentDemandData iicCurrentDemandData = new IicCurrentDemandData();
                                result = iicCurrentDemandData.ParseData(bytes);
                                if (result)
                                {
                                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                    {
                                        CurrentDemandDataCollection.Add(iicCurrentDemandData);
                                    });
                                }
                            }
                            else if (bytes[1] == 0x21)
                            {
                                IicLast1DemandData last1DemandData = new IicLast1DemandData();
                                result = last1DemandData.ParseData(bytes);
                                if (result)
                                {
                                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                    {
                                        Last1DemandDataCollection.Add(last1DemandData);
                                    });
                                }
                            }
                            else if (bytes[1] == 0x22)
                            {
                                IicLast2DemandData last2DemandData = new IicLast2DemandData();
                                result = last2DemandData.ParseData(bytes);
                                if (result)
                                {
                                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                    {
                                        Last2DemandDataCollection.Add(last2DemandData);
                                    });
                                }
                            }
                            else if (bytes[1] == 0x30)
                            {
                                IicHarmonicData iicHarmonicDataUa = new IicHarmonicData();
                                IicHarmonicData iicHarmonicDataUb = new IicHarmonicData();
                                IicHarmonicData iicHarmonicDataUc = new IicHarmonicData();
                                IicHarmonicData iicHarmonicDataIa = new IicHarmonicData();
                                IicHarmonicData iicHarmonicDataIb = new IicHarmonicData();
                                IicHarmonicData iicHarmonicDataIc = new IicHarmonicData();
                                var resultUa = iicHarmonicDataUa.ParseData(bytes.Skip(2).Take(42).ToArray());
                                var resultUb = iicHarmonicDataUb.ParseData(bytes.Skip(44).Take(42).ToArray());
                                var resultUc = iicHarmonicDataUc.ParseData(bytes.Skip(86).Take(42).ToArray());
                                var resultIa = iicHarmonicDataIa.ParseData(bytes.Skip(128).Take(42).ToArray());
                                var resultIb = iicHarmonicDataIb.ParseData(bytes.Skip(170).Take(42).ToArray());
                                var resultIc = iicHarmonicDataIc.ParseData(bytes.Skip(212).Take(42).ToArray());
                                if (resultUa && resultUb && resultUc && resultIa && resultIb && resultIc)
                                {
                                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                    {
                                        UaHarmonicDataCollection.Add(iicHarmonicDataUa);
                                        UbHarmonicDataCollection.Add(iicHarmonicDataUb);
                                        UcHarmonicDataCollection.Add(iicHarmonicDataUc);
                                        IaHarmonicDataCollection.Add(iicHarmonicDataIa);
                                        IbHarmonicDataCollection.Add(iicHarmonicDataIb); 
                                        IcHarmonicDataCollection.Add(iicHarmonicDataIc);
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}