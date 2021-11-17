using JobMaster.Models;
using JobMaster.ViewModels;
using MyDlmsStandard.ApplicationLay.Get;
using NLog;
using Quartz;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace JobMaster.Jobs
{
    /// <summary>
    /// 曲线采集的父类，定义实现了基本的采集规则
    /// </summary>
    public abstract class ProfileGenericJobBase : IProfileGenericJob
    {

        protected ProfileGenericJobBase(NetLoggerViewModel netLoggerViewModel, DlmsClient dlmsClient)
        {
            Client = dlmsClient;
            NetLogViewModel = netLoggerViewModel;
        }

        public DlmsClient Client { get; set; }
        public NetLoggerViewModel NetLogViewModel { get; set; }
        public string JobName { get; set; }
        public int Period { get; set; }
        public CustomCosemProfileGenericModel CustomCosemProfileGenericModel { get; set; }

        public Dictionary<Socket, GetResponse> CaptureObjectsResponsesBindingSocket { get; set; } =
            new Dictionary<Socket, GetResponse>();

        public Dictionary<Socket, List<GetResponse>> DataBufferResponsesBindingSocket { get; set; } =
            new Dictionary<Socket, List<GetResponse>>();

        public virtual async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await Task.Run(async () =>
                {
                    NetLogViewModel.MyServerNetLogModel.Log = "正在执行" + JobName + "\r\n";
                    NetLogViewModel.MyServerNetLogModel.Log = "开始执行初始化请求\r\n";
                    if (!await Client.InitRequest())
                    {
                        NetLogViewModel.MyServerNetLogModel.Log = "初始化请求失败，开始重试1\r\n";
                        await Task.Delay(2000);
                        if (!await Client.InitRequest())
                        {
                            NetLogViewModel.MyServerNetLogModel.Log = "初始化请求失败，开始重试2\r\n";
                            await Task.Delay(2000);
                            if (!await Client.InitRequest())
                            {
                                return;
                            }
                        }
                    }

                    await Task.Delay(2000);
                    NetLogViewModel.MyServerNetLogModel.Log = "正在执行读取曲线捕获对象\r\n";
                    var CaptureObjectsResponse = new GetResponse();
                    CaptureObjectsResponse =
                        await Client.GetRequestAndWaitResponse(CustomCosemProfileGenericModel
                            .CaptureObjectsAttributeDescriptor);
                    if (CaptureObjectsResponse?.GetResponseNormal?.Result.Data.Value != null)
                    {
                        NetLogViewModel.MyServerNetLogModel.Log = "读取曲线捕获对象成功\r\n";
                        CaptureObjectsResponsesBindingSocket.Add(Client.TcpServerViewModel.CurrentSocketClient, CaptureObjectsResponse);
                    }
                    else
                    {
                        NetLogViewModel.MyServerNetLogModel.Log = "读取曲线捕获对象失败\r\n";
                        return;
                    }

                    await Task.Delay(2000);
                    NetLogViewModel.MyServerNetLogModel.Log = "正在执行读取曲线Buffer\r\n";
                    var ResponsesBuffer = new List<GetResponse>();
                    ResponsesBuffer = await Client.GetRequestAndWaitResponseArray(CustomCosemProfileGenericModel
                        .GetBufferAttributeDescriptorWithSelectionByRange);
                    if (ResponsesBuffer != null)
                    {
                        NetLogViewModel.MyServerNetLogModel.Log = "读取曲线Buffer成功\r\n";
                        DataBufferResponsesBindingSocket.Add(Client.TcpServerViewModel.CurrentSocketClient, ResponsesBuffer);
                    }
                    else
                    {
                        NetLogViewModel.MyServerNetLogModel.Log = "读取曲线Buffer失败\r\n";
                        return;
                    }

                    await Task.Delay(2000);
                    NetLogViewModel.MyServerNetLogModel.Log = "正在执行释放请求\r\n";
                    await Client.ReleaseRequest(true);
                });
            }
            catch (Exception e)
            {
                NetLogViewModel.MyServerNetLogModel.Log = e.Message;
                LogManager.GetCurrentClassLogger().Error(JobName + e.Message);
            }
        }
    }
}