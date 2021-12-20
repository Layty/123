using JobMaster.Handlers;
using JobMaster.Models;
using JobMaster.Services;
using JobMaster.ViewModels;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.CosemObjects.ProfileGeneric;
using Quartz;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace JobMaster.Jobs
{
    public class ClearEnergyProfileGenericBufferJob : ProfileGenericJobBaseNew
    {
        public ClearEnergyProfileGenericBufferJob(NetLoggerViewModel netLoggerViewModel, MainServerViewModel mainServerViewModel,
           IProtocol protocol, DlmsSettingsViewModel dlmsSettingsViewModel) : base(netLoggerViewModel, protocol, dlmsSettingsViewModel)
        {
            JobName = "清空1分钟冻结Buffer任务";
            CustomCosemProfileGenericModel = new CustomCosemProfileGenericModel(ProfileGenericLogicNameDefine.一分钟电量曲线)
            {
                CaptureObjects = new ObservableCollection<CaptureObjectDefinition>()
                {
                    new CaptureObjectDefinition(){ ClassId=8,LogicalName="0.0.1.0.0.255",AttributeIndex=2,DataIndex=0,Description="Clock time"},
                    new CaptureObjectDefinition(){ ClassId=3,LogicalName="1.0.1.8.0.255",AttributeIndex=2,DataIndex=0,Description="正向有功总电能"},
                    new CaptureObjectDefinition(){ ClassId=3,LogicalName="1.0.1.8.1.255",AttributeIndex=2,DataIndex=0,Description="正向有功总电能尖"},
                    new CaptureObjectDefinition(){ ClassId=3,LogicalName="1.0.1.8.2.255",AttributeIndex=2,DataIndex=0,Description="正向有功总电能峰"},
                    new CaptureObjectDefinition(){ ClassId=3,LogicalName="1.0.1.8.3.255",AttributeIndex=2,DataIndex=0,Description="正向有功总电能平"},
                    new CaptureObjectDefinition(){ ClassId=3,LogicalName="1.0.1.8.4.255",AttributeIndex=2,DataIndex=0,Description="正向有功总电能谷"},
                    new CaptureObjectDefinition(){ ClassId=3,LogicalName="1.0.2.8.0.255",AttributeIndex=2,DataIndex=0,Description="反向有功总电能"},
                    new CaptureObjectDefinition(){ ClassId=3,LogicalName="1.0.3.8.0.255",AttributeIndex=2,DataIndex=0,Description="正向无功总电能"},
                    new CaptureObjectDefinition(){ ClassId=3,LogicalName="1.0.4.8.0.255",AttributeIndex=2,DataIndex=0,Description="反向无功总电能"},
                }
            };
            MeterIdMatchSockets = mainServerViewModel.MeterIdMatchSockets;
        }
        private ObservableCollection<MeterIdMatchSocketNew> MeterIdMatchSockets;
        public override async Task Execute(IJobExecutionContext context)
        {
            if (MeterIdMatchSockets.Count == 0) return;
            NetLogViewModel.LogDebug("In ClearEnergyBufferTask Execute");
            for (int i = 0; i < MeterIdMatchSockets.Count; i++)
            {
                var index = i; //处理闭包

                _ = Task.Run(async () =>
                {
                    var tmp = MeterIdMatchSockets[index];
                    try
                    {
                        await Task.Run(async () =>
                        {
                            ILinkLayer linkLayer = new NettyLinkLayer(tmp.MySocket, NetLogViewModel);
                            var Business = new NettyBusiness(Protocol, linkLayer);

                            NetLogViewModel.LogDebug("正在执行" + JobName);
                            NetLogViewModel.LogDebug("开始执行协商请求");
                            var strIp = tmp.MySocket.Channel.RemoteAddress.ToString();
                            try
                            {
                                await Business.AssociationRequestAsyncNetty();
                                await Task.Delay(2000);
                                if (!AssiactionResponseHandler.Successors[strIp])
                                {
                                    NetLogViewModel.LogWarn("协商请求失败");
                                    return;
                                }
                            }
                            catch (Exception e)
                            {
                                NetLogViewModel.LogError(e.Message);

                                return;
                            }

                            NetLogViewModel.LogDebug("正在执行设置曲线捕获对象");
                            DLMSArray array = new DLMSArray()
                            {
                                Items = CustomCosemProfileGenericModel.CaptureObjects.Select(captureObjectDefinition => captureObjectDefinition.ToDlmsDataItem())
                        .ToArray()
                            };
                            await Business.SetRequestAndWaitResponseNetty(CustomCosemProfileGenericModel.CaptureObjectsAttributeDescriptor,
                                   new DlmsDataItem(DataType.Array, array));
                            await Task.Delay(2000);




                            NetLogViewModel.LogDebug("正在执行释放请求");

                            await Business.ReleaseRequestAsyncNetty(true);
                            await Task.Delay(2000);

                            var Release = ReleaseResponseHandler.ReleaseSuccessors[strIp];
                            if (!Release)
                            {
                                NetLogViewModel.LogWarn("释放失败");
                                return;
                            }



                        });
                    }
                    catch (Exception e)
                    {
                        NetLogViewModel.LogError(e.Message);
                    }
                });
            }
        }
    }
}