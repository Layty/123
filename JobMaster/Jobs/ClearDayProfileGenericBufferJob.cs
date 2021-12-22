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
    /// <summary>
    /// 通过设置捕获对象的方式清空曲线Buffer
    /// </summary>
    public class ClearDayProfileGenericBufferJob : ProfileGenericJobBaseNew
    {
        public ClearDayProfileGenericBufferJob(NetLoggerViewModel netLoggerViewModel, MainServerViewModel mainServerViewModel,
           IProtocol protocol, DlmsSettingsViewModel dlmsSettingsViewModel) : base(netLoggerViewModel, protocol, dlmsSettingsViewModel)
        {
            JobName = "清空日电量曲线Buffer任务";
            netLoggerViewModel.LogFront($"任务名称:{JobName}\r\n");
            CustomCosemProfileGenericModel = new CustomCosemProfileGenericModel(ProfileGenericLogicNameDefine.日冻结电量曲线)
            {
                CaptureObjects = ProfileGenericDefalutCaptrueObject.Day,
           
            };
            MeterIdMatchSockets = mainServerViewModel.MeterIdMatchSockets;
        }
        private ObservableCollection<MeterIdMatchSocketNew> MeterIdMatchSockets;
        public override async Task Execute(IJobExecutionContext context)
        {
            if (MeterIdMatchSockets.Count == 0) return;
            NetLogViewModel.LogDebug("In ClearDayBufferTask Execute");
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
                            var setResult = SetResponseHandler.SetResponseBindingSocketNew[strIp];
                            if (setResult != DataAccessResult.Success)
                            {
                                NetLogViewModel.LogWarn("设置失败");
                                return;
                            }
                            NetLogViewModel.LogFront($"{strIp}成功");


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