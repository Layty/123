using System;
using System.Collections.Generic;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using 三相智慧能源网关调试软件.Properties;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class SkinViewModel : ViewModelBase
    {
        public SkinViewModel()
        {
            if (IsInDesignMode)
            {
                //Swatches = new SwatchesProvider().Swatches;
                //IsDarkTheme = Properties.Settings.Default.IsDarkTheme;
                //ApplyBase(IsDarkTheme);
            }
            else
            {
                Swatches = new SwatchesProvider().Swatches;
                IsDarkTheme = Settings.Default.IsDarkTheme;
                ApplyBase(IsDarkTheme);
                ApplyBase();
            }
        }

        private bool _isDarkTheme;

        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                _isDarkTheme = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 样式集合
        /// </summary>
        public IEnumerable<Swatch> Swatches { get; }


        public void ApplyBase()
        {
            var theme = new PaletteHelper().GetTheme();
            theme.SetPrimaryColor((Color) ColorConverter.ConvertFromString(Settings.Default.PrimarySkin));
            theme.SetSecondaryColor((Color) ColorConverter.ConvertFromString(Settings.Default.AccentSkin));
            new PaletteHelper().SetTheme(theme);
        }

        public RelayCommand<bool> ToggleBaseCommand { get; } = new RelayCommand<bool>(ApplyBase);

        private static void ApplyBase(bool isDark)
        {
            ModifyTheme(theme => theme.SetBaseTheme(isDark ? Theme.Dark : Theme.Light));
            Settings.Default.IsDarkTheme = isDark;
            Settings.Default.Save();
        }

        public RelayCommand<Swatch> ApplyPrimaryCommand { get; } =
            new RelayCommand<Swatch>(ApplyPrimary);

        public RelayCommand<Swatch> ApplyAccentCommand { get; } =
            new RelayCommand<Swatch>(ApplyAccent);

        private static void ApplyPrimary(Swatch swatch)
        {
            ModifyTheme(theme => theme.SetPrimaryColor(swatch.ExemplarHue.Color));
            Settings.Default.PrimarySkin = swatch.ExemplarHue.Color.ToString();
            Settings.Default.Save();
        }

        private static void ApplyAccent(Swatch swatch)
        {
            ModifyTheme(theme => theme.SetSecondaryColor(swatch.AccentExemplarHue.Color));
            Settings.Default.AccentSkin = swatch.ExemplarHue.Color.ToString();
            Settings.Default.Save();
        }

        private static void ModifyTheme(Action<ITheme> modificationAction)
        {
            PaletteHelper paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();
            modificationAction?.Invoke(theme);
            paletteHelper.SetTheme(theme);
        }
    }
}