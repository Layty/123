using JobMaster.ViewModels;
using MyDlmsStandard;
using NLog;
using Quartz;
using System;
using System.Threading.Tasks;

namespace JobMaster.Jobs
{
    public class TestJob : IBaseJob
    {
        public TestJob(NetLoggerViewModel netLoggerViewModel, DlmsClient dlmsClient)
        {
            NetLoggerViewModel = netLoggerViewModel;
            Client = dlmsClient;
            JobName = "简单测试任务";
        }
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                if (Client.TcpServerViewModel.TcpServerHelper.SocketClientList.Count == 0)
                {
                    return;
                }
                foreach (var socket in Client.TcpServerViewModel.TcpServerHelper.SocketClientList)
                {
                    var so = socket;
                    await Task.Run(async () =>
                    {
                        var tempClient = Client;                  
                        tempClient.DlmsSettingsViewModel.ProtocolInterfaceType = ProtocolInterfaceType.WRAPPER;
                        tempClient.DlmsSettingsViewModel.PhysicalChanelType = PhysicalChanelType.FrontEndProcess;

                        NetLoggerViewModel.MyServerNetLogModel.Log =
                            "正在执行" + JobName + "\r\n";
                        NetLoggerViewModel.MyServerNetLogModel.Log =
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
                        NetLoggerViewModel.MyServerNetLogModel.Log =
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

        public NetLoggerViewModel NetLoggerViewModel { get; }
        public DlmsClient Client { get; set; }
        public string JobName { get; set; }
        public int Period { get; set; }
    }
}