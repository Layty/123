using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using 三相智慧能源网关调试软件.Common;
using 三相智慧能源网关调试软件.Helpers;
using 三相智慧能源网关调试软件.Model.IIC;

namespace 三相智慧能源网关调试软件.ViewModels
{
    /*
        联系邮箱：694965217@qq.com
        创建时间：2020/05/14 16:06:49
        主要用途：用于三相网关IIC通道数据分析，需要专门的baseMeter程序
        更改记录：
    */
    public class IicDataViewModel : ObservableObject
    {
        public ObservableCollection<IicInstantData> InstantDataCollection
        {
            get => _instantDataCollection;
            set
            {
                _instantDataCollection = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<IicInstantData> _instantDataCollection;

        public ObservableCollection<IicEnergyData> CurrentEnergyDataCollection
        {
            get => _currentEnergyDataCollection;
            set
            {
                _currentEnergyDataCollection = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<IicEnergyData> _currentEnergyDataCollection;

        public ObservableCollection<IicEnergyData> Last1EnergyDataCollection
        {
            get => _last1EnergyDataCollection;
            set
            {
                _last1EnergyDataCollection = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<IicEnergyData> _last1EnergyDataCollection;

        public ObservableCollection<IicEnergyData> Last2EnergyDataCollection
        {
            get => _last2EnergyDataCollection;
            set
            {
                _last2EnergyDataCollection = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<IicEnergyData> _last2EnergyDataCollection;

        /// <summary>
        /// IIC当前需量数据集合
        /// </summary>
        public ObservableCollection<IicDemandData> CurrentDemandDataCollection
        {
            get => _currentDemandDataCollection;
            set
            {
                _currentDemandDataCollection = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<IicDemandData> _currentDemandDataCollection;

        /// <summary>
        /// IIC上一月需量数据集合
        /// </summary>
        public ObservableCollection<IicDemandData> Last1DemandDataCollection
        {
            get => _last1DemandDataCollection;
            set
            {
                _last1DemandDataCollection = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<IicDemandData> _last1DemandDataCollection;

        /// <summary>
        /// IIC上二月需量数据集合
        /// </summary>
        public ObservableCollection<IicDemandData> Last2DemandDataCollection
        {
            get => _last2DemandDataCollection;
            set
            {
                _last2DemandDataCollection = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<IicDemandData> _last2DemandDataCollection;


        public ObservableCollection<IicHarmonicData> UaHarmonicDataCollection
        {
            get => _uaHarmonicDataCollection;
            set
            {
                _uaHarmonicDataCollection = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<IicHarmonicData> _uaHarmonicDataCollection;

        public ObservableCollection<IicHarmonicData> UbHarmonicDataCollection
        {
            get => _ubHarmonicDataCollection;
            set
            {
                _ubHarmonicDataCollection = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<IicHarmonicData> _ubHarmonicDataCollection;

        public ObservableCollection<IicHarmonicData> UcHarmonicDataCollection
        {
            get => _ucHarmonicDataCollection;
            set
            {
                _ucHarmonicDataCollection = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<IicHarmonicData> _ucHarmonicDataCollection;

        public ObservableCollection<IicHarmonicData> IaHarmonicDataCollection
        {
            get => _iaHarmonicDataCollection;
            set
            {
                _iaHarmonicDataCollection = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<IicHarmonicData> _iaHarmonicDataCollection;

        public ObservableCollection<IicHarmonicData> IbHarmonicDataCollection
        {
            get => _ibHarmonicDataCollection;
            set
            {
                _ibHarmonicDataCollection = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<IicHarmonicData> _ibHarmonicDataCollection;

        public ObservableCollection<IicHarmonicData> IcHarmonicDataCollection
        {
            get => _icHarmonicDataCollection;
            set
            {
                _icHarmonicDataCollection = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<IicHarmonicData> _icHarmonicDataCollection;


        public ObservableCollection<IicParameter> IicParameterDataCollection
        {
            get => _iicParameterDataCollection;
            set
            {
                _iicParameterDataCollection = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<IicParameter> _iicParameterDataCollection;


        private IicInstantData _instantData = new IicInstantData();

        IicEnergyData _iicCurrentEnergyData = new IicEnergyData();
        IicEnergyData _iicLast1EnergyData = new IicEnergyData();
        IicEnergyData _iicLast2EnergyData = new IicEnergyData();

        IicDemandData _iicCurrentDemandData = new IicDemandData();
        IicDemandData _last1DemandData = new IicDemandData();
        IicDemandData _last2DemandData = new IicDemandData();

        IicHarmonicData _iicHarmonicDataUa = new IicHarmonicData();
        IicHarmonicData _iicHarmonicDataUb = new IicHarmonicData();
        IicHarmonicData _iicHarmonicDataUc = new IicHarmonicData();
        IicHarmonicData _iicHarmonicDataIa = new IicHarmonicData();
        IicHarmonicData _iicHarmonicDataIb = new IicHarmonicData();
        IicHarmonicData _iicHarmonicDataIc = new IicHarmonicData();

        IicParameter _iicParameter = new IicParameter();
        public RelayCommand ClearCommand { get; set; }

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
            IicParameterDataCollection = new ObservableCollection<IicParameter>();

            StrongReferenceMessenger.Default.Register<Tuple<Socket, byte[]>, string>(typeof(IicDataViewModel),
                "ClientReceiveDataEvent", HandlerData);
            ClearCommand = new RelayCommand(() =>
            {
                InstantDataCollection.Clear();
                CurrentEnergyDataCollection.Clear();
                Last1EnergyDataCollection.Clear();
                Last2EnergyDataCollection.Clear();
                CurrentDemandDataCollection.Clear();
                Last1DemandDataCollection.Clear();
                Last2DemandDataCollection.Clear();
                UaHarmonicDataCollection.Clear();
                UbHarmonicDataCollection.Clear();
                UcHarmonicDataCollection.Clear();
                IaHarmonicDataCollection.Clear();
                IbHarmonicDataCollection.Clear();
                IcHarmonicDataCollection.Clear();
                IicParameterDataCollection.Clear();
            });
        }

        private void HandlerData(object sender, Tuple<Socket, byte[]> mTuple)
        {
            var stringData = Encoding.Default.GetString(mTuple.Item2);
            var bb = stringData.Split('\n');
            if (bb.Length == 3)
            {
                var bbb = bb[1].Replace('\r', ' ');
                if (bbb.Length != 0)
                {
                    var bytes = bbb.StringToByte();
                    var dataType = BitConverter.ToUInt16(bytes.Take(2).Reverse().ToArray(), 0);
                    if (dataType == (ushort)IicDataType.IicInstantData)
                    {
                        _instantData = new IicInstantData();
                        if (_instantData.ParseData(bbb))
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() => { InstantDataCollection.Add(_instantData); });
                        }
                    }
                    else if (dataType == (ushort)IicDataType.IicCurrentEnergyData)
                    {
                        _iicCurrentEnergyData = new IicEnergyData();
                        if (_iicCurrentEnergyData.ParseData(bytes))
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                CurrentEnergyDataCollection.Add(_iicCurrentEnergyData);
                            });
                        }
                    }
                    else if (dataType == (ushort)IicDataType.IicLast1EnergyData)
                    {
                        _iicLast1EnergyData = new IicEnergyData();
                        if (_iicLast1EnergyData.ParseData(bytes))
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                Last1EnergyDataCollection.Add(_iicLast1EnergyData);
                            });
                        }
                    }
                    else if (dataType == (ushort)IicDataType.IicLast2EnergyData)
                    {
                        _iicLast2EnergyData = new IicEnergyData();
                        if (_iicLast2EnergyData.ParseData(bytes))
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                Last2EnergyDataCollection.Add(_iicLast2EnergyData);
                            });
                        }
                    }
                    else if (dataType == (ushort)IicDataType.IicCurrentDemandData)
                    {
                        _iicCurrentDemandData = new IicDemandData();
                        if (_iicCurrentDemandData.ParseData(bytes))
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                CurrentDemandDataCollection.Add(_iicCurrentDemandData);
                            });
                        }
                    }
                    else if (dataType == (ushort)IicDataType.IicLast1DemandData)
                    {
                        _last1DemandData = new IicDemandData();
                        if (_last1DemandData.ParseData(bytes))
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                Last1DemandDataCollection.Add(_last1DemandData);
                            });
                        }
                    }
                    else if (dataType == (ushort)IicDataType.IicLast2DemandData)
                    {
                        _last2DemandData = new IicDemandData();
                        if (_last2DemandData.ParseData(bytes))
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                Last2DemandDataCollection.Add(_last2DemandData);
                            });
                        }
                    }
                    else if (dataType == (ushort)IicDataType.IicHarmonicData)
                    {
                        _iicHarmonicDataUa = new IicHarmonicData();
                        _iicHarmonicDataUb = new IicHarmonicData();
                        _iicHarmonicDataUc = new IicHarmonicData();
                        _iicHarmonicDataIa = new IicHarmonicData();
                        _iicHarmonicDataIb = new IicHarmonicData();
                        _iicHarmonicDataIc = new IicHarmonicData();
                        var resultUa = _iicHarmonicDataUa.ParseData(bytes.Skip(2).Take(42).ToArray());
                        var resultUb = _iicHarmonicDataUb.ParseData(bytes.Skip(44).Take(42).ToArray());
                        var resultUc = _iicHarmonicDataUc.ParseData(bytes.Skip(86).Take(42).ToArray());
                        var resultIa = _iicHarmonicDataIa.ParseData(bytes.Skip(128).Take(42).ToArray());
                        var resultIb = _iicHarmonicDataIb.ParseData(bytes.Skip(170).Take(42).ToArray());
                        var resultIc = _iicHarmonicDataIc.ParseData(bytes.Skip(212).Take(42).ToArray());
                        if (resultUa && resultUb && resultUc && resultIa && resultIb && resultIc)
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                UaHarmonicDataCollection.Add(_iicHarmonicDataUa);
                                UbHarmonicDataCollection.Add(_iicHarmonicDataUb);
                                UcHarmonicDataCollection.Add(_iicHarmonicDataUc);
                                IaHarmonicDataCollection.Add(_iicHarmonicDataIa);
                                IbHarmonicDataCollection.Add(_iicHarmonicDataIb);
                                IcHarmonicDataCollection.Add(_iicHarmonicDataIc);
                            });
                        }
                    }
                    else if (dataType == (ushort)IicDataType.IicParameterData)
                    {
                        _iicParameter = new IicParameter();
                        if (_iicParameter.ParseData(bytes))
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                IicParameterDataCollection.Add(_iicParameter);
                            });
                        }
                    }
                }
            }
        }
    }
}