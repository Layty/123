using DotNetty.Transport.Channels;
using JobMaster.Services;
using JobMaster.ViewModels;
using System.Collections.Generic;

namespace JobMaster.Handlers
{

    public class AssiactionResponseHandler : ChannelHandlerAdapter
    {
        private readonly NetLoggerViewModel _logger;

        private readonly IProtocol Protocol;
        public bool IsSuccessed { get; set; }

        public static Dictionary<string, bool> Successors = new();

        public AssiactionResponseHandler(NetLoggerViewModel logger, IProtocol protocol)
        {
            _logger = logger;
            _logger.LogTrace("AssiactionResponseHandler 实例化成功");
            this.Protocol = protocol;
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            if (message is byte[] bytes)
            {
                var result = Protocol.TakeReplyApduFromFrame(bytes)
                    .ByteToString();
                if (Protocol.AssociationResponse.PduStringInHexConstructor(ref result))
                {
                    IsSuccessed = true;
                    Successors[context.Channel.RemoteAddress.ToString()] = true;
                }
                else
                {
                    Successors[context.Channel.RemoteAddress.ToString()] = false;
                    context.FireChannelRead(bytes);

                }
            }
        }
    }
}