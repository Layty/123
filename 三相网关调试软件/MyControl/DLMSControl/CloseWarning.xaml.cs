using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.Axdr;
using 三相智慧能源网关调试软件.Model;
using 三相智慧能源网关调试软件.ViewModel.DlmsViewModels;
using DlmsDataItem = MyDlmsStandard.ApplicationLay.DlmsDataItem;

namespace 三相智慧能源网关调试软件.MyControl.DLMSControl
{
    /// <summary>
    /// CloseWarning.xaml 的交互逻辑
    /// </summary>
    public partial class CloseWarning : UserControl
    {
        public CloseWarning()
        {
            InitializeComponent();
            DataContext=new CloseWarningViewModel();
        }
        public class CloseWarningViewModel : ObservableObject
        {
            public RelayCommand InitCommand
            {
                get => _initCommand;
                set { _initCommand = value; }
            }

            private RelayCommand _initCommand;

            public RelayCommand ReleaseCommand
            {
                get => _ReleaseCommand;
                set { _ReleaseCommand = value; }
            }

            private RelayCommand _ReleaseCommand;


            public RelayCommand GetWarningConfigCommand
            {
                get => _GetWarningConfigCommand;
                set { _GetWarningConfigCommand = value; }
            }

            private RelayCommand _GetWarningConfigCommand;



            public RelayCommand SetWarningConfigCommand
            {
                get => _SetWarningConfigCommand;
                set { _SetWarningConfigCommand = value;  }
            }
            private RelayCommand _SetWarningConfigCommand;
                

            public DlmsClient Client { get; set; }


            public CloseWarningViewModel()
            {
                Client = SimpleIoc.Default.GetInstance<DlmsClient>();

                InitCommand = new RelayCommand(async () => { await Client.InitRequest(); });
                ReleaseCommand = new RelayCommand(async () => { await Client.ReleaseRequest(); });
                CustomCosemDataModel cosemData = new CustomCosemDataModel("0.0.96.50.22.255", ObjectType.Data,
                    new AxdrInteger8(
                        "02"));

                GetWarningConfigCommand = new RelayCommand(async () =>
                {
                    var t = cosemData.ValueAttributeDescriptor;
                    await Client.GetRequestAndWaitResponse(t);
                });

                SetWarningConfigCommand=new RelayCommand(async () =>
                {
                    await Client.SetRequestAndWaitResponse(cosemData.ValueAttributeDescriptor,
                        new DlmsDataItem(DataType.UInt64,"00000000" )
                        );
                });

            }
        }
    }

  
}