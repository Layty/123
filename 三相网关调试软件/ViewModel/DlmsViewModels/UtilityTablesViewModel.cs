using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Newtonsoft.Json;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.Get;
using 三相智慧能源网关调试软件.Model;

namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{
    public class UtilityTablesViewModel : ObservableObject
    {
        public class DiYaGuiDataModel : ObservableObject
        {
            public byte? MeterId
            {
                get => _meterId;
                set
                {
                    _meterId = value;
                    OnPropertyChanged();
                }
            }

            private byte? _meterId;


            public float? Ua
            {
                get => _Ua;
                set
                {
                    _Ua = value;
                    OnPropertyChanged();
                }
            }

            private float? _Ua;

            public float? Ub
            {
                get => _Ub;
                set
                {
                    _Ub = value;
                    OnPropertyChanged();
                }
            }

            private float? _Ub;

            public float? Uc
            {
                get => _Uc;
                set
                {
                    _Uc = value;
                    OnPropertyChanged();
                }
            }

            private float? _Uc;


            public float? Ia
            {
                get => _Ia;
                set
                {
                    _Ia = value;
               
                    OnPropertyChanged();
                }
            }

            private float? _Ia;

            public float? Ib
            {
                get => _Ib;
                set
                {
                    _Ib = value;
                    OnPropertyChanged();
                }
            }

            private float? _Ib;


            public float? Ic
            {
                get => _Ic;
                set
                {
                    _Ic = value;
                    OnPropertyChanged();
                }
            }

            private float? _Ic;


            public float? remain
            {
                get => _remain;
                set
                {
                    _remain = value;
                    OnPropertyChanged();
                }
            }

            private float? _remain;


            public float? PPTotal
            {
                get => _PPTotal;
                set
                {
                    _PPTotal = value;
                    OnPropertyChanged();
                }
            }

            private float? _PPTotal;

            public float? PPA
            {
                get => _PPA;
                set
                {
                    _PPA = value;
                    OnPropertyChanged();
                }
            }

            private float? _PPA;


            public float? PPB
            {
                get => _PPB;
                set
                {
                    _PPB = value;
                    OnPropertyChanged();
                }
            }

            private float? _PPB;

            public float? PPC
            {
                get => _PPC;
                set
                {
                    _PPC = value;
                    OnPropertyChanged();
                }
            }

            private float? _PPC;


            public float? PQTotal
            {
                get => _PQTotal;
                set
                {
                    _PQTotal = value;
                    OnPropertyChanged();
                }
            }

            private float? _PQTotal;


            public float? PQA
            {
                get => _PQA;
                set
                {
                    _PQA = value;
                    OnPropertyChanged();
                }
            }

            private float? _PQA;

            public float? PQB
            {
                get => _PQB;
                set
                {
                    _PQB = value;
                    OnPropertyChanged();
                }
            }

            private float? _PQB;


            public float? PQC
            {
                get => _PQC;
                set
                {
                    _PQC = value;
                    OnPropertyChanged();
                }
            }

            private float? _PQC;

            public float? DemandP
            {
                get => _DemandP;
                set
                {
                    _DemandP = value;
                    OnPropertyChanged();
                }
            }

            private float? _DemandP;

            public float? DemandQ
            {
                get => _DemandQ;
                set
                {
                    _DemandQ = value;
                    OnPropertyChanged();
                }
            }

            private float? _DemandQ;


            public float? PTotal
            {
                get => _PTotal;
                set
                {
                    _PTotal = value;
                    OnPropertyChanged();
                }
            }

            private float? _PTotal;

            public float? Pa
            {
                get => _Pa;
                set
                {
                    _Pa = value;
                    OnPropertyChanged();
                }
            }

            private float? _Pa;

            public float? Pb
            {
                get => _Pb;
                set
                {
                    _Pb = value;
                    OnPropertyChanged();
                }
            }

            private float? _Pb;

            public float? Pc
            {
                get => _Pc;
                set
                {
                    _Pc = value;
                    OnPropertyChanged();
                }
            }

            private float? _Pc;


            public float? QTotal
            {
                get => _QTotal;
                set
                {
                    _QTotal = value;
                    OnPropertyChanged();
                }
            }

            private float? _QTotal;

            public float? Qa
            {
                get => _Qa;
                set
                {
                    _Qa = value;
                    OnPropertyChanged();
                }
            }

            private float? _Qa;

            public float? Qb
            {
                get => _Qb;
                set
                {
                    _Qb = value;
                    OnPropertyChanged();
                }
            }

            private float? _Qb;

            public float? Qc
            {
                get => _Qc;
                set
                {
                    _Qc = value;
                    OnPropertyChanged();
                }
            }

            private float? _Qc;

            public float? STotal
            {
                get => _STotal;
                set
                {
                    _STotal = value;
                    OnPropertyChanged();
                }
            }

            private float? _STotal;

            public float? Sa
            {
                get => _Sa;
                set
                {
                    _Sa = value;
                    OnPropertyChanged();
                }
            }

            private float? _Sa;

            public float? Sb
            {
                get => _Sb;
                set
                {
                    _Sb = value;
                    OnPropertyChanged();
                }
            }

            private float? _Sb;


            public float? Sc
            {
                get => _Sc;
                set
                {
                    _Sc = value;
                    OnPropertyChanged();
                }
            }

            private float? _Sc;


            public float? CosTotal
            {
                get => _CosTotal;
                set
                {
                    _CosTotal = value;
                    OnPropertyChanged();
                }
            }

            private float? _CosTotal;


            public float? Cosa
            {
                get => _Cosa;
                set
                {
                    _Cosa = value;
                    OnPropertyChanged();
                }
            }

            private float? _Cosa;


            public float? Cosb
            {
                get => _Cosb;
                set
                {
                    _Cosb = value;
                    OnPropertyChanged();
                }
            }

            private float? _Cosb;

            public float? Cosc
            {
                get => _Cosc;
                set
                {
                    _Cosc = value;
                    OnPropertyChanged();
                }
            }

            private float? _Cosc;

            public float? TempA
            {
                get => _TempA;
                set
                {
                    _TempA = value;
                    OnPropertyChanged();
                }
            }

            private float? _TempA;


            public float? TempB
            {
                get => _TempB;
                set
                {
                    _TempB = value;
                    OnPropertyChanged();
                }
            }

            private float? _TempB;


            public float? TempC
            {
                get => _TempC;
                set
                {
                    _TempC = value;
                    OnPropertyChanged();
                }
            }

            private float? _TempC;


            public float? Breaker
            {
                get => _Breaker;
                set
                {
                    _Breaker = value;
                    OnPropertyChanged();
                }
            }

            private float? _Breaker;

            public float? AlarmStatus
            {
                get => _AlarmStatus;
                set
                {
                    _AlarmStatus = value;
                    OnPropertyChanged();
                }
            }

            private float? _AlarmStatus;


            public float? ThresholdP
            {
                get => _ThresholdP;
                set
                {
                    _ThresholdP = value;
                    OnPropertyChanged();
                }
            }

            private float? _ThresholdP;

            public float? ThresholdI
            {
                get => _ThresholdI;
                set
                {
                    _ThresholdI = value;
                    OnPropertyChanged();
                }
            }

            private float? _ThresholdI;

            public float? threshold_temp
            {
                get => _threshold_temp;
                set
                {
                    _threshold_temp = value;
                    OnPropertyChanged();
                }
            }

            private float? _threshold_temp;

            public float? NominalI
            {
                get => _NominalI;
                set
                {
                    _NominalI = value;
                    OnPropertyChanged();
                }
            }

            private float? _NominalI;
        }

        public DLMSClient Client { get; set; }

        private ObservableCollection<CustomCosemUtilityTablesModel> _utilityTablesCollection;

        public ObservableCollection<CustomCosemUtilityTablesModel> UtilityTablesCollection
        {
            get => _utilityTablesCollection;
            set
            {
                _utilityTablesCollection = value;
                OnPropertyChanged();
            }
        }


        public ObservableCollection<DiYaGuiDataModel> DiYaGuiDataModels
        {
            get => _DiYaGuiDataModels;
            set
            {
                _DiYaGuiDataModels = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<DiYaGuiDataModel> _DiYaGuiDataModels;
  

        public UtilityTablesViewModel()
        {
//            if (IsInDesignMode)
//            {
//                UtilityTablesCollection = new ObservableCollection<CustomCosemUtilityTablesModel>
//                {
//                    new CustomCosemUtilityTablesModel {LogicalName = "1.1.98.0.128.255"},
//                    new CustomCosemUtilityTablesModel {LogicalName = "1.2.98.0.128.255"},
//                    new CustomCosemUtilityTablesModel {LogicalName = "1.3.98.0.128.255"},
//                    new CustomCosemUtilityTablesModel {LogicalName = "1.4.98.0.128.255"}
//                };
//            }
//            else
            {
                DiYaGuiDataModels = new ObservableCollection<DiYaGuiDataModel>();
                Client = CommonServiceLocator.ServiceLocator.Current.GetInstance<DLMSClient>();

                ExcelHelper excel = new ExcelHelper("DLMS设备信息.xls");
                var dataTable = excel.GetExcelDataTable("UtilityTables$");

                UtilityTablesCollection = new ObservableCollection<CustomCosemUtilityTablesModel>();
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    UtilityTablesCollection.Add(new CustomCosemUtilityTablesModel()
                        {LogicalName = dataTable.Rows[i][0].ToString(), Name = dataTable.Rows[i][1].ToString()});
                }

                GetLogicNameDataCommand = new RelayCommand<CustomCosemUtilityTablesModel>(
                    async t =>
                    {
                        t.DataForShow = "";
                        var dataResult = await Client.GetRequest(t.GetLogicNameAttributeDescriptor());
                        GetResponse getResponse = new GetResponse();
                        var data = dataResult.ByteToString("");

                        if (getResponse.PduStringInHexConstructor(ref data))
                        {
                            if (getResponse.GetResponseNormal.Result.Data.DataType == DataType.OctetString)
                            {
                                t.DataForShow = NormalDataParse.HowToDisplayOctetString(
                                    getResponse.GetResponseNormal.Result.Data.Value.ToString().StringToByte(),
                                    OctetStringDisplayFormat.Obis);
                            }
                        }
                    });
                GetMeterAddressData = new RelayCommand<CustomCosemUtilityTablesModel>(async t =>
                    {
                        t.DataForShow = "";
                        var dataResult = await Client.GetRequest(t.GetTableIdAttributeDescriptor());
                        t.DataForShow = NormalDataParse.ParsePduData(dataResult);
                    }
                );
                GetDataLengthData = new RelayCommand<CustomCosemUtilityTablesModel>(async t =>
                    {
                        t.DataForShow = "";
                        var dataResult = await Client.GetRequest(t.GetLengthAttributeDescriptor());
                        t.DataForShow = NormalDataParse.ParsePduData(dataResult);
                    }
                );
                GetBuffData = new RelayCommand<CustomCosemUtilityTablesModel>(async t =>
                    {
                        t.DataForShow = "";
                   
                        var dataResult = await Client.GetRequest(t.GetBufferAttributeDescriptor());
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

        private RelayCommand<CustomCosemUtilityTablesModel> _getLogicNameDataCommand;

        public RelayCommand<CustomCosemUtilityTablesModel> GetLogicNameDataCommand
        {
            get => _getLogicNameDataCommand;
            set
            {
                _getLogicNameDataCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand<CustomCosemUtilityTablesModel> _etMeterAddressData;

        public RelayCommand<CustomCosemUtilityTablesModel> GetMeterAddressData
        {
            get => _etMeterAddressData;
            set
            {
                _etMeterAddressData = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand<CustomCosemUtilityTablesModel> _getDataLengthData;

        public RelayCommand<CustomCosemUtilityTablesModel> GetDataLengthData
        {
            get => _getDataLengthData;
            set
            {
                _getDataLengthData = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand<CustomCosemUtilityTablesModel> _getBuffData;

        public RelayCommand<CustomCosemUtilityTablesModel> GetBuffData
        {
            get => _getBuffData;
            set
            {
                _getBuffData = value;
                OnPropertyChanged();
            }
        }
    }
}