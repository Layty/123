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
    public class ClearPowerProfileGenericBufferJob : ProfileGenericJobBaseNew
    {
        public ClearPowerProfileGenericBufferJob(NetLoggerViewModel netLoggerViewModel, MainServerViewModel mainServerViewModel,
           IProtocol protocol, DlmsSettingsViewModel dlmsSettingsViewModel) : base(netLoggerViewModel, protocol, dlmsSettingsViewModel)
        {
            JobName = "清空15功率冻结Buffer任务";
            CustomCosemProfileGenericModel = new CustomCosemProfileGenericModel(ProfileGenericLogicNameDefine.十五分钟电量曲线)
            {
                CaptureObjects = new ObservableCollection<CaptureObjectDefinition>()
                {
                    new CaptureObjectDefinition(){ ClassId=8,LogicalName="0.0.1.0.0.255",AttributeIndex=2,DataIndex=0,Description="Clock time"},
                    new CaptureObjectDefinition(){ ClassId=3,LogicalName="1.0.1.7.0.255",AttributeIndex=2,DataIndex=0,Description="总正向有功功率"},
                    new CaptureObjectDefinition(){ ClassId=3,LogicalName="1.0.2.7.0.255",AttributeIndex=2,DataIndex=0,Description="总反向有功功率"},
                    new CaptureObjectDefinition(){ ClassId=3,LogicalName="1.0.32.7.0.255",AttributeIndex=2,DataIndex=0,Description="L1 相电压"},
                    new CaptureObjectDefinition(){ ClassId=3,LogicalName="1.0.52.7.0.255",AttributeIndex=2,DataIndex=0,Description="L2 相电压"},
                    new CaptureObjectDefinition(){ ClassId=3,LogicalName="1.0.72.7.0.255",AttributeIndex=2,DataIndex=0,Description="L3 相电压"},
                    new CaptureObjectDefinition(){ ClassId=3,LogicalName="1.0.31.7.0.255",AttributeIndex=2,DataIndex=0,Description="L1 相电流"},
                    new CaptureObjectDefinition(){ ClassId=3,LogicalName="1.0.51.7.0.255",AttributeIndex=2,DataIndex=0,Description="L2 相电流"},
                    new CaptureObjectDefinition(){ ClassId=3,LogicalName="1.0.71.7.0.255",AttributeIndex=2,DataIndex=0,Description="L3 相电流"},
                }
            };
            MeterIdMatchSockets = mainServerViewModel.MeterIdMatchSockets;
        }
        private ObservableCollection<MeterIdMatchSocketNew> MeterIdMatchSockets;
        public override async Task Execute(IJobExecutionContext context)
        {
            if (MeterIdMatchSockets.Count == 0) return;
            NetLogViewModel.LogDebug("In ClearPowerBufferTask Execute");
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