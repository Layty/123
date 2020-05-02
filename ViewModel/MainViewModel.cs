using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using MaterialDesignThemes.Wpf;
using 三相智慧能源网关调试软件.Model;
using 三相智慧能源网关调试软件.MyControl;
using 三相智慧能源网关调试软件.View;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        
        public MainViewModel()
        {
            if (IsInDesignMode)
            {
            }

            else
            {
            }

            DispatcherHelper.Initialize();
        }
    }
}