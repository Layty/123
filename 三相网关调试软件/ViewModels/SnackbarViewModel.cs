using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace 三相智慧能源网关调试软件.ViewModels
{
    public class SnackbarViewModel : ObservableObject
    {
        public RelayCommand ErrorMessageCommand { get; set; }

        public SnackbarViewModel()
        {
            ErrorMessageCommand = new RelayCommand(() =>
            {
                StrongReferenceMessenger.Default.Send("wo shi da shuai bi", "Snackbar");
            });
        }

    }
}