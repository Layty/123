using DotNetty.Transport.Channels;
using JobMaster.Helpers;
using JobMaster.ViewModels;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.CosemObjects;
using MyDlmsStandard.ApplicationLay.DataNotification;
using MyDlmsStandard.Wrapper;
using Newtonsoft.Json;
using RestSharp;
using System;


namespace JobMaster.Handlers
{
    /// <summary>
    /// 主动上报服务
    /// </summary>
    public partial class DataNotificationHandler : ChannelHandlerAdapter
    {
        private readonly NetLoggerViewModel _logger;
        private readonly MainServerViewModel mainServerViewModel;

        public DataNotificationHandler(NetLoggerViewModel logger, MainServerViewModel mainServerViewModel,
            DataNotificationViewModel dataNotificationViewModel)
        {
            _logger = logger;
            this.mainServerViewModel = mainServerViewModel;
            _dataNotificationViewModel = dataNotificationViewModel;
        }

        private DataNotificationViewModel _dataNotificationViewModel;

        public string BaseUriString = $"http://localhost:5000/api/Meter/NotificationData/";
        public RestClient RestClient = new RestClient();
        public RestRequest RestRequest = new RestRequest(Method.POST);

        public override void ChannelActive(IChannelHandlerContext context)
        {
            _logger.LogDebug(DateTime.Now.ToString() + "DataNotificationHandler ChannelActive" );
            base.ChannelActive(context);
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            //主动上报后续再实现

            if (message is byte[] bytes)
            {
                _logger.LogDebug(DateTime.Now.ToString()+"DataNotificationHandler is running");

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
                                    DataNotificationModel.AlarmType = AlarmType.烟感and水浸变位;
                                    break;
                                case "0006190900FF":
                                    //风机控制上报相关
                                    DataNotificationModel.AlarmType = AlarmType.风机控制;
                                    break;
                                default:
                                    DataNotificationModel.AlarmType = AlarmType.Unknown;
                                    break;
                            }


                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                _dataNotificationViewModel.DataNotifications.Add(DataNotificationModel);

                             
                            });
                            Notification notification = new Notification
                            {
                                Id = Guid.NewGuid(),
                                MeterId = DataNotificationModel.MeterId,
                                NotifyData = JsonConvert.SerializeObject(DataNotificationModel),
                                DateTime = cosemClock.ToDateTime()
                            };
                            InsertData(DataNotificationModel.MeterId, notification);
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
                _logger.LogError(e.Message);
            }
        }

        public void InsertData(string meterId, Notification notification)
        {

            if (notification == null)
            {
                _logger.LogWarn($"{meterId} {nameof(notification)} 主动上报为空,不调用API写数据库");

                return;
            }

            RestClient.BaseUrl = new Uri(BaseUriString + meterId);
            var RestRequest = new RestRequest(Method.POST);
            RestRequest.AddHeader("Content-Type", "application/json");
            var str = JsonConvert.SerializeObject(notification, Formatting.Indented);
            _logger.LogInfo(str);
            RestRequest.AddParameter("CurrentDataNotification", str, ParameterType.RequestBody);
            IRestResponse restResponse = RestClient.Execute(RestRequest);
            _logger.LogInfo("插入数据库" + (restResponse.IsSuccessful ? "成功" : "失败"));
        }
    }
}