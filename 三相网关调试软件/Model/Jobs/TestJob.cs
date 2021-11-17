using CommonServiceLocator;
using MyDlmsStandard;
using NLog;
using Quartz;
using System;
using System.Threading.Tasks;
using 三相智慧能源网关调试软件.ViewModel.DlmsViewModels;
namespace 三相智慧能源网关调试软件.Model.Jobs
{
    public class TestJob : IBaseJob
    {
        public TestJob()
        {
            Client = ServiceLocator.Current.GetInstance<DlmsClient>();
            JobName = "简单测试任务";
        }
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                if (Client.Socket.SocketClientList.Count == 0)
                {
                    return;
                }
                foreach (var socket in Client.Socket.SocketClientList)
                {
                    var so = socket;
                    await Task.Run(async () =>
                    {
                        var tempClient = Client;
                        tempClient.CurrentSocket = so;
                        tempClient.DlmsSettingsViewModel.ProtocolInterfaceType = ProtocolInterfaceType.WRAPPER;
                        tempClient.DlmsSettingsViewModel.PhysicalChanelType = PhysicalChanelType.FrontEndProcess;
                        var netLogViewModel = ServiceLocator.Current.GetInstance<NetLogViewModel>();
                        netLogViewModel.MyServerNetLogModel.Log =
                            "正在执行" + JobName + "\r\n";
                        netLogViewModel.MyServerNetLogModel.Log =
                            "正在执行初始化请求";
                        await tempClient.InitRequest();

                        //                        await Task.Delay(2000);
                        //                        netLogViewModel.MyServerNetLogModel.Log =
                        //                            "正在执行读取曲线捕获对象";
                        //                        await tempClient.GetRequestAndWaitResponse(CustomCosemProfileGenericModel
                        //                            .GetCaptureObjectsAttributeDescriptor());
                        //                        await Task.Delay(2000);
                        //                        netLogViewModel.MyServerNetLogModel.Log =
                        //                            "正在执行读取曲线Buffer";
                        //                        await tempClient.GetRequestAndWaitResponseArray(CustomCosemProfileGenericModel
                        //                            .GetBufferAttributeDescriptorWithSelectionByRange());
                        //                        await Task.Delay(2000);
                        netLogViewModel.MyServerNetLogModel.Log =
                            "正在执行释放请求";
                        await tempClient.ReleaseRequest(true);
                    });
                }
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(JobName + e.Message);
            }
        }

        public DlmsClient Client { get; set; }
        public string JobName { get; set; }
        public int Period { get; set; }
    }
}