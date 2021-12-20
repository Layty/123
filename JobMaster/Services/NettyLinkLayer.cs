using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using JobMaster.ViewModels;
using System.Threading.Tasks;

namespace JobMaster.Services
{
    public class NettyLinkLayer : ILinkLayer
    {
        public NettyLinkLayer(IChannelHandlerContext context, NetLoggerViewModel netLoggerViewModel)
        {
            Context = context;
            NetLoggerViewModel = netLoggerViewModel;
        }

        public IChannelHandlerContext Context { get; }
        public NetLoggerViewModel NetLoggerViewModel { get; }

        public async Task<byte[]> SendAsync(string sendHexString)
        {
            return await SendAsync(sendHexString.StringToByte());
        }

        public async Task<byte[]> SendAsync(byte[] sendBytes)
        {
            var t = Unpooled.Buffer();
            t.WriteBytes(sendBytes);
            NetLoggerViewModel.LogInfo($"Send     To  {Context.Channel.RemoteAddress}==> {sendBytes.ByteToString(" ")}");

            await Context.WriteAndFlushAsync(t);
            return null;
        }
    }
}