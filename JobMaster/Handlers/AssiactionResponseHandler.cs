using DotNetty.Transport.Channels;
using JobMaster.ViewModels;
using MyDlmsStandard;
using System.Collections.Generic;

namespace JobMaster.Handlers
{
    public class AssiactionResponseHandler : ChannelHandlerAdapter
    {
        private readonly NetLoggerViewModel _logger;

        private readonly DlmsClient dlmsClient;
        public bool IsSuccessed { get; set; }

        public static Dictionary<string, bool> Successors = new();
        public AssiactionResponseHandler(NetLoggerViewModel logger, DlmsClient dlmsClient)
        {
            _logger = logger;
            _logger.MyServerNetLogModel.Log = "AssiactionResponseHandler 实例化成功";
            this.dlmsClient = dlmsClient;
        }
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            if (message is byte[] bytes)
            {
                var result = dlmsClient.Business.Protocol.TakeReplyApduFromFrame(ProtocolInterfaceType.WRAPPER, bytes).ByteToString();
                if (dlmsClient.Business.Protocol.AssociationResponse.PduStringInHexConstructor(ref result))
                {
                    IsSuccessed = true;
                    Successors[context.Channel.RemoteAddress.ToString()] = true;
                }
                else
                {
                    Successors[context.Channel.RemoteAddress.ToString()] = false;
                    context.FireChannelRead(bytes);
                    // throw new Exception("协商失败");
                }
            }



        }
    }
}