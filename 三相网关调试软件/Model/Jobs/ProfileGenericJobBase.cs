using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.ComponentModel;
using MyDlmsStandard;
using MyDlmsStandard.ApplicationLay.Get;
using NLog;
using Quartz;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using 三相智慧能源网关调试软件.ViewModel.DlmsViewModels;

namespace 三相智慧能源网关调试软件.Model.Jobs
{
    /// <summary>
    /// 曲线采集的父类，定义实现了基本的采集规则
    /// </summary>
    public abstract class ProfileGenericJobBase : ObservableObject, IProfileGenericJob
    {
        protected ProfileGenericJobBase()
        {
            
            Client = App.Current.Services.GetService<DlmsClient>();
            NetLogViewModel = App.Current.Services.GetService<NetLogViewModel>();
        }
      
        public DlmsClient Client { get; set; }
        public NetLogViewModel NetLogViewModel { get; set; }
        public string JobName { get; set; }
        public int Period { get; set; }


        public CustomCosemProfileGenericModel CustomCosemProfileGenericModel
        {
            get => _customCosemProfileGenericModel;
            set
            {
                _customCosemProfileGenericModel = value;
                OnPropertyChanged();
            }
        }

        private CustomCosemProfileGenericModel _customCosemProfileGenericModel;


        public List<GetResponse> Responses { get; set; }
        public GetResponse CaptureObjects { get; set; }

        public Dictionary<Socket, GetResponse> CaptureObjectsResponsesBindingSocket { get; set; } =
            new Dictionary<Socket, GetResponse>();

        public Dictionary<Socket, List<GetResponse>> DataBufferResponsesBindingSocket { get; set; } =
            new Dictionary<Socket, List<GetResponse>>();

        public virtual async Task Execute(IJobExecutionContext context)
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
                        tempClient.TcpServerViewModel.CurrentSocketClient = so;
                        //初始化设置读取方式协议为47协议+以太网,后续放出去配置
                        tempClient.DlmsSettingsViewModel.ProtocolInterfaceType = ProtocolInterfaceType.WRAPPER;
                        tempClient.DlmsSettingsViewModel.PhysicalChanelType = PhysicalChanelType.FrontEndProcess;
                     
                        NetLogViewModel.MyServerNetLogModel.Log = "正在执行" + JobName + "\r\n";
                        NetLogViewModel.MyServerNetLogModel.Log = "正在执行初始化请求\r\n";
                        if (!await tempClient.InitRequest())
                        {
                            NetLogViewModel.MyServerNetLogModel.Log = "初始化请求失败，开始重试1\r\n";
                            await Task.Delay(2000);
                            if (!await tempClient.InitRequest())
                            {
                                NetLogViewModel.MyServerNetLogModel.Log = "初始化请求失败，开始重试2\r\n";
                                await Task.Delay(2000);
                                if (!await tempClient.InitRequest())
                                {
                                    return;
                                }
                            }
                        }

                        await Task.Delay(2000);
                        NetLogViewModel.MyServerNetLogModel.Log = "正在执行读取曲线捕获对象\r\n";
                        CaptureObjects = new GetResponse();
                        CaptureObjects =
                            await tempClient.GetRequestAndWaitResponse(CustomCosemProfileGenericModel
                                .CaptureObjectsAttributeDescriptor);
                        if (CaptureObjects.GetResponseNormal?.Result.Data.Value != null)
                        {
                            NetLogViewModel.MyServerNetLogModel.Log = "读取曲线捕获对象成功\r\n";
                            CaptureObjectsResponsesBindingSocket.Add(so, CaptureObjects);
                        }
                        else
                        {
                            NetLogViewModel.MyServerNetLogModel.Log = "读取曲线捕获对象失败\r\n";
                            return;
                        }

                        await Task.Delay(2000);
                        NetLogViewModel.MyServerNetLogModel.Log = "正在执行读取曲线Buffer\r\n";
                        Responses = new List<GetResponse>();
                        Responses = await tempClient.GetRequestAndWaitResponseArray(CustomCosemProfileGenericModel
                            .GetBufferAttributeDescriptorWithSelectionByRange);
                        if (Responses != null)
                        {
                            NetLogViewModel.MyServerNetLogModel.Log = "读取曲线Buffer成功\r\n";
                            DataBufferResponsesBindingSocket.Add(so, Responses);
                        }
                        else
                        {
                            NetLogViewModel.MyServerNetLogModel.Log = "读取曲线Buffer失败\r\n";
                            return;
                        }

                        await Task.Delay(2000);
                        NetLogViewModel.MyServerNetLogModel.Log = "正在执行释放请求\r\n";
                        await tempClient.ReleaseRequest(true);
                    });
                }
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(JobName + e.Message);
            }
        }
    }
}