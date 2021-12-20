using DotNetty.Transport.Channels;
using JobMaster.Services;
using JobMaster.ViewModels;
using System;
using System.Collections.Generic;

namespace JobMaster.Handlers
{
    public class ReleaseResponseHandler : ChannelHandlerAdapter
    {
        private readonly NetLoggerViewModel _logger;

        private readonly IProtocol Protocol;


        public static Dictionary<string, bool> ReleaseSuccessors = new Dictionary<string, bool>();

        public ReleaseResponseHandler(NetLoggerViewModel logger, IProtocol protocol)
        {
            _logger = logger;
            _logger.LogTrace("ReleaseResponseHandler 实例化成功");
            Protocol = protocol;
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            if (message is byte[] bytes)
            {
                var result = Protocol.TakeReplyApduFromFrame(bytes);
                if (AppProtocolFactory.CreateReleaseResponse(result) != null)
                {
                    ReleaseSuccessors[context.Channel.RemoteAddress.ToString()] = true;
                }
                else
                {
                    ReleaseSuccessors[context.Channel.RemoteAddress.ToString()] = false;

                }
            }
        }
    }
}