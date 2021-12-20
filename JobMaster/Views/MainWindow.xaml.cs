﻿using HandyControl.Controls;
using HandyControl.Themes;
using HandyControl.Tools;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Prism.Regions;
using JobMaster.ViewModels;

namespace JobMaster.Views
{
    public partial class MainWindow
    {
        public MainWindow(IRegionManager regionManager, MainServerViewModel mainServerViewModel)
        {
            InitializeComponent();  
            RegionManager = regionManager;
            MainServerViewModel = mainServerViewModel;
        }

        public IRegionManager RegionManager { get; }
        public MainServerViewModel MainServerViewModel { get; }

        #region Change Theme
        private void ButtonConfig_OnClick(object sender, RoutedEventArgs e) => PopupConfig.IsOpen = true;

        private void ButtonSkins_OnClick(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Button button)
            {
                PopupConfig.IsOpen = false;
                if (button.Tag is ApplicationTheme tag)
                {
                    ((App)Application.Current).UpdateTheme(tag);
                }
                else if (button.Tag is Brush accentTag)
                {
                    ((App)Application.Current).UpdateAccent(accentTag);
                }
                else if (button.Tag is "Picker")
                {
                    var picker = SingleOpenHelper.CreateControl<ColorPicker>();
                    var window = new PopupWindow
                    {
                        PopupElement = picker,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        AllowsTransparency = true,
                        WindowStyle = WindowStyle.None,
                        MinWidth = 0,
                        MinHeight = 0,
                        Title = "Select Accent Color"
                    };

                    picker.SelectedColorChanged += delegate
                    {
                        ((App)Application.Current).UpdateAccent(picker.SelectedBrush);
                        window.Close();
                    };
                    picker.Canceled += delegate { window.Close(); };
                    window.Show();
                }
            }
        }
        #endregion

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            RegionManager.RequestNavigate("ContentRegion", "JobCenterView");
        }

     

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MainServerViewModel.IsServerRunning)
            {
                await MainServerViewModel.CloseServerAsync();
            }

        }

        private void Job_Click(object sender, RoutedEventArgs e)
        {
            RegionManager.RequestNavigate("ContentRegion", "JobCenterView");
        }

        private void Noti_Click(object sender, RoutedEventArgs e)
        {
            RegionManager.RequestNavigate("ContentRegion", "DataNotificationView");
        }
    }
}