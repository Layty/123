using GalaSoft.MvvmLight;

namespace 三相智慧能源网关调试软件.Model
{
    public class MenuModel : ObservableObject
    {
        private string _assembly;

        public string Assembly
        {
            get => _assembly;
            set { _assembly = value; RaisePropertyChanged(); }
        }


        private string _menuName;

        public string MenuName
        {
            get => _menuName;
            set
            {
                _menuName = value;
                RaisePropertyChanged();
            }
        }

        private string _iconFont;

        public string IconFont
        {
            get => _iconFont;
            set
            {
                _iconFont = value;
                RaisePropertyChanged();
            }
        }

        private string _fontSize;

        public string FontSize
        {
            get => _fontSize;
            set
            {
                _fontSize = value;
                RaisePropertyChanged();
            }
        }

        private string _foreground;

        public string Foreground
        {
            get => _foreground;
            set
            {
                _foreground = value;
                RaisePropertyChanged();
            }
        }

       
    }
}