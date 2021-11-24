using DotNetty.Transport.Channels;
using JobMaster.Helpers;
using JobMaster.ViewModels;
using MyDlmsStandard;
using MyDlmsStandard.ApplicationLay;
using MyDlmsStandard.ApplicationLay.ApplicationLayEnums;
using MyDlmsStandard.ApplicationLay.Get;
using System.Collections.Generic;

namespace JobMaster.Handlers
{
    public class CaptureObjectsResponseHandler : ChannelHandlerAdapter
    {
        private readonly NetLoggerViewModel _logger;
        private readonly IProtocol Protocol;
        public static Dictionary<string, GetResponse> CaptureObjectsResponsesBindingSocketNew = new ();

        public CaptureObjectsResponseHandler(NetLoggerViewModel logger, IProtocol protocol)
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

                var CaptureObjectsResponse = AppProtocolFactory.CreateGetResponse(result);
                if (CaptureObjectsResponse != null)
                {
                    if (CaptureObjectsResponse.GetResponseNormal?.Result.Data.Value != null)
                    {
                        if (CaptureObjectsResponse.GetResponseNormal.Result.Data.DataType == DataType.Array)
                        {
                            var CaptureObjectsArray = new DLMSArray();
                            var ar = CaptureObjectsResponse.GetResponseNormal.Result.Data.ToPduStringInHex();
                            if (!CaptureObjectsArray.PduStringInHexConstructor(ref ar))
                            {
                                return;
                            }
                            else if (CaptureObjectsArray.Items.Length != 9)
                            {
                                //内部进一步判断合理性
                                context.FireChannelRead(bytes);
                            }
                            else
                            {
                                _logger.LogTrace("读取曲线捕获对象成功");
                               
                                CaptureObjectsResponsesBindingSocketNew[context.Channel.RemoteAddress.ToString()] =
                                    CaptureObjectsResponse;
                            }
                        }
                    }
                    else
                    {
                        //_logger.Log = "读取曲线捕获对象失败\r\n";
                        CaptureObjectsResponsesBindingSocketNew[context.Channel.RemoteAddress.ToString()] = null;
                        context.FireChannelRead(bytes);
                    }
                }
                else
                {
                    CaptureObjectsResponsesBindingSocketNew[context.Channel.RemoteAddress.ToString()] = null;
                    context.FireChannelRead(bytes);
                }
            }
        }
    }
}