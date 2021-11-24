using Prism.Mvvm;

namespace JobMaster.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "智慧能源网关主站采集系统";

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel()
        {
        }
    }
}