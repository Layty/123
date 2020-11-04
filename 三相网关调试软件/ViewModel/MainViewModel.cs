using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private string _title;

        public MainViewModel()
        {
            Title = Properties.Settings.Default.Title;
        }
    }
}