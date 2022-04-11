using Microsoft.Extensions.DependencyInjection;
using MyDlmsStandard;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using NLog;
using Quartz;
using System;
using System.Threading.Tasks;
using 三相智慧能源网关调试软件.ViewModel.DlmsViewModels;

namespace 三相智慧能源网关调试软件.Model.Jobs
{
    public class CloseWarmingJob : IDataJob
    {
        public CloseWarmingJob()
        {
            Client = App.Current.Services.GetService<DlmsClient>();
            JobName = "背光窃电状态字配置任务";
            DataModel = new CustomCosemDataModel("0.0.96.50.22.255");
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
                        var netLogViewModel = App.Current.Services.GetService<NetLogViewModel>();
                        netLogViewModel.MyServerNetLogModel.Log =
                            "正在执行" + JobName + "\r\n";
                        netLogViewModel.MyServerNetLogModel.Log =
                            "正在执行初始化请求";
                        await tempClient.InitRequest();
                        await Task.Delay(2000);
                        netLogViewModel.MyServerNetLogModel.Log =
                            "正在执行读取告警捕获对象" + DataModel.LogicalName;
                        await tempClient.GetRequestAndWaitResponse(DataModel.ValueAttributeDescriptor);
                        await Task.Delay(2000);
                        //TODO 这里可加入对返回结果的判断逻辑
                        {
                            netLogViewModel.MyServerNetLogModel.Log =
                                "正在执行清零操作";
                            await tempClient.SetRequestAndWaitResponse(DataModel.ValueAttributeDescriptor,
                                new DlmsDataItem(DataType.UInt32, "00000000"));
                            await Task.Delay(2000);
                        }
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
        public CustomCosemDataModel DataModel { get; set; }
    }
}