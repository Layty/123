using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using 三相智慧能源网关调试软件.Commom;
using 三相智慧能源网关调试软件.DLMS;
using 三相智慧能源网关调试软件.DLMS._21EMode;
using 三相智慧能源网关调试软件.DLMS.CosemObjects;
using 三相智慧能源网关调试软件.ViewModel;
using DataType = 三相智慧能源网关调试软件.DLMS.ApplicationLayEnums.DataType;
using Task = System.Threading.Tasks.Task;

namespace 三相智慧能源网关调试软件.View.BaseMeter
{
    /// <summary>
    /// UpGradeBaseMeterPage.xaml 的交互逻辑
    /// </summary>
    public partial class UpGradeBaseMeterPage : Page
    {
        public SerialPortViewModel SerialPortViewModel = ServiceLocator.Current.GetInstance<SerialPortViewModel>();
        public DlmsViewModel DlmsViewModel = ServiceLocator.Current.GetInstance<DlmsViewModel>();

        public UpGradeBaseMeterPage()
        {
            InitializeComponent();
            Messenger.Default.Register<(int, int)>(this, "ReportProgressBar", ReportProgressBar);
        }

        private void ReportProgressBar((int, int) obj)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                ProgressBar.Value = obj.Item1;
                ProgressBar.Maximum = obj.Item2;
            });
        }

        private async void ButtonInit_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DlmsViewModel.IsUse21E)
                {
                    EModeExecutor eModeExecutor = new EModeExecutor(SerialPortViewModel.SerialPortModel, "");
                    if (await eModeExecutor.Execute())
                    {
                        var t = DlmsViewModel.DExecutor.ExecuteHdlcSNRMRequest();
                        await t.ContinueWith(
                            t1 =>
                            {
                                if (!t.Result)
                                {
                                    return null;
                                }

                                return DlmsViewModel.DExecutor.ExecuteHdlcComm(DlmsViewModel.HdlcFrameMaker
                                    .AarqRequest);
                            },
                            TaskContinuationOptions.OnlyOnRanToCompletion);
                    }
                }
                else
                {
                    var t = DlmsViewModel.DExecutor.ExecuteHdlcSNRMRequest();

                    //   Task t = DlmsViewModel.DExecutor.ExecuteHdlcSNRMRequest();
                    await t.ContinueWith(
                        t1 =>
                        {
                            if (!t.Result)
                            {
                                return null;
                            }

                            return DlmsViewModel.DExecutor.ExecuteHdlcComm(DlmsViewModel.HdlcFrameMaker.AarqRequest);
                        },
                        TaskContinuationOptions.OnlyOnRanToCompletion);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private async void ButtonDisConnect_OnClick(object sender, RoutedEventArgs e)
        {
            await DlmsViewModel.DExecutor.ExecuteHdlcDisConnectRequest();
        }

        private async void ButtonGetSoftVersion_OnClick(object sender, RoutedEventArgs e)
        {
            var cosem = new DLMSData("1.0.0.2.0.255");
            var value = await cosem.GetValue();
            if (value != null)
            {
                var data = NormalDataParse.GetDataContent(value, 3, out bool result);
                if (result)
                {
                    TextBoxSoftVersion.Text = data.ByteToString();
                }
                else
                {
                    TextBoxSoftVersion.Text = "";
                }
            }
        }


        private async void ButtonReadFactory_OnClick(object sender, RoutedEventArgs e)
        {
            var cosem = new DLMSData("0.0.96.5.0.255");
            var value = await cosem.GetValue();
            if (value != null)
            {
                var data = NormalDataParse.GetDataFactoryContent(value, 3, out bool result);
                if (result)
                {
                    TextBoxFactory.Text = BitConverter.ToUInt16(data.Reverse().ToArray(), 0).ToString();
                }
                else
                {
                    TextBoxFactory.Text = "";
                }
            }
        }

        private void ButtonEnterFactory_OnClick(object sender, RoutedEventArgs e)
        {
            var cosem = new DLMSData("0.0.96.5.0.255");
            byte[] inputBytes = BitConverter.GetBytes(short.Parse("8192")).Reverse().ToArray();
            DLMSDataItem dataItem = new DLMSDataItem(DataType.UInt16, inputBytes);
            cosem.SetValue(dataItem);
        }

        private void ButtonQuitFactory_OnClick(object sender, RoutedEventArgs e)
        {
            var cosem = new DLMSData("0.0.96.5.0.255");
            byte[] inputDate = BitConverter.GetBytes(short.Parse("0")).Reverse().ToArray();
            var dateItem = new DLMSDataItem(DataType.UInt16, inputDate);
            cosem.SetValue(dateItem);
        }

        private async void ButtonEnterUpgradeMode_OnClick(object sender, RoutedEventArgs e)
        {
            var msg = DlmsViewModel.HdlcFrameMaker.SetEnterUpGradeMode(256); //写256
            await SerialPortViewModel.SerialPortModel.SendAndReceiveReturnData(msg);
        }

        private void ButtonClearAllData_OnClick(object sender, RoutedEventArgs e)
        {
            var cosem = new ScriptTable();
            cosem.ScriptExecute(1);
        }

        private void ButtonSetCapturePeriod_OnClick(object sender, RoutedEventArgs e)
        {
            var cosem = new ProfileGeneric("1.0.99.1.0.255");
            cosem.SetCapturePeriod(60);
        }

        private void ButtonGetCaptureObjects_OnClick(object sender, RoutedEventArgs e)
        {
            var cosem = new ProfileGeneric("1.0.99.1.0.255");
            cosem.Reset();
//            cosem.GetCaptureObjects();
        }


        private async void ButtonOneKeyStart_OnClick(object sender, RoutedEventArgs e)
        {
            SerialPortViewModel.SerialPortModel.SendAndReceiveDataCollections = "开始搞事情" + Environment.NewLine;
            ButtonInit_OnClick(sender, e);
            await Task.Delay(500);
            SerialPortViewModel.SerialPortModel.SendAndReceiveDataCollections = "读工厂模式"+Environment.NewLine;
            ButtonReadFactory_OnClick(sender, e);
            await Task.Delay(500);
            SerialPortViewModel.SerialPortModel.SendAndReceiveDataCollections = "读软件版本" + Environment.NewLine;
            ButtonGetSoftVersion_OnClick(sender, e);
            await Task.Delay(500);
            SerialPortViewModel.SerialPortModel.SendAndReceiveDataCollections = "进入工厂模式" + Environment.NewLine;
            ButtonEnterFactory_OnClick(sender, e);
            await Task.Delay(500);
            SerialPortViewModel.SerialPortModel.SendAndReceiveDataCollections = "读工厂模式" + Environment.NewLine;
            ButtonReadFactory_OnClick(sender, e);
            await Task.Delay(500);
            SerialPortViewModel.SerialPortModel.SendAndReceiveDataCollections = "进入升级模式" + Environment.NewLine;
            ButtonEnterUpgradeMode_OnClick(sender, e);
            await Task.Delay(500);
            SerialPortViewModel.SerialPortModel.SendAndReceiveDataCollections = "请开始你的表演" + Environment.NewLine;
        }

        private async  void ButtonOneKeyLeave_OnClick(object sender, RoutedEventArgs e)
        {
            SerialPortViewModel.SerialPortModel.SendAndReceiveDataCollections = "开始收拾" + Environment.NewLine;
            ButtonInit_OnClick(sender, e);
            await Task.Delay(500);
            SerialPortViewModel.SerialPortModel.SendAndReceiveDataCollections = "读软件版本" + Environment.NewLine;
            ButtonGetSoftVersion_OnClick(sender, e);
            await Task.Delay(500);
            SerialPortViewModel.SerialPortModel.SendAndReceiveDataCollections = "设置捕获时间60s" + Environment.NewLine;
            ButtonSetCapturePeriod_OnClick(sender, e);
            await Task.Delay(500);
            SerialPortViewModel.SerialPortModel.SendAndReceiveDataCollections = "退出工厂模式" + Environment.NewLine;
            ButtonQuitFactory_OnClick(sender, e);
            await Task.Delay(500);
            SerialPortViewModel.SerialPortModel.SendAndReceiveDataCollections = "读工厂模式" + Environment.NewLine;
            ButtonReadFactory_OnClick(sender, e);
            await Task.Delay(500);
            SerialPortViewModel.SerialPortModel.SendAndReceiveDataCollections = "事了拂袖去" + Environment.NewLine;
            ButtonDisConnect_OnClick(sender,e);
            await Task.Delay(500);
            SerialPortViewModel.SerialPortModel.SendAndReceiveDataCollections = "深藏功与名" + Environment.NewLine;
        }
    }
}