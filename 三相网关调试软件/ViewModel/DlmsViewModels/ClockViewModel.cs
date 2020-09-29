using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects;


namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{
    public class ClockViewModel : ViewModelBase
    {
        public DLMSClient Client { get; set; }
        public CosemClock Clock { get; set; }

      


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

        public ClockViewModel()
        {
            Client = CommonServiceLocator.ServiceLocator.Current.GetInstance<DLMSClient>();
            ExcelHelper excel = new ExcelHelper("DLMS设备信息.xls");
            var dataTable = excel.GetExcelDataTable("Clock$");
            Clock = new CosemClock(dataTable.Rows[0][0].ToString());
            GetTimeCommand = new RelayCommand(async () =>
            {
                var response = await Client.GetRequestAndWaitResponse(Clock.GetTimeAttributeDescriptor());

                if (response != null)
                {
                    var DisplayFormat = OctetStringDisplayFormat.DateTime;
                    Clock.Time =
                        NormalDataParse.HowToDisplayOctetString(
                            response.GetResponseNormal.Result.Data.ValueBytes, DisplayFormat);
                }
            });
            GetTimeZoneCommand = new RelayCommand(async () =>
            {
                var response = await Client.GetRequestAndWaitResponse(Clock.GetTimeZoneAttributeDescriptor());
                if (response != null)
                {
                    Clock.TimeZone = int.Parse(response.GetResponseNormal.Result.Data.ValueDisplay.ValueString);
                }
            });
        }
    }
}