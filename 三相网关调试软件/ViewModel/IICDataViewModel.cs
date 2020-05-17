using System;
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


        public ObservableCollection<IicDemandData> CurrentDemandDataCollection
        {
            get => _currentDemandDataCollection;
            set
            {
                _currentDemandDataCollection = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<IicDemandData> _currentDemandDataCollection;


        public ObservableCollection<IicDemandData> Last1DemandDataCollection
        {
            get => _last1DemandDataCollection;
            set
            {
                _last1DemandDataCollection = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<IicDemandData> _last1DemandDataCollection;

        public ObservableCollection<IicDemandData> Last2DemandDataCollection
        {
            get => _last2DemandDataCollection;
            set
            {
                _last2DemandDataCollection = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<IicDemandData> _last2DemandDataCollection;


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
            CurrentDemandDataCollection = new ObservableCollection<IicDemandData>();
            Last1DemandDataCollection = new ObservableCollection<IicDemandData>();
            Last2DemandDataCollection = new ObservableCollection<IicDemandData>();
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
                    var dataType = BitConverter.ToUInt16(bytes.Take(2).Reverse().ToArray(), 0);
                    if (dataType == (ushort) IicDataType.IicInstantData)
                    {
                        IicInstantData data = new IicInstantData();
                        if (data.ParseData(bbb))
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() => { InstantDataCollection.Add(data); });
                        }
                    }
                    else if (dataType == (ushort) IicDataType.IicCurrentEnergyData)
                    {
                        IicEnergyData iicCurrentEnergyData = new IicEnergyData();
                        if (iicCurrentEnergyData.ParseData(bytes))
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                CurrentEnergyDataCollection.Add(iicCurrentEnergyData);
                            });
                        }
                    }
                    else if (dataType == (ushort) IicDataType.IicLast1EnergyData)
                    {
                        IicEnergyData iicLast1EnergyData = new IicEnergyData();
                        if (iicLast1EnergyData.ParseData(bytes))
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                Last1EnergyDataCollection.Add(iicLast1EnergyData);
                            });
                        }
                    }
                    else if (dataType == (ushort) IicDataType.IicLast2EnergyData)
                    {
                        IicEnergyData iicLast2EnergyData = new IicEnergyData();
                        if (iicLast2EnergyData.ParseData(bytes))
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                Last2EnergyDataCollection.Add(iicLast2EnergyData);
                            });
                        }
                    }
                    else if (dataType == (ushort) IicDataType.IicCurrentDemandData)
                    {
                        IicDemandData iicCurrentDemandData = new IicDemandData();
                        if (iicCurrentDemandData.ParseData(bytes))
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                CurrentDemandDataCollection.Add(iicCurrentDemandData);
                            });
                        }
                    }
                    else if (dataType == (ushort) IicDataType.IicLast1DemandData)
                    {
                        IicDemandData last1DemandData = new IicDemandData();
                        if (last1DemandData.ParseData(bytes))
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                Last1DemandDataCollection.Add(last1DemandData);
                            });
                        }
                    }
                    else if (dataType == (ushort) IicDataType.IicLast2DemandData)
                    {
                        IicDemandData last2DemandData = new IicDemandData();
                        if (last2DemandData.ParseData(bytes))
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                Last2DemandDataCollection.Add(last2DemandData);
                            });
                        }
                    }
                    else if (dataType == (ushort) IicDataType.IicHarmonicData)
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