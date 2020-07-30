using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using 三相智慧能源网关调试软件.DLMS;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects;

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
                    var DisplayFormat = DisplayFormatToShow.DateTime;
                    Clock.Time =
                        NormalDataParse.HowToDisplayOctetString(r.GetResponseNormal.GetDataResult.Data.ValueBytes,
                            DisplayFormat);
//                   Clock.Time = r.GetResponseNormal.GetDataResult.Data.ValueString;
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
            get => _getTimeCommand;
            set
            {
                _getTimeCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _getTimeCommand;


        public RelayCommand GetTimeZoneCommand
        {
            get => _getTimeZoneCommand;
            set
            {
                _getTimeZoneCommand = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _getTimeZoneCommand;
    }
}