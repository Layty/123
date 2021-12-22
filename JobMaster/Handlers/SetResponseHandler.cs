using DotNetty.Transport.Channels;
using JobMaster.Services;
using JobMaster.ViewModels;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using System.Collections.Generic;

namespace JobMaster.Handlers
{
    public class SetResponseHandler : ChannelHandlerAdapter
    {
        private readonly NetLoggerViewModel _logger;
        private readonly IProtocol Protocol;
        public static Dictionary<string, DataAccessResult> SetResponseBindingSocketNew = new();
        public SetResponseHandler(NetLoggerViewModel logger, IProtocol protocol)
        {
            _logger = logger;
            _logger.LogTrace("CaptureObjectsResponseHandler 实例化成功");

            Protocol = protocol;
        }
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            if (message is byte[] bytes)
            {
                var result = Protocol.TakeReplyApduFromFrame(bytes);

                var CaptureObjectsResponse = AppProtocolFactory.CreateSetResponse(result);
                if (CaptureObjectsResponse?.SetResponseNormal != null)
                {
                    var reslut = CaptureObjectsResponse.SetResponseNormal.Result;
                    if (reslut == DataAccessResult.Success)
                    {
                        _logger.LogTrace("Response成功");
                        SetResponseBindingSocketNew[context.Channel.RemoteAddress.ToString()] =
                            DataAccessResult.Success;
                    }
                    else
                    {
                        //_logger.Log = "读取曲线捕获对象失败\r\n";
                        SetResponseBindingSocketNew[context.Channel.RemoteAddress.ToString()] = reslut;
                        context.FireChannelRead(bytes);
                    }
                }
                else
                {
                    SetResponseBindingSocketNew[context.Channel.RemoteAddress.ToString()] = DataAccessResult.OtherReason;
                    context.FireChannelRead(bytes);
                }
            }
        }
    }
}