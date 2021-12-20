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
        private readonly MainServerViewModel _mainServerViewModel;
        public HeartBeatHandler(NetLoggerViewModel logger, MainServerViewModel mainServerViewModel)
        {
            _logger = logger;
            _mainServerViewModel = mainServerViewModel;
        }
        public override void ChannelActive(IChannelHandlerContext context)
        {
            _logger.LogTrace(DateTime.Now.ToString() + "HeartBeatHandler ChannelActive" + "\r\n");
            _mainServerViewModel.AddClient(context);
            base.ChannelActive(context);
        }
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            if (message is byte[] bytes)
            {
                _logger.LogTrace("HeartBeatHandler is running");
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
                _logger.LogTrace("符合心跳帧");
                heartBeatFrame.WrapperHeader.OverturnDestinationSource();
                
               // Task.Delay(_mainServerViewModel.HeartBeatDelayTime).Wait(); 
               
                var strAdd = heartBeatFrame.GetMeterAddressString();
                _logger.LogFront($"MeterAddress:{strAdd}");
                var t = Unpooled.Buffer();
                var sendBytes = heartBeatFrame.ToPduStringInHex().StringToByte();
                t.WriteBytes(sendBytes);
                context.WriteAsync(t);
                _logger.LogInfo($"Send     To  {context.Channel.RemoteAddress}==> {sendBytes.ByteToString(" ")}");

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
                _logger.LogError("Exception: " + exception.Message);
                _mainServerViewModel.RemoveClient(context);
                _logger.LogError("断开: " + context.Channel.RemoteAddress);

                context.CloseAsync();
            });
        }

        public override void ChannelUnregistered(IChannelHandlerContext context)
        {
            _mainServerViewModel.RemoveClient(context);
            _logger.LogInfo("已断开: " + context.Channel.RemoteAddress);

            base.ChannelUnregistered(context);
        }
        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            if (evt is not IdleStateEvent)
            {
                return;
            }

            IdleStateEvent e = evt as IdleStateEvent;
            if (e.State == IdleState.ReaderIdle)
            {
                // The connection was OK but there was no traffic for last period.
                //Console.WriteLine("Disconnecting due to no inbound traffic");
                _logger.LogInfo("由于未主动与服务器通讯而断开连接: " + context.Channel.RemoteAddress);
                context.CloseAsync();
            }

        }
    }
}