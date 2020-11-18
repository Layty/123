using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.CosemObjects;
using 三相智慧能源网关调试软件.Commom;


namespace 三相智慧能源网关调试软件.ViewModel.DlmsViewModels
{
    public class ClockViewModel : ObservableObject
    {
        public DlmsClient Client { get; set; }
        public CosemClock Clock { get; set; }


        public RelayCommand GetTimeCommand
        {
            get => _getTimeCommand;
            set
            {
                _getTimeCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _getTimeCommand;


        public RelayCommand GetTimeZoneCommand
        {
            get => _getTimeZoneCommand;
            set
            {
                _getTimeZoneCommand = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _getTimeZoneCommand;

        public ClockViewModel()
        {
            Client = CommonServiceLocator.ServiceLocator.Current.GetInstance<DlmsClient>();
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
                            response.GetResponseNormal.Result.Data.Value.ToString().StringToByte(), DisplayFormat);
                }
            });
            GetTimeZoneCommand = new RelayCommand(async () =>
            {
                var response = await Client.GetRequestAndWaitResponse(Clock.GetTimeZoneAttributeDescriptor());
                if (response != null)
                {
                    Clock.TimeZone = int.Parse(response.GetResponseNormal.Result.Data.ValueString);
                }
            });
        }
    }
}