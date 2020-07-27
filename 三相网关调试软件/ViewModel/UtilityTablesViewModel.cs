using System.Collections.ObjectModel;
using System.Text;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MySerialPortMaster;
using Newtonsoft.Json;
using 三相智慧能源网关调试软件.DLMS;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.Model;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class UtilityTablesViewModel : ViewModelBase
    {
        public class DiYaGuiDataModel : ObservableObject
        {
            public byte? MeterId
            {
                get => _meterId;
                set
                {
                    _meterId = value;
                    RaisePropertyChanged();
                }
            }

            private byte? _meterId;


            public float? Ua
            {
                get => _Ua;
                set
                {
                    _Ua = value;
                    RaisePropertyChanged();
                }
            }

            private float? _Ua;

            public float? Ub
            {
                get => _Ub;
                set
                {
                    _Ub = value;
                    RaisePropertyChanged();
                }
            }

            private float? _Ub;

            public float? Uc
            {
                get => _Uc;
                set
                {
                    _Uc = value;
                    RaisePropertyChanged();
                }
            }

            private float? _Uc;


            public float? Ia
            {
                get => _Ia;
                set
                {
                    _Ia = value;
                    RaisePropertyChanged();
                }
            }

            private float? _Ia;

            public float? Ib
            {
                get => _Ib;
                set
                {
                    _Ib = value;
                    RaisePropertyChanged();
                }
            }

            private float? _Ib;


            public float? Ic
            {
                get => _Ic;
                set
                {
                    _Ic = value;
                    RaisePropertyChanged();
                }
            }

            private float? _Ic;


            public float? remain
            {
                get => _remain;
                set
                {
                    _remain = value;
                    RaisePropertyChanged();
                }
            }

            private float? _remain;


            public float? PPTotal
            {
                get => _PPTotal;
                set
                {
                    _PPTotal = value;
                    RaisePropertyChanged();
                }
            }

            private float? _PPTotal;

            public float? PPA
            {
                get => _PPA;
                set
                {
                    _PPA = value;
                    RaisePropertyChanged();
                }
            }

            private float? _PPA;


            public float? PPB
            {
                get => _PPB;
                set
                {
                    _PPB = value;
                    RaisePropertyChanged();
                }
            }

            private float? _PPB;

            public float? PPC
            {
                get => _PPC;
                set
                {
                    _PPC = value;
                    RaisePropertyChanged();
                }
            }

            private float? _PPC;


            public float? PQTotal
            {
                get => _PQTotal;
                set
                {
                    _PQTotal = value;
                    RaisePropertyChanged();
                }
            }

            private float? _PQTotal;


            public float? PQA
            {
                get => _PQA;
                set
                {
                    _PQA = value;
                    RaisePropertyChanged();
                }
            }

            private float? _PQA;

            public float? PQB
            {
                get => _PQB;
                set
                {
                    _PQB = value;
                    RaisePropertyChanged();
                }
            }

            private float? _PQB;


            public float? PQC
            {
                get => _PQC;
                set
                {
                    _PQC = value;
                    RaisePropertyChanged();
                }
            }

            private float? _PQC;

            public float? DemandP
            {
                get => _DemandP;
                set
                {
                    _DemandP = value;
                    RaisePropertyChanged();
                }
            }

            private float? _DemandP;

            public float? DemandQ
            {
                get => _DemandQ;
                set
                {
                    _DemandQ = value;
                    RaisePropertyChanged();
                }
            }

            private float? _DemandQ;


            public float? PTotal
            {
                get => _PTotal;
                set
                {
                    _PTotal = value;
                    RaisePropertyChanged();
                }
            }

            private float? _PTotal;

            public float? Pa
            {
                get => _Pa;
                set
                {
                    _Pa = value;
                    RaisePropertyChanged();
                }
            }

            private float? _Pa;

            public float? Pb
            {
                get => _Pb;
                set
                {
                    _Pb = value;
                    RaisePropertyChanged();
                }
            }

            private float? _Pb;

            public float? Pc
            {
                get => _Pc;
                set
                {
                    _Pc = value;
                    RaisePropertyChanged();
                }
            }

            private float? _Pc;


            public float? QTotal
            {
                get => _QTotal;
                set
                {
                    _QTotal = value;
                    RaisePropertyChanged();
                }
            }

            private float? _QTotal;

            public float? Qa
            {
                get => _Qa;
                set
                {
                    _Qa = value;
                    RaisePropertyChanged();
                }
            }

            private float? _Qa;

            public float? Qb
            {
                get => _Qb;
                set
                {
                    _Qb = value;
                    RaisePropertyChanged();
                }
            }

            private float? _Qb;

            public float? Qc
            {
                get => _Qc;
                set
                {
                    _Qc = value;
                    RaisePropertyChanged();
                }
            }

            private float? _Qc;

            public float? STotal
            {
                get => _STotal;
                set
                {
                    _STotal = value;
                    RaisePropertyChanged();
                }
            }

            private float? _STotal;

            public float? Sa
            {
                get => _Sa;
                set
                {
                    _Sa = value;
                    RaisePropertyChanged();
                }
            }

            private float? _Sa;

            public float? Sb
            {
                get => _Sb;
                set
                {
                    _Sb = value;
                    RaisePropertyChanged();
                }
            }

            private float? _Sb;


            public float? Sc
            {
                get => _Sc;
                set
                {
                    _Sc = value;
                    RaisePropertyChanged();
                }
            }

            private float? _Sc;


            public float? CosTotal
            {
                get => _CosTotal;
                set
                {
                    _CosTotal = value;
                    RaisePropertyChanged();
                }
            }

            private float? _CosTotal;


            public float? Cosa
            {
                get => _Cosa;
                set
                {
                    _Cosa = value;
                    RaisePropertyChanged();
                }
            }

            private float? _Cosa;


            public float? Cosb
            {
                get => _Cosb;
                set
                {
                    _Cosb = value;
                    RaisePropertyChanged();
                }
            }

            private float? _Cosb;

            public float? Cosc
            {
                get => _Cosc;
                set
                {
                    _Cosc = value;
                    RaisePropertyChanged();
                }
            }

            private float? _Cosc;

            public float? TempA
            {
                get => _TempA;
                set
                {
                    _TempA = value;
                    RaisePropertyChanged();
                }
            }

            private float? _TempA;


            public float? TempB
            {
                get => _TempB;
                set
                {
                    _TempB = value;
                    RaisePropertyChanged();
                }
            }

            private float? _TempB;


            public float? TempC
            {
                get => _TempC;
                set
                {
                    _TempC = value;
                    RaisePropertyChanged();
                }
            }

            private float? _TempC;


            public float? Breaker
            {
                get => _Breaker;
                set
                {
                    _Breaker = value;
                    RaisePropertyChanged();
                }
            }

            private float? _Breaker;

            public float? AlarmStatus
            {
                get => _AlarmStatus;
                set
                {
                    _AlarmStatus = value;
                    RaisePropertyChanged();
                }
            }

            private float? _AlarmStatus;


            public float? ThresholdP
            {
                get => _ThresholdP;
                set
                {
                    _ThresholdP = value;
                    RaisePropertyChanged();
                }
            }

            private float? _ThresholdP;

            public float? ThresholdI
            {
                get => _ThresholdI;
                set
                {
                    _ThresholdI = value;
                    RaisePropertyChanged();
                }
            }

            private float? _ThresholdI;

            public float? threshold_temp
            {
                get => _threshold_temp;
                set
                {
                    _threshold_temp = value;
                    RaisePropertyChanged();
                }
            }

            private float? _threshold_temp;

            public float? NominalI
            {
                get => _NominalI;
                set
                {
                    _NominalI = value;
                    RaisePropertyChanged();
                }
            }

            private float? _NominalI;
        }

        public DLMSClient Client { get; set; }

        private ObservableCollection<DLMSSelfDefineUtilityTablesModel> _utilityTablesCollection;

        public ObservableCollection<DLMSSelfDefineUtilityTablesModel> UtilityTablesCollection
        {
            get => _utilityTablesCollection;
            set
            {
                _utilityTablesCollection = value;
                RaisePropertyChanged();
            }
        }


        public ObservableCollection<DiYaGuiDataModel> DiYaGuiDataModels
        {
            get => _DiYaGuiDataModels;
            set
            {
                _DiYaGuiDataModels = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<DiYaGuiDataModel> _DiYaGuiDataModels;

        public UtilityTablesViewModel()
        {
            if (IsInDesignMode)
            {
                UtilityTablesCollection = new ObservableCollection<DLMSSelfDefineUtilityTablesModel>
                {
                    new DLMSSelfDefineUtilityTablesModel {LogicalName = "1.1.98.0.128.255"},
                    new DLMSSelfDefineUtilityTablesModel {LogicalName = "1.2.98.0.128.255"},
                    new DLMSSelfDefineUtilityTablesModel {LogicalName = "1.3.98.0.128.255"},
                    new DLMSSelfDefineUtilityTablesModel {LogicalName = "1.4.98.0.128.255"}
                };
            }
            else
            {
                DiYaGuiDataModels = new ObservableCollection<DiYaGuiDataModel>();
                Client = CommonServiceLocator.ServiceLocator.Current.GetInstance<DLMSClient>();

                ExcelHelper excel = new ExcelHelper("DLMS设备信息.xls");
                var dataTable = excel.GetExcelDataTable("UtilityTables$");

                UtilityTablesCollection = new ObservableCollection<DLMSSelfDefineUtilityTablesModel>();
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    UtilityTablesCollection.Add(new DLMSSelfDefineUtilityTablesModel()
                        {LogicalName = dataTable.Rows[i][0].ToString(), Name = dataTable.Rows[i][1].ToString()});
                }

                GetLogicNameDataCommand = new RelayCommand<DLMSSelfDefineUtilityTablesModel>(
                    async t =>
                    {
                        t.DataForShow = "";
                        var dataResult = await Client.GetRequest(t.GetLogicName());
                        GetResponse getResponse = new GetResponse();
                        if (getResponse.PduBytesToConstructor(dataResult))
                        {
                            if (getResponse.GetResponseNormal.GetDataResult.Data.DataType == DataType.OctetString)
                            {
                                t.DataForShow = NormalDataParse.HowToDisplayOctetString(
                                    getResponse.GetResponseNormal.GetDataResult.Data.ValueBytes,
                                    DisplayFormatToShow.OBIS);
                            }
                        }
                    });
                GetMeterAddressData = new RelayCommand<DLMSSelfDefineUtilityTablesModel>(async t =>
                    {
                        t.DataForShow = "";
                        var dataResult = await Client.GetRequest(t.GetTableId());
                        t.DataForShow = NormalDataParse.ParsePduData(dataResult);
                    }
                );
                GetDataLengthData = new RelayCommand<DLMSSelfDefineUtilityTablesModel>(async t =>
                    {
                        t.DataForShow = "";
                        var dataResult = await Client.GetRequest(t.GetLength());
                        t.DataForShow = NormalDataParse.ParsePduData(dataResult);
                    }
                );
                GetBuffData = new RelayCommand<DLMSSelfDefineUtilityTablesModel>(async t =>
                    {
                        t.DataForShow = "";
                        var dataResult = await Client.GetRequest(t.GetBuffer());
                        t.DataForShow =
                            NormalDataParse.ParsePduData(dataResult);
                        var d = JsonConvert.DeserializeObject(t.DataForShow, typeof(DiYaGuiDataModel));
                        var daa = d as DiYaGuiDataModel;
                        {
                            if (daa != null)
                            {
                                DiYaGuiDataModels.Add(daa);
                            }
                        }
                    }
                );
            }
        }

        private RelayCommand<DLMSSelfDefineUtilityTablesModel> _getLogicNameDataCommand;

        public RelayCommand<DLMSSelfDefineUtilityTablesModel> GetLogicNameDataCommand
        {
            get => _getLogicNameDataCommand;
            set
            {
                _getLogicNameDataCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand<DLMSSelfDefineUtilityTablesModel> _etMeterAddressData;

        public RelayCommand<DLMSSelfDefineUtilityTablesModel> GetMeterAddressData
        {
            get => _etMeterAddressData;
            set
            {
                _etMeterAddressData = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand<DLMSSelfDefineUtilityTablesModel> _getDataLengthData;

        public RelayCommand<DLMSSelfDefineUtilityTablesModel> GetDataLengthData
        {
            get => _getDataLengthData;
            set
            {
                _getDataLengthData = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand<DLMSSelfDefineUtilityTablesModel> _getBuffData;

        public RelayCommand<DLMSSelfDefineUtilityTablesModel> GetBuffData
        {
            get => _getBuffData;
            set
            {
                _getBuffData = value;
                RaisePropertyChanged();
            }
        }
    }
}