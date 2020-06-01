using GalaSoft.MvvmLight;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class MainViewModel : ViewModelBase
    {

        public string Title
        {
            get => _Title;
            set { _Title = value; RaisePropertyChanged(); }
        }
        private string _Title;

        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                Title = Properties.Settings.Default.Title;
            }
            else
            {
                Title = Properties.Settings.Default.Title;
            }
           
        }
    }
}