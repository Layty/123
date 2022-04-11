using MaterialDesignThemes.Wpf;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using 三相智慧能源网关调试软件.MyControl;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class DialogsViewModel : ObservableObject
    {
        public RelayCommand OpenSkinViewDialogCommand { get; set; }


        public DialogsViewModel()
        {
            OpenSkinViewDialogCommand = new RelayCommand(() =>
            {
                DialogHost.Show(new SkinView(), "Root");
            });

        }
    }
}