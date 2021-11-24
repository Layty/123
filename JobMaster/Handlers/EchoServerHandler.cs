using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using JobMaster.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobMaster.Handlers
{
    /// <summary>
    /// 拼包服务
    /// </summary>
    public class EchoServerHandler : ChannelHandlerAdapter
    {
        private readonly NetLoggerViewModel _logger;

        public EchoServerHandler(NetLoggerViewModel logger)
        {
            _logger = logger;
        }


        public override bool IsSharable => true;


        #region 处理拼帧

        private byte[] _returnBytes;
        private readonly List<byte> _listReturnBytes = new List<byte>();
        private bool _isNeedContinue;
        private int TotalLength { get; set; }
        private int NeedReceiveLength { get; set; }

        #endregion

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            if (message is IByteBuffer buffer)
            {
                if (buffer.HasArray)
                {
                    var bytes = new byte[buffer.ReadableBytes];
                    buffer.ReadBytes(bytes);
                    _logger.LogInfo($"Received From{context.Channel.RemoteAddress} <=={bytes.ByteToString(" ")}");


                    if (!_isNeedContinue)
                    {
                        if (bytes.Length < 8)
                        {
                            _listReturnBytes.AddRange(bytes);
                            _returnBytes = _listReturnBytes.ToArray();

                            _listReturnBytes.Clear();
                        }
                        else
                        {
                            var len = bytes[7] + (bytes[6] << 8);
                            if (len == bytes.Length - 8)
                            {
                                _listReturnBytes.AddRange(bytes);
                                _returnBytes = _listReturnBytes.ToArray();
                                _logger.LogTrace("完整帧交给下一个通道处理");

                                context.FireChannelRead(_returnBytes);

                                _listReturnBytes.Clear();
                                _isNeedContinue = false;
                            }

                            if (len > bytes.Length - 8)
                            {
                                TotalLength = len;
                                NeedReceiveLength = TotalLength - (bytes.Length - 8);
                                _listReturnBytes.AddRange(bytes);
                                _isNeedContinue = true;
                                _logger.LogTrace("需要继续接收,进行拼帧");
                            }
                        }
                    }
                    else
                    {
                        if (bytes.Length < NeedReceiveLength)
                        {
                            NeedReceiveLength -= bytes.Length;
                            _listReturnBytes.AddRange(bytes);
                        }

                        if (bytes.Length >= NeedReceiveLength)
                        {
                            NeedReceiveLength = 0;
                            _isNeedContinue = false;
                            _listReturnBytes.AddRange(bytes);
                            _returnBytes = _listReturnBytes.ToArray();
                            _logger.LogTrace("完整帧交给下一个通道处理");

                            context.FireChannelRead(_returnBytes);
                            _listReturnBytes.Clear();
                        }
                    }
                }
            }
        }


        public override async void ChannelReadComplete(IChannelHandlerContext context) =>
            await Task.Run(() => { context.Flush(); });
    }
}