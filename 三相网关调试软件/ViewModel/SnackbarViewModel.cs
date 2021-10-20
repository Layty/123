using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace 三相智慧能源网关调试软件.ViewModel
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