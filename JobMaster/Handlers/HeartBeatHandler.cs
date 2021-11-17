using DotNetty.Buffers;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using JobMaster.Helpers;
using JobMaster.ViewModels;
using MyDlmsStandard.Wrapper;
using System;
using System.Threading.Tasks;

namespace JobMaster.Handlers
{
    //DLMS心跳服务
    public class HeartBeatHandler : ChannelHandlerAdapter
    {
        private readonly NetLoggerViewModel _logger;
        private MainServerViewModel _mainServerViewModel;
        public HeartBeatHandler(NetLoggerViewModel logger, MainServerViewModel mainServerViewModel)
        {
            _logger = logger;
            _mainServerViewModel = mainServerViewModel;
        }
        public override void ChannelActive(IChannelHandlerContext context)
        {
            _logger.MyServerNetLogModel.Log = DateTime.Now.ToString() + "HeartBeatHandler ChannelActive" + "\r\n";
            _mainServerViewModel.AddClient(context);
            base.ChannelActive(context);
        }
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            if (message is byte[] bytes)
            {
                _logger.MyServerNetLogModel.Log = "HeartBeatHandler is running";
                HandleHeartBeat(context, bytes);
            }

        }
        public void HandleHeartBeat(IChannelHandlerContext context, byte[] bytes)
        {
            // 处理心跳
            var heartBeatFrame = Wrapper47FrameFactory.CreateHeartBeatFrame(bytes);

            if (heartBeatFrame != null)
            {
                if (_mainServerViewModel.IsAutoResponseHeartBeat == false)
                {
                    return;
                }
                _logger.MyServerNetLogModel.Log = "符合心跳帧";
                heartBeatFrame.WrapperHeader.OverturnDestinationSource();
                Task.Delay(_mainServerViewModel.HeartBeatDelayTime).Wait(); ;
                var strAdd = heartBeatFrame.GetMeterAddressString();
                _logger.MyServerNetLogModel.Log = $"MeterAddress:{strAdd}";
                var t = Unpooled.Buffer();
                t.WriteBytes(heartBeatFrame.ToPduStringInHex().StringToByte());
                context.WriteAsync(t);
                _logger.MyServerNetLogModel.Log = $"Send Data To client:{context.Channel.RemoteAddress} " + heartBeatFrame.ToPduStringInHex().StringToByte().ByteToString(" ") + "\r\n";


                if (_mainServerViewModel.MeterIdMatchSockets.Count == 0)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        _mainServerViewModel.MeterIdMatchSockets.Add(new MeterIdMatchSocketNew()
                        {
                            MySocket = context,
                            IpString = context.Channel.RemoteAddress.ToString(),
                            MeterId = strAdd,
                            IsCheck = false
                        });
                    });
                }
                else
                {
                    var boo = false;
                    foreach (var myClass in _mainServerViewModel.MeterIdMatchSockets)
                    {
                        if (myClass.MeterId.Contains(strAdd))
                        {
                            boo = false;
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                //socket链接可能会变，但表号唯一,此处进行更新最新的Socket链接
                                myClass.IpString = context.Channel.RemoteAddress.ToString();
                                myClass.MySocket = context;
                            });
                            break;
                        }
                        else
                        {
                            boo = true;
                        }
                    }

                    if (boo)
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            _mainServerViewModel.MeterIdMatchSockets.Add(new MeterIdMatchSocketNew()
                            {
                                MySocket = context,
                                IpString = context.Channel.RemoteAddress.ToString(),
                                MeterId = strAdd,
                                IsCheck = false
                            });
                        });
                    }
                }
            }
            else { context.FireChannelRead(bytes); }

        }
        public override async void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            await Task.Run(() =>
            {
                _logger.MyServerNetLogModel.Log = "Exception: " + exception + "\r\n";
                _mainServerViewModel.RemoveClient(context);

                _logger.MyServerNetLogModel.Log = "断开: " + context.Channel.RemoteAddress + "\r\n";
                context.CloseAsync();
            });
        }

        public override void ChannelUnregistered(IChannelHandlerContext context)
        {
            _mainServerViewModel.RemoveClient(context);

            _logger.MyServerNetLogModel.Log = "已断开: " + context.Channel.RemoteAddress + "\r\n";
            base.ChannelUnregistered(context);
        }
        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            if (!(evt is IdleStateEvent))
            {
                return;
            }

            IdleStateEvent e = evt as IdleStateEvent;
            if (e.State == IdleState.ReaderIdle)
            {
                // The connection was OK but there was no traffic for last period.
                //Console.WriteLine("Disconnecting due to no inbound traffic");
                _logger.MyServerNetLogModel.Log = "长时间未与服务器通讯，踢掉";
                context.CloseAsync();
            }

        }
    }
}