#define 张诗华
#undef 张诗华
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Controls;
using 三相智慧能源网关调试软件.Model;

namespace 三相智慧能源网关调试软件.ViewModel
{
    public class MenuViewModel : ObservableObject
    {
        public MenuViewModel()
        {

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
                        MenuName = "Telnet", FontSize = "20", IconFont = "\xe6ee", Assembly = "ServerCenter.TcpClientPage",
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
                        Assembly = "ServerCenter.DlmsSettingsPage",
                        Foreground = "#FF0000"
                    },
                    new MenuModel
                    {
                        MenuName = "DLMSClient", FontSize = "20", IconFont = "\xe63a",
                        Assembly = "ServerCenter.DlmsClientPage",
                        Foreground = "#FFFF00"
                    },
                    new MenuModel
                    {
                        MenuName = "TcpClient", FontSize = "20", IconFont = "\xe6ee",
                        Assembly = "ServerCenter.TcpClientPage",
                        Foreground = "#FF0000"
                    },
                    new MenuModel
                    {
                        MenuName = "TFTPMaster", FontSize = "20", IconFont = "\xe619",
                        Assembly = "ServerCenter.TftpMasterPage",
                        Foreground = "#0000FF"
                    },
                    new MenuModel()
                    {
                        MenuName = "IICDataAnalysis", FontSize = "20", IconFont = "\xe6ab",
                        Assembly = "ServerCenter.IicDataPage",
                        Foreground = "#6666FF"
                    },
                };

                ToolsMenuCollection = new ObservableCollection<MenuModel>()
                {
                    new MenuModel
                    {
                        MenuName = "DLMSSettings", FontSize = "20", IconFont = "\xe606",
                        Assembly = "ServerCenter.DLMSSettingsPage",
                        Foreground = "#FF0000"
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
                OnPropertyChanged();
            }
        }


        private ObservableCollection<MenuModel> _baseMeterMenuCollection;

        public ObservableCollection<MenuModel> BaseMeterMenuCollection
        {
            get => _baseMeterMenuCollection;
            set
            {
                _baseMeterMenuCollection = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<MenuModel> _servicesMenuCollection;

        public ObservableCollection<MenuModel> ServicesMenuCollection
        {
            get => _servicesMenuCollection;
            set
            {
                _servicesMenuCollection = value;
                OnPropertyChanged();
            }
        }


        public ObservableCollection<MenuModel> ToolsMenuCollection
        {
            get => _toolsMenuCollection;
            set { _toolsMenuCollection = value; OnPropertyChanged(); }
        }
        private ObservableCollection<MenuModel> _toolsMenuCollection;


        private MenuModel _menuModel;

        public MenuModel MenuModel
        {
            get => _menuModel;
            set
            {
                _menuModel = value;
                OnPropertyChanged();
            }
        }

        private Page _currentPage;

        public Page CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }
        public RelayCommand<MenuModel> SelectCommand { get; set; }


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