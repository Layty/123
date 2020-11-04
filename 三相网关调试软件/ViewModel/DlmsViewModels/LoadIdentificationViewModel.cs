﻿using System;
using System.Linq;
using System.Windows;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.Get;

namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{
    public class LoadIdentificationViewModel : ObservableObject
    {
        public DLMSClient Client { get; set; }
        public CosemLoadIdentification CosemLoadIdentification { get; set; }
        public string LoadOriginalDataHex
        {
            get => _loadOriginalDataHex;
            set
            {
                _loadOriginalDataHex = value;
                OnPropertyChanged();
            }
        }

        private string _loadOriginalDataHex;

        public string LoadDataFormat
        {
            get => _loadDataFormat;
            set
            {
                _loadDataFormat = value;
                OnPropertyChanged();
            }
        }

        private string _loadDataFormat;

        public class LoadDataParse
        {
            public string End = "E5";
            public string Start = "A0A0";

            public string DataTime;
            public string Energy;
            public string MeterCount;
            private MeterRecordUnit[] meterRecordUnit;

            public class MeterRecordUnit
            {
                public string MeterType { get; set; }
                public string InnerNum { get; set; }
                public string MeterUsedEnergy { get; set; }
                public string StartMin { get; set; }
                public string StopMin { get; set; }
                public string TopPower { get; set; }
                public string CombineNum { get; set; }
                public string Reserve { get; set; }
                public string SplitAa { get; set; }
            }

            public LoadDataParse()
            {
            }

            public override string ToString()
            {
                var meterRecord = "";
                if (meterRecordUnit != null)
                {
                    foreach (var recordUnit in meterRecordUnit)
                    {
                        meterRecord += $"电器类型{OverTurnData(recordUnit.MeterType)} " +
                                       $"内部编码{OverTurnData(recordUnit.InnerNum)} " +
                                       $"该电器用电量{OverTurnData(recordUnit.MeterUsedEnergy)} " +
                                       $"启动分钟数{OverTurnData(recordUnit.StartMin)} " +
                                       $"停止分钟数{OverTurnData(recordUnit.StopMin)} " +
                                       $"该电器峰值功率{OverTurnData(recordUnit.TopPower)} " +
                                       $"合并启停数{OverTurnData(recordUnit.CombineNum)} " +
                                       $"预留其他电器特征{OverTurnData(recordUnit.Reserve)} " +
                                       $"块分割码{OverTurnData(recordUnit.SplitAa)}\r\n";
                    }
                }

                return $"存储时间{OverTurnData(DataTime)}\r\n" +
                       $"周期内总电量{OverTurnData(Energy)}\r\n" +
                       $"电器设备数量{OverTurnData(MeterCount)}\r\n" + meterRecord
                    ;
            }

            private string OverTurnData(string s)
            {
                if (s.Length % 2 != 0)
                {
                    return "";
                }

                return s.StringToByte().Reverse().ToArray().ByteToString(" ");
            }

            public bool Parse(string str)
            {
                if (str.Substring(0, 4) != Start)
                {
                    return false;
                }

                var len = Convert.ToByte(str.Substring(6).Length / 2);
                var lenCount = Convert.ToByte(str.Substring(4, 2), 16);
                if (len != lenCount)
                {
                    MessageBox.Show($"长度不匹配, 报文指示长度：{lenCount} , 帧内容实际长度：{len}");
                    return false;
                }

                if (!str.EndsWith("E5"))
                {
                    return false;
                }

                DataTime = str.Substring(6, 10);
                Energy = str.Substring(16, 8);
                MeterCount = str.Substring(24, 2);
                if (MeterCount != "00")
                {
                    str = str.Substring(26);
                    int num = int.Parse(MeterCount);
                    meterRecordUnit = new MeterRecordUnit[num];
                    for (int i = 0; i < num; i++)
                    {
                        meterRecordUnit[i] = new MeterRecordUnit()
                        {
                            MeterType = str.Substring(0, 6),
                            InnerNum = str.Substring(6, 2),
                            MeterUsedEnergy = str.Substring(8, 8),
                            StartMin = str.Substring(16, 2),
                            StopMin = str.Substring(18, 2),
                            TopPower = str.Substring(20, 6),
                            CombineNum = str.Substring(26, 2),
                            Reserve = str.Substring(28, 4),
                            SplitAa = str.Substring(32, 2)
                        };
                        str = str.Substring(34);
                    }
                }

                return true;
            }
        }

        private void ParseData(GetResponse loadDataBytes)
        {
            if (loadDataBytes != null)
            {
                LoadOriginalDataHex = loadDataBytes.GetResponseNormal.Result.Data.ValueBytes.ByteToString("");
                LoadOriginalDataHex = LoadOriginalDataHex.Substring(2); //去掉长度
                var load = new LoadDataParse();
                if (load.Parse(LoadOriginalDataHex))
                {
                    LoadDataFormat = load.ToString();
                }
            }
        }

        public LoadIdentificationViewModel()
        {
            Client = CommonServiceLocator.ServiceLocator.Current.GetInstance<DLMSClient>();
            CosemLoadIdentification = new CosemLoadIdentification();
            GetEarliestCommand = new RelayCommand(async () =>
            {
                var LoadDataBytes =
                    await Client.GetRequestAndWaitResponse(CosemLoadIdentification.GetEarliestLoadIdentification());
                ParseData(LoadDataBytes);
            });
            GetLatestCommand = new RelayCommand(async () =>
            {
                var LoadDataBytes =
                    await Client.GetRequestAndWaitResponse(CosemLoadIdentification.GetLatestLoadIdentification());
                ParseData(LoadDataBytes);
            });
            GetGivenTimeCommand = new RelayCommand<string>(async (t) =>
            {
                DateTime.TryParse(t, out var setDateTime);
                CosemClock dt = new CosemClock(setDateTime);
                var LoadDataBytes =
                    await Client.GetRequestAndWaitResponse(CosemLoadIdentification.GetLoadIdentificationWithTime(dt));
                ParseData(LoadDataBytes);
            });
        }


        public RelayCommand GetEarliestCommand
        {
            get => _getEarliestCommand;
            set
            {
                _getEarliestCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _getEarliestCommand;

        public RelayCommand GetLatestCommand
        {
            get => _getLatestCommand;
            set
            {
                _getLatestCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _getLatestCommand;

        public RelayCommand<string> GetGivenTimeCommand
        {
            get => _getGivenTimeCommand;
            set
            {
                _getGivenTimeCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand<string> _getGivenTimeCommand;
    }
}