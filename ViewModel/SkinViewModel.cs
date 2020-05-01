using System;
using System.Collections.Generic;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class SkinViewModel : ViewModelBase
    {
        public SkinViewModel()
        {
            if (IsInDesignMode)
            {
                Swatches = new SwatchesProvider().Swatches;
                //IsDarkTheme = Properties.Settings.Default.IsDarkTheme;
                //ApplyBase(IsDarkTheme);
            }
            else
            {
                Swatches = new SwatchesProvider().Swatches;
                IsDarkTheme = Properties.Settings.Default.IsDarkTheme;
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
            theme.SetPrimaryColor((Color) ColorConverter.ConvertFromString(Properties.Settings.Default.PrimarySkin));
            theme.SetSecondaryColor((Color) ColorConverter.ConvertFromString(Properties.Settings.Default.AccentSkin));
            new PaletteHelper().SetTheme(theme);
        }

        public RelayCommand<bool> ToggleBaseCommand { get; } = new RelayCommand<bool>(o => ApplyBase((bool) o));

        private static void ApplyBase(bool isDark)
        {
            ModifyTheme(theme => theme.SetBaseTheme(isDark ? Theme.Dark : Theme.Light));
            Properties.Settings.Default.IsDarkTheme = isDark;
            Properties.Settings.Default.Save();
        }

        public RelayCommand<Swatch> ApplyPrimaryCommand { get; } =
            new RelayCommand<Swatch>(o => ApplyPrimary((Swatch) o));

        public RelayCommand<Swatch> ApplyAccentCommand { get; } =
            new RelayCommand<Swatch>(o => ApplyAccent((Swatch) o));

        private static void ApplyPrimary(Swatch swatch)
        {
            ModifyTheme(theme => theme.SetPrimaryColor(swatch.ExemplarHue.Color));
            Properties.Settings.Default.PrimarySkin = swatch.ExemplarHue.Color.ToString();
            Properties.Settings.Default.Save();
        }

        private static void ApplyAccent(Swatch swatch)
        {
            ModifyTheme(theme => theme.SetSecondaryColor(swatch.AccentExemplarHue.Color));
            Properties.Settings.Default.AccentSkin = swatch.ExemplarHue.Color.ToString();
            Properties.Settings.Default.Save();
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