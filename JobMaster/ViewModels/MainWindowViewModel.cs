using CommunityToolkit.Mvvm.ComponentModel;


namespace JobMaster.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _title = "智慧能源网关主站采集系统";



        public MainWindowViewModel()
        {
        }
    }
}