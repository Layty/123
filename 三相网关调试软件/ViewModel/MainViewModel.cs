using GalaSoft.MvvmLight;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                RaisePropertyChanged();
            }
        }

        private string _title;

        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                Title = "曾雄威";
            }
            else
            {
                Title = Properties.Settings.Default.Title;
            }
        }
    }
}