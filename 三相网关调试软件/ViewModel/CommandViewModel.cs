using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace 三相智慧能源网关调试软件.ViewModel
{
   public class CommandViewModel:ViewModelBase
    {
        public CommandViewModel()
        {
            if (IsInDesignMode)
            {
                
            }
            else
            {
                RestartCommand=new RelayCommand(Restart);
                TimingCommand=new RelayCommand(Timing);
            }
        }
        private RelayCommand _restartCommand;

        public RelayCommand RestartCommand
        {
            get { return _restartCommand; }
            set { _restartCommand = value; RaisePropertyChanged(); }
        }
        private RelayCommand _timingCommand;

        public RelayCommand TimingCommand
        {
            get { return _timingCommand; }
            set { _timingCommand = value; RaisePropertyChanged(); }
        }
        public void Restart()
        {
        }
        public void Timing()
        {
        }

    }
}
