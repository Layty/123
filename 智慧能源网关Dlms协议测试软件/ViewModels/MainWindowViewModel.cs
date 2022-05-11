using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Prism.Events;
using Prism.Regions;


namespace 智慧能源网关Dlms协议测试软件.ViewModels
{
    internal partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _title="测试软件";
        //private IRegionNavigationJournal journal;
        //[ICommand]
        //public void GoBack()
        //{
        //    if (journal != null && journal.CanGoBack)
        //        journal.GoBack();

        //}
        //[ICommand]
        //public void GoForward ()
        //{
        //    if (journal != null && journal.CanGoForward)
        //        journal.GoForward();
        //}
        
    }
}
