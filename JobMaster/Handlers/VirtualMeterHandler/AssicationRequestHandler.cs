using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using JobMaster.Services;
using JobMaster.ViewModels;

namespace JobMaster.Handlers
{
    /// <summary>
    /// 作为模拟数据时使用，但判断为协商请求，响应协商响应报文
    /// </summary>
    public class AssicationRequestHandler : ChannelHandlerAdapter
    {
        private readonly NetLoggerViewModel _logger;
        private readonly IProtocol Protocol;

        public AssicationRequestHandler(NetLoggerViewModel logger, IProtocol protocol)
        {
            _logger = logger;
            Protocol = protocol;
            _logger.LogTrace("AssicationRequestHandler 实例化成功");
        }
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            if (message is byte[] bytes)
            {
                var result = Protocol.TakeReplyApduFromFrame(bytes)
                    .ByteToString();
                if (Protocol.AssociationRequest.PduStringInHexConstructor(ref result))
                {
                    //属于协商请求，则响应AssocitationResponse
                    var response = Protocol.AssociationResponse.ToPduStringInHex();
                    var t = Unpooled.Buffer();
                    t.WriteBytes(response.StringToByte());
                    context.WriteAndFlushAsync(t);

                }
                else
                {
                    //跳转给下一个handler处理
                    context.FireChannelRead(bytes);

                }
            }
        }
    }
}