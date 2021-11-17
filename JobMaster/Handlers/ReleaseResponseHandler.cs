using DotNetty.Transport.Channels;
using JobMaster.Helpers;
using JobMaster.ViewModels;
using MyDlmsStandard;
using System;
using System.Collections.Generic;

namespace JobMaster.Handlers
{
    public class ReleaseResponseHandler : ChannelHandlerAdapter
    {
        private readonly NetLoggerViewModel _logger;

        private readonly DlmsClient dlmsClient;


        public static Dictionary<string, bool> ReleaseSuccessors = new Dictionary<string, bool>();
        public ReleaseResponseHandler(NetLoggerViewModel logger, DlmsClient dlmsClient)
        {
            _logger = logger;
            _logger.MyServerNetLogModel.Log = "ReleaseResponseHandler 实例化成功";
            this.dlmsClient = dlmsClient;
        }
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            if (message is byte[] bytes)
            {
                var result = dlmsClient.Business.Protocol.TakeReplyApduFromFrame(ProtocolInterfaceType.WRAPPER, bytes);
                if (AppProtocolFactory.CreateReleaseResponse(result) != null)
                {
                    ReleaseSuccessors[context.Channel.RemoteAddress.ToString()] = true;
                }
                else
                {
                    ReleaseSuccessors[context.Channel.RemoteAddress.ToString()] = false;
                    throw new Exception("Release失败");
                }
            }



        }
    }
}