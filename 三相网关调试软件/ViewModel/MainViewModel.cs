using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        
        public MainViewModel()
        {
            if (IsInDesignMode)
            {
            }

            DispatcherHelper.Initialize();
        }
    }
}