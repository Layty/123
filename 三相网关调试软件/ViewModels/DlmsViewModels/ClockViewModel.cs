using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.CosemObjects;
using MyDlmsStandard.Common;
using 三相智慧能源网关调试软件.Helpers;

namespace 三相智慧能源网关调试软件.ViewModels.DlmsViewModels
{
    public class ClockViewModel : ObservableObject
    {
        public DlmsClient Client { get; set; }
        public CosemClock Clock { get; set; }
        private bool _isUsePcClock = true;

        public bool IsUsePcClock
        {
            get => _isUsePcClock;
            set => SetProperty(ref _isUsePcClock, value);
        }

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

        private DateTime _dateTimeForSet = DateTime.Now;

        public DateTime DateTimeForSet
        {
            get => _dateTimeForSet;
            set => SetProperty(ref _dateTimeForSet, value);
        }



        public RelayCommand SetTimeCommand { get; set; }

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

        public ClockViewModel(DlmsClient dlmsClient, ExcelHelper excel)
        {
            Client = dlmsClient;
            var dataTable = excel.GetExcelDataTable("Clock$");
            Clock = new CosemClock(dataTable.Rows[0][0].ToString());
            GetTimeCommand = new RelayCommand(async () =>
            {
                var response = await Client.GetRequestAndWaitResponse(Clock.GetTimeAttributeDescriptor());

                if (response != null)
                {
                    var DisplayFormat = OctetStringDisplayFormat.DateTime;
                    Clock.Time =
                        MyConvert.HowToDisplayOctetString(
                            Common.Common.StringToByte(response.GetResponseNormal.Result.Data.Value.ToString()), DisplayFormat);
                }
            });

            SetTimeCommand = new RelayCommand(async () =>
            {
                CosemClock cosemClock = IsUsePcClock ? new CosemClock(DateTime.Now) : new CosemClock(DateTimeForSet);
                DlmsDataItem dataItem = new DlmsDataItem(DataType.OctetString, cosemClock.GetDateTimeBytes().ByteToString());
                var response = await Client.SetRequestAndWaitResponse(Clock.GetTimeAttributeDescriptor(), dataItem);

                if (response != null)
                {
                    //var DisplayFormat = OctetStringDisplayFormat.DateTime;
                    //Clock.Time =
                    //    MyConvert.HowToDisplayOctetString(
                    //        Common.Common.StringToByte(response.GetResponseNormal.Result.Data.Value.ToString()), DisplayFormat);
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