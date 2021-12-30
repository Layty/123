using HandyControl.Controls;
using HandyControl.Themes;
using Microsoft.Extensions.Logging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Prism.Regions;
using JobMaster.ViewModels;
using HandyControl.Tools;

namespace JobMaster.Views
{
    public partial class MainWindow
    {
        public MainWindow(IRegionManager regionManager, ILogger logger, MainServerViewModel mainServerViewModel, JobCenterViewModel jobCenterViewModel)
        {
            InitializeComponent();
            RegionManager = regionManager;
            Logger = logger;
            MainServerViewModel = mainServerViewModel;
            JobCenterViewModel = jobCenterViewModel;
        }

        public IRegionManager RegionManager { get; }
        public ILogger Logger { get; }
        public MainServerViewModel MainServerViewModel { get; }
        public JobCenterViewModel JobCenterViewModel { get; }

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
            if (JobCenterViewModel.IsSchedulerStarted)
            {
                JobCenterViewModel.Shutdown();
            }
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

        private void VirtualMeter_Click(object sender, RoutedEventArgs e)
        {
            
            RegionManager.RequestNavigate("ContentRegion", "VirtualMeterClientView");
        }
    }
}
