using CommunityToolkit.Mvvm.ComponentModel;

namespace 三相智慧能源网关调试软件.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _title;

        public MainViewModel()
        {
            Title = Properties.Settings.Default.Title;
        }
    }
}