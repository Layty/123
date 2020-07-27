using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.ApplicationLayEnums;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects;
using 三相智慧能源网关调试软件.Model;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class ClockViewModel : ViewModelBase
    {
        public DLMSClient Client { get; set; }
        public DLMSClock Clock { get; set; }

        public ClockViewModel()
        {
            Client = CommonServiceLocator.ServiceLocator.Current.GetInstance<DLMSClient>();
            ExcelHelper excel = new ExcelHelper("DLMS设备信息.xls");
            var dataTable = excel.GetExcelDataTable("Clock$");
            Clock = new DLMSClock(dataTable.Rows[0][0].ToString());
            GetTimeCommand = new RelayCommand(async () =>
            {
                var dataResult = await Client.GetRequest(Clock.GetTime());
                var r = new GetResponse();
                ;
                if (r.PduBytesToConstructor(dataResult))
                {
                    r.GetResponseNormal.GetDataResult.Data.DisplayFormat = DisplayFormatToShow.DateTime;
                    Clock.Time = r.GetResponseNormal.GetDataResult.Data.ValueString;
                }
            });
            GetTimeZoneCommand = new RelayCommand(async () =>
            {
                var dataResult = await Client.GetRequest(Clock.GetTimeZone());
                var r = new GetResponse();
                r.PduBytesToConstructor(dataResult);
                Clock.TimeZone = int.Parse(r.GetResponseNormal.GetDataResult.Data.ValueString);
            });
        }


        public RelayCommand GetTimeCommand
        {
            get => _GetTimeCommand;
            set
            {
                _GetTimeCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _GetTimeCommand;


        public RelayCommand GetTimeZoneCommand
        {
            get => _GetTimeZoneCommand;
            set
            {
                _GetTimeZoneCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _GetTimeZoneCommand;
    }

    public class DataViewModel : ViewModelBase
    {
        public ObservableCollection<DLMSSelfDefineData> DataCollection
        {
            get => _dataCollection;
            set
            {
                _dataCollection = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<DLMSSelfDefineData> _dataCollection;

        public RelayCommand<DLMSSelfDefineData> GetLogicNameCommand
        {
            get => _getLogicNameCommand;
            set
            {
                _getLogicNameCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand<DLMSSelfDefineData> _getLogicNameCommand;

        public RelayCommand<DLMSSelfDefineData> GetValueCommand
        {
            get => _getValueCommand;
            set
            {
                _getValueCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand<DLMSSelfDefineData> _getValueCommand;

        public RelayCommand<DLMSSelfDefineData> SetValueCommand
        {
            get => _setValueCommand;
            set
            {
                _setValueCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand<DLMSSelfDefineData> _setValueCommand;

        public DLMSClient Client { get; set; }

        public enum ValueType : byte
        {
            OctetString = 9,
            Int16 = 16,
            UInt16 = 18,


            UInt32 = 6,


            UInt64 = 21,


            UInt8 = 17,
            Bcd = 13,


            BitString = 4,


            Boolean = 3,


            Date = 26,


            DateTime = 25,

            Enum = 22,


            double32 = 7,


            double64 = 8,


            Int32 = 5,


            Int64 = 20,


            Int8 = 15,


            None = 0,


            String = 10,


            StringUTF8 = 12,


            Time = 27,
        }

        public DataViewModel()
        {
            Client = CommonServiceLocator.ServiceLocator.Current.GetInstance<DLMSClient>();
            ExcelHelper excel = new ExcelHelper("DLMS设备信息.xls");
            var dataTable = excel.GetExcelDataTable("Data$");
            DataCollection = new ObservableCollection<DLMSSelfDefineData>();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                DataCollection.Add(new DLMSSelfDefineData(dataTable.Rows[i][0].ToString())
                    {DataName = dataTable.Rows[i][1].ToString()});
            }

            GetLogicNameCommand = new RelayCommand<DLMSSelfDefineData>(async t =>
            {
                t.Value = new DLMSDataItem();
                var dataResult = await Client.GetRequest(t.GetLogicName());
                GetResponse getResponse=new GetResponse();
                getResponse.GetResponseNormal.PduBytesToConstructor(dataResult);
                t.Value.ValueBytes = getResponse.GetResponseNormal.GetDataResult.Data.ValueBytes;
                t.Value.DisplayFormat = DisplayFormatToShow.OBIS;
            });
            GetValueCommand = new RelayCommand<DLMSSelfDefineData>(async t =>
            {
                t.LastResult = new ErrorCode();
                t.Value = new DLMSDataItem();
                var dataResult = await Client.GetRequest(t.GetValue());

                GetResponse getResponse = new GetResponse();

                if (getResponse.PduBytesToConstructor(dataResult))
                {
                    t.Value.DataType = getResponse.GetResponseNormal.GetDataResult.Data.DataType;
                    t.LastResult = getResponse.GetResponseNormal.GetDataResult.DataAccessResult;
                    t.Value.ValueString = getResponse.GetResponseNormal.GetDataResult.Data.ValueString;
                    t.Value.ValueBytes = getResponse.GetResponseNormal.GetDataResult.Data.ValueBytes;
                    if (t.Value.DataType== DataType.OctetString)
                    {
                        t.Value.ValueString =
                            NormalDataParse.HowToDisplayOctetString(t.Value.ValueBytes, DisplayFormatToShow.ASCII);
                    }
                  
                }
            });
            SetValueCommand = new RelayCommand<DLMSSelfDefineData>(async (t) =>
            {
                t.LastResult = new ErrorCode();
                var d = new DLMSDataItem(t.Value.DataType, t.Value.ValueString);
                await Client.SetRequest(t.SetValue(d));
            });
        }
    }
}