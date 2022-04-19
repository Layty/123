using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using 三相智慧能源网关调试软件.MyControl;

namespace 三相智慧能源网关调试软件.ViewModels
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