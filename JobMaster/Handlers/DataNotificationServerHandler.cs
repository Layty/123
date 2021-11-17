using DotNetty.Transport.Channels;
using JobMaster.ViewModels;

namespace JobMaster.Handlers
{
    /// <summary>
    /// 主动上报服务
    /// </summary>
    public class DataNotificationServerHandler : ChannelHandlerAdapter
    {
        private readonly NetLoggerViewModel _logger;
        private MainServerViewModel _mainServerViewModel;
        public DataNotificationServerHandler(NetLoggerViewModel logger, MainServerViewModel mainServerViewModel)
        {
            _logger = logger;
            _mainServerViewModel = mainServerViewModel;
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            //主动上报后续再实现
            // base.ChannelRead(context, message);
        }
    }
}