#define 张诗华
#undef 张诗华
using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using 三相智慧能源网关调试软件.Model;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class MenuViewModel : ViewModelBase
    {
        public MenuViewModel()
        {
            if (IsInDesignMode)
            {
                ManagementMenuCollection = new ObservableCollection<MenuModel>
                {
                    new MenuModel {MenuName = "命令", FontSize = "20", IconFont = "\xe7b7"},
                    new MenuModel {MenuName = "系统日志", FontSize = "20", IconFont = "\xe668"},
                    new MenuModel {MenuName = "实时数据", FontSize = "20", IconFont = "\xe6ab"},
                    new MenuModel {MenuName = "全局参数", FontSize = "20", IconFont = "\xe606"},
                };

                BaseMeterMenuCollection = new ObservableCollection<MenuModel>
                {
                    new MenuModel
                    {
                        MenuName = "基表升级", FontSize = "20", IconFont = "\xe600", Assembly = "UpGradeBaseMeterPage",
                        Foreground = "#00FF00"
                    },

                    new MenuModel
                    {
                        MenuName = "Telnet", FontSize = "20", IconFont = "\xe600", Assembly = "TelnetPage",
                        Foreground = "#00FF00"
                    }
                };
                SelectCommand = new RelayCommand<MenuModel>(Select);
            }

            else
            {
#if 张诗华
                BaseMeterMenuCollection = new ObservableCollection<MenuModel>()
                {
                    new MenuModel()
                    {
                        MenuName = "基表串口", FontSize = "20", IconFont = "\xe66c", Assembly = "BaseMeter.SerialPortPage",
                        Foreground = "#FF0000"
                    },

                    new MenuModel()
                    {
                        MenuName = "基表升级", FontSize = "20", IconFont = "\xe600",
                        Assembly = "BaseMeter.UpGradeBaseMeterPage",
                        Foreground = "#00FF00"
                    },
                };

                ServicesMenuCollection = new ObservableCollection<MenuModel>()
                {
                    new MenuModel()
                    {
                        MenuName = "Telnet", FontSize = "20", IconFont = "\xe6ee", Assembly = "ServerCenter.TelnetPage",
                        Foreground = "#FF0000"
                    },
                };
#else
                ManagementMenuCollection = new ObservableCollection<MenuModel>
                {
                    new MenuModel
                    {
                        MenuName = "ENetMessageBuilder", FontSize = "20", IconFont = "\xe6ee",
                        Assembly = "Management.ENetDataPage",
                        Foreground = "#00FF00"
                    },
                };


                BaseMeterMenuCollection = new ObservableCollection<MenuModel>
                {
                    new MenuModel
                    {
                        MenuName = "基表升级", FontSize = "20", IconFont = "\xe600",
                        Assembly = "BaseMeter.UpGradeBaseMeterPage",
                        Foreground = "#00FF00"
                    }
                };

                ServicesMenuCollection = new ObservableCollection<MenuModel>
                {
                    new MenuModel
                    {
                        MenuName = "DLMSSettings", FontSize = "20", IconFont = "\xe606",
                        Assembly = "ServerCenter.DLMSSettingsPage",
                        Foreground = "#FF0000"
                    },
                    new MenuModel
                    {
                        MenuName = "DLMSClient", FontSize = "20", IconFont = "\xe6ee",
                        Assembly = "ServerCenter.DLMSClientPage",
                        Foreground = "#FF0000"
                    },
                    new MenuModel
                    {
                        MenuName = "Telnet", FontSize = "20", IconFont = "\xe6ee", Assembly = "ServerCenter.TelnetPage",
                        Foreground = "#FF0000"
                    },
                    new MenuModel
                    {
                        MenuName = "TFTPServer", FontSize = "20", IconFont = "\xe619",
                        Assembly = "ServerCenter.TftpServerPage",
                        Foreground = "#0000FF"
                    },
                    new MenuModel()
                    {
                        MenuName = "IICDataAnalysis", FontSize = "20", IconFont = "\xe6ab",
                        Assembly = "ServerCenter.IicDataPage",
                        Foreground = "#6666FF"
                    },
                };
#endif
                SelectCommand = new RelayCommand<MenuModel>(Select);
            }
        }

        private ObservableCollection<MenuModel> _managementMenuCollection;

        public ObservableCollection<MenuModel> ManagementMenuCollection
        {
            get => _managementMenuCollection;
            set
            {
                _managementMenuCollection = value;
                RaisePropertyChanged();
            }
        }


        private ObservableCollection<MenuModel> _baseMeterMenuCollection;

        public ObservableCollection<MenuModel> BaseMeterMenuCollection
        {
            get => _baseMeterMenuCollection;
            set
            {
                _baseMeterMenuCollection = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<MenuModel> _servicesMenuCollection;

        public ObservableCollection<MenuModel> ServicesMenuCollection
        {
            get => _servicesMenuCollection;
            set
            {
                _servicesMenuCollection = value;
                RaisePropertyChanged();
            }
        }


        private MenuModel _menuModel;

        public MenuModel MenuModel
        {
            get => _menuModel;
            set
            {
                _menuModel = value;
                RaisePropertyChanged();
            }
        }

        private Page _currentPage;

        public Page CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand<MenuModel> _selectCommand;

        public RelayCommand<MenuModel> SelectCommand
        {
            get => _selectCommand;
            set
            {
                _selectCommand = value;
                RaisePropertyChanged();
            }
        }


        private void Select(MenuModel menuModel)
        {
            MenuModel = menuModel;
            Type type = GetType();
            Assembly assembly = type.Assembly;
            CurrentPage = assembly.CreateInstance("三相智慧能源网关调试软件.View" + "." + MenuModel.Assembly) as Page;
            Activator.CreateInstance(type);
        }
    }
}