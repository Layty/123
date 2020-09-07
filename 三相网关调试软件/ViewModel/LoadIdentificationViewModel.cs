using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using 三相智慧能源网关调试软件.DLMS;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.CosemObjects;
using 三相智慧能源网关调试软件.DLMS.ApplicationLay.Get;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class LoadIdentificationViewModel : ViewModelBase
    {
        public DLMSClient Client { get; set; }
        public DlMSLoadIdentification DlMSLoadIdentification { get; set; }
  
        public LoadIdentificationViewModel()
        {
            Client = CommonServiceLocator.ServiceLocator.Current.GetInstance<DLMSClient>();
            DlMSLoadIdentification=new DlMSLoadIdentification();
            GetEarliestCommand=new RelayCommand(async () =>
            {
                await Client.GetRequest(new GetRequest()
                    {GetRequestNormal = new GetRequestNormal(DlMSLoadIdentification.GetEarliestLoadIdentification())});
            });
            GetLatestCommand = new RelayCommand(async () =>
            {
                await Client.GetRequest(new GetRequest()
                    { GetRequestNormal = new GetRequestNormal(DlMSLoadIdentification.GetLatestLoadIdentification()) });
            });
            GetGivenTimeCommand = new RelayCommand<string>(async (t) =>
            {
                DateTime.TryParse(t, out var setDateTime);
                DLMSClock dt = new DLMSClock(setDateTime);
                await Client.GetRequest(new GetRequest()
                    { GetRequestNormal = new GetRequestNormal(DlMSLoadIdentification.GetLoadIdentificationWithTime(dt)) });
            });
        }


        public RelayCommand GetEarliestCommand
        {
            get => _GetEarliestCommand;
            set { _GetEarliestCommand = value; RaisePropertyChanged(); }
        }
        private RelayCommand _GetEarliestCommand;
        public RelayCommand GetLatestCommand
        {
            get => _GetLatestCommand;
            set { _GetLatestCommand = value; RaisePropertyChanged(); }
        }
        private RelayCommand _GetLatestCommand;
        public RelayCommand<string> GetGivenTimeCommand
        {
            get => _GetGivenTimeCommand;
            set { _GetGivenTimeCommand = value; RaisePropertyChanged(); }
        }
        private RelayCommand<string> _GetGivenTimeCommand;

    }
}