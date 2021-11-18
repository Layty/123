using DotNetty.Transport.Channels;
using JobMaster.Helpers;
using JobMaster.ViewModels;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.CosemObjects;
using MyDlmsStandard.ApplicationLay.DataNotification;
using MyDlmsStandard.Axdr;
using MyDlmsStandard.Wrapper;
using System;
using System.Net.Sockets;

namespace JobMaster.Handlers
{
    /// <summary>
    /// 主动上报服务
    /// </summary>
    public class DataNotificationHandler : ChannelHandlerAdapter
    {
        private readonly NetLoggerViewModel _logger;
        private readonly MainServerViewModel mainServerViewModel;

        public DataNotificationHandler(NetLoggerViewModel logger, MainServerViewModel mainServerViewModel, DataNotificationViewModel dataNotificationViewModel)
        {
            _logger = logger;
            this.mainServerViewModel = mainServerViewModel;
            _dataNotificationViewModel = dataNotificationViewModel;
        }

        private DataNotificationViewModel _dataNotificationViewModel;

        public override void ChannelActive(IChannelHandlerContext context)
        {
            _logger.MyServerNetLogModel.Log = DateTime.Now.ToString() + "DataNotificationHandler ChannelActive" + "\r\n";

            base.ChannelActive(context);
        }
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            //主动上报后续再实现

            if (message is byte[] bytes)
            {
                _logger.MyServerNetLogModel.Log = "DataNotificationHandler is running";

                Handler_Notify(context, bytes);

            }

        }


        private void Handler_Notify(IChannelHandlerContext context, byte[] bytes)
        {
            try
            {
                var s = bytes.ByteToString();
                var netFrame = new WrapperFrame();
                if (!netFrame.PduStringInHexConstructor(ref s))
                {
                    context.FireChannelRead(bytes);
                }

                var s1 = netFrame.WrapperBody.DataBytes.ByteToString();
                var dataNotification = new DataNotification();
                if (dataNotification.PduStringInHexConstructor(ref s1))
                {

                    var DataNotificationModel = new DataNotificationModel()
                    {
                        DateTime = DateTime.Now.ToString("yy-MM-dd ddd HH:mm:ss"),
                        IpAddress = context.Channel.RemoteAddress.ToString(),
                        MeterId = mainServerViewModel.FindMeterIdFromMeterIdMatchSockets(context)
                    };

                    var cosemClock = new CosemClock();
                    cosemClock.DlmsClockParse(dataNotification.DateTime.Value.StringToByte());
                    DataNotificationModel.AlarmDateTime = cosemClock.ToDateTime().ToString();

                    if (dataNotification.NotificationBody.DataValue.DataType == DataType.Structure)
                    {
                        var dlmsStructure = (DlmsStructure)dataNotification.NotificationBody.DataValue.Value;
                        var stringStructure = dlmsStructure.ToPduStringInHex();

                        if (DataNotificationModel.CustomAlarm.PduStringInHexConstructor(ref stringStructure))
                        {
                            switch (DataNotificationModel.CustomAlarm.PushId.Value)
                            {
                                case "0004190900FF":
                                    //停电上报相关
                                    switch (DataNotificationModel.CustomAlarm.AlarmDescriptor2.Value)
                                    {
                                        case "02000000":
                                            DataNotificationModel.AlarmType = AlarmType.ByPass;
                                            break;
                                        case "00000001":
                                            DataNotificationModel.AlarmType = AlarmType.PowerOff;
                                            break;
                                        case "00000004":
                                            DataNotificationModel.AlarmType = AlarmType.PowerOn;
                                            break;
                                        default:
                                            DataNotificationModel.AlarmType = AlarmType.Unknown;
                                            break;
                                    }

                                    break;
                                case "0005190900FF":
                                    //水浸烟感上报相关
                                    DataNotificationModel.AlarmType = AlarmType.烟感and水浸;
                                    break;
                                case "0006190900FF":
                                    //风机控制上报相关
                                    DataNotificationModel.AlarmType = AlarmType.风机控制;
                                    break;
                                default:
                                    DataNotificationModel.AlarmType = AlarmType.Unknown;
                                    break;
                            }


                            DispatcherHelper.CheckBeginInvokeOnUI(() => { _dataNotificationViewModel.DataNotifications.Add(DataNotificationModel); });
                        }
                    }
                }
                else
                {
                    context.FireChannelRead(bytes);
                }

            }
            catch (Exception e)
            {

                _logger.MyServerNetLogModel.Log = e.Message + "\r\n";
            }
        }

    }
}