using System;
using System.Windows;
using System.Windows.Controls;

namespace 三相智慧能源网关调试软件.MyControl.DLMSControl
{
    /// <summary>
    /// LoadIdentificationUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class LoadIdentificationUserControl : UserControl
    {
        public LoadIdentificationUserControl()
        {
            InitializeComponent();
            DatePicker.SelectedDate = DateTime.Now;

            TimePicker.SelectedTime = DateTime.Now;
        }
        private void DatePicker_OnSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DatePicker.SelectedDate != null)
                TextBlockShowTheDateTimeToBeSet.Text = DatePicker.SelectedDate.Value.ToString("yy-MM-dd ddd ");
            if (TimePicker.SelectedTime != null)
                TextBlockShowTheDateTimeToBeSet.Text += TimePicker.SelectedTime.Value.ToString("HH:mm:ss");
        }

        private void TimePicker_OnSelectedTimeChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            if (DatePicker.SelectedDate != null)
                TextBlockShowTheDateTimeToBeSet.Text = DatePicker.SelectedDate.Value.ToString("yy-MM-dd ddd ");
            if (TimePicker.SelectedTime != null)
                TextBlockShowTheDateTimeToBeSet.Text += TimePicker.SelectedTime.Value.ToString("HH:mm:ss");
        }

    }
}