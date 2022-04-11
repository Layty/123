using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace 三相智慧能源网关调试软件.Model
{
    public class NewMenuModel : MenuModel
    {
        public string ItemName { get; set; }
        public List<NewMenuModel> SubMenuModel { get; set; }

    }
    public class MenuModel : ObservableObject
    {

        private string _assembly;

        public string Assembly
        {
            get => _assembly;
            set { _assembly = value; OnPropertyChanged(); }
        }


        private string _menuName;

        public string MenuName
        {
            get => _menuName;
            set
            {
                _menuName = value;
                OnPropertyChanged();
            }
        }

        private string _iconFont;

        public string IconFont
        {
            get => _iconFont;
            set
            {
                _iconFont = value;
                OnPropertyChanged();
            }
        }

        private string _fontSize;

        public string FontSize
        {
            get => _fontSize;
            set
            {
                _fontSize = value;
                OnPropertyChanged();
            }
        }

        private string _foreground;

        public string Foreground
        {
            get => _foreground;
            set
            {
                _foreground = value;
                OnPropertyChanged();
            }
        }


    }
}