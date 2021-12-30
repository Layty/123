using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using JobMaster.Services;
using JobMaster.ViewModels;
using MyDlmsStandard.ApplicationLay.Release;

namespace JobMaster.Handlers
{
    public class ReleaseRequestHandler : ChannelHandlerAdapter
    {
        // 00 01 00 01 00 01 00 05 62 03 80 01 00
        private readonly NetLoggerViewModel _logger;
        private readonly IProtocol Protocol;

        public ReleaseRequestHandler(NetLoggerViewModel logger, IProtocol protocol)
        {
            _logger = logger;
            Protocol = protocol;
            _logger.LogTrace("ReleaseRequestHandler 实例化成功");
        }
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            if (message is byte[] bytes)
            {
                var result = Protocol.TakeReplyApduFromFrame(bytes)
                    .ByteToString();
                var release = new ReleaseRequest();
                if (release.PduStringInHexConstructor(ref result))
                {
                    //属于释放请求，则响应释放Response
                    //00 01 00 01 00 01 00 05 63 03 80 01 00
                    string re = "00 01 00 01 00 01 00 05 63 03 80 01 00";
                    var t = Unpooled.Buffer();
                    t.WriteBytes(re.StringToByte());
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