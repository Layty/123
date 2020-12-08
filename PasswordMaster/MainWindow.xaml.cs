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
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.Action;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.Association;
using MyDlmsStandard.Axdr;
using MyDlmsStandard.Common;
using MyDlmsStandard.HDLC;

namespace PasswordMaster
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPortViewModel serialPortViewModel = new SerialPortViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = serialPortViewModel;
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            HdlcFrameMaker frameMaker = new HdlcFrameMaker(1, 1, new DLMSInfo());
            await serialPortViewModel.SerialPortMaster.SendAndReceiveReturnDataAsync(frameMaker.SNRMRequest());
            await Task.Delay(600);
            AssociationRequest associationRequest = new AssociationRequest(
                Encoding.Default.GetBytes(TextBoxCurrentPassword.Text), 65535, 6, "", (Conformance) 0x7E1F);
            await serialPortViewModel.SerialPortMaster.SendAndReceiveReturnDataAsync(
                frameMaker.InvokeApdu(associationRequest.ToPduBytes()));
            await Task.Delay(600);
            CosemMethodDescriptor descriptor = new CosemMethodDescriptor(
                new AxdrIntegerUnsigned16("0F"), new AxdrOctetStringFixed(MyConvert.ObisToHexCode("0.0.40.0.5.255"), 6),
                new AxdrInteger8("02"));

            ActionRequest actionRequest = new ActionRequest()
            {
                ActionRequestNormal =
                    new ActionRequestNormal(descriptor,
                        new DlmsDataItem(DataType.OctetString,
                            Encoding.Default.GetBytes(TextBoxNextPassword.Text).ByteToString()))
            };
            var t = actionRequest.ToPduStringInHex();
            await serialPortViewModel.SerialPortMaster.SendAndReceiveReturnDataAsync(frameMaker.InvokeApdu(
                t.StringToByte()
            ));
            await Task.Delay(600);
        }
    }
}