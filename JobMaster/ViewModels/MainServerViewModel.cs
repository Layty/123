using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using JobMaster.Handlers;
using JobMaster.Helpers;
using JobMaster.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace JobMaster.ViewModels
{

    public partial class MainServerViewModel : ObservableObject
    {
        private readonly NetLoggerViewModel NetLoggerViewModel;
        private readonly IProtocol Protocol;

        [ObservableProperty]
        private ObservableCollection<MeterIdMatchSocketNew> _meterIdMatchSockets = new();

        [ObservableProperty]
        [AlsoNotifyChangeFor(nameof(CloseServerCommand))]
        private bool _isServerRunning;

        /// <summary>
        /// 超时时间默认2s
        /// </summary>

        [ObservableProperty]
        private int _responseTimeOut = 2;

        /// <summary>
        /// 是否自动响应47心跳帧
        /// </summary>
        [ObservableProperty]

        private bool _isAutoResponseHeartBeat = true;

        /// <summary>
        /// 心跳帧延时响应时间(ms)
        /// </summary>

        [ObservableProperty]
        private int _heartBeatDelayTime = 1000;

        [ObservableProperty]
        private string _serverIp = "192.168.1.155";


        [ObservableProperty]
        private int _serverPort = 8881;




        public void AddClient(IChannelHandlerContext context)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => { ChannelHandlerContextCollection.Add(context); });
        }

        public void RemoveClient(IChannelHandlerContext context)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                var reslut = MeterIdMatchSockets.FirstOrDefault(t => t.MySocket == context);
                if (reslut != null)
                {
                    MeterIdMatchSockets.Remove(reslut);
                }

                ChannelHandlerContextCollection.Remove(context);
            });
        }

        public string FindMeterIdFromMeterIdMatchSockets(IChannelHandlerContext context)
        {
            //由于context从不同的handler来，所以转换为 集合中的context,根据RemoteAddress进行匹配
            var endPoint = context.Channel.RemoteAddress;
            var chanelInChannelHandlerContextCollection =
                ChannelHandlerContextCollection.FirstOrDefault(t1 =>
                    t1.Channel.RemoteAddress == context.Channel.RemoteAddress);
            var reslut = MeterIdMatchSockets.FirstOrDefault(t => t.MySocket == chanelInChannelHandlerContextCollection);
            if (reslut != null) return reslut.MeterId;
            else return "";
        }

        public MainServerViewModel(NetLoggerViewModel netLoggerViewModel, IProtocol protocol,
            DataNotificationViewModel dataNotificationViewModel)
        {
            NetLoggerViewModel = netLoggerViewModel;
            Protocol = protocol;
            this.dataNotificationViewModel = dataNotificationViewModel;
        }

        private IEventLoopGroup _bossGroup;
        private IChannel _boundChannel;
        private IEventLoopGroup _workerGroup;



        [ObservableProperty]
        private bool _ipDetectResult;


        [ObservableProperty]
        private ObservableCollection<IChannelHandlerContext> _channelHandlerContextCollection =
            new ObservableCollection<IChannelHandlerContext>();


        [ObservableProperty]
        private int _readerIdleTimeMin = 15;
        private readonly DataNotificationViewModel dataNotificationViewModel;


        [ICommand()]
       
        public async Task RunServerAsync()
        {
            NetLoggerViewModel.LogFront("正在开启服务器");
            _bossGroup = new MultithreadEventLoopGroup(1);
            _workerGroup = new MultithreadEventLoopGroup();


            ServerBootstrap bootstrap = new ServerBootstrap();

            bootstrap.Group(_bossGroup, _workerGroup); // 设置主和工作线程组

            bootstrap.Channel<TcpServerSocketChannel>(); // 设置通道模式为TcpSocket
            bootstrap
                //存放已完成三次握手的请求的等待队列的最大长度;
                .Option(ChannelOption.SoBacklog, 1024) // 设置网络IO参数等，这里可以设置很多参数，当然你对网络调优和参数设置非常了解的话，你可以设置，或者就用默认参数吧
                                                       //ByteBuf的分配器(重用缓冲区)大小
                .Option(ChannelOption.Allocator, UnpooledByteBufferAllocator.Default)

                //保持长连接
                .Option(ChannelOption.SoKeepalive, true)
                //取消延迟发送
                .Option(ChannelOption.TcpNodelay, true)
                //端口复用
                .ChildOption(ChannelOption.SoReuseport, true)
                // .Handler(new LoggingHandler("SRV-LSTN", LogLevel.INFO))
                .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    //工作线程连接器 是设置了一个管道，服务端主线程所有接收到的信息都会通过这个管道一层层往下传输
                    //同时所有出栈的消息 也要这个管道的所有处理器进行一步步处理

                    IChannelPipeline pipeline = channel.Pipeline;
                    //IdleStateHandler心跳检测处理器，添加自定义处理Handler类实现userEventTriggered()方法作为超时事件的逻辑处理
                    //IdleStateHandler心跳检测每十秒进行一次读检测，如果十秒内ChannelRead()方法未被调用则触发一次userEventTrigger()方法.
                    //                        pipeline.AddLast(new LoggingHandler("SRV-CONN"));
                    //                        pipeline.AddLast("framing-enc", new LengthFieldPrepender(2));
                    //                        pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));
                    pipeline.AddLast(new IdleStateHandler(_readerIdleTimeMin * 60, 0, 0));
                    //根据47协议定义 指出长度的索引
                    pipeline.AddLast("LengthField", new LengthFieldBasedFrameDecoder(1024, 6, 2));
                    pipeline.AddLast("echo", new EchoServerHandler(NetLoggerViewModel));
                    pipeline.AddLast("HeartBeat", new HeartBeatHandler(NetLoggerViewModel, this));
                    pipeline.AddLast("DataNotification",
                        new DataNotificationHandler(NetLoggerViewModel, this, dataNotificationViewModel));

                    pipeline.AddLast("Assiaction", new AssiactionResponseHandler(NetLoggerViewModel, Protocol));
                    pipeline.AddLast("CaptureObjects",
                        new CaptureObjectsResponseHandler(NetLoggerViewModel, Protocol));
                    pipeline.AddLast("SetResponse",
                       new SetResponseHandler(NetLoggerViewModel, Protocol));
                    pipeline.AddLast("BufferResponse", new BufferResponseHandler(NetLoggerViewModel, Protocol));
                    pipeline.AddLast("ReleaseResponse", new ReleaseResponseHandler(NetLoggerViewModel, Protocol));

                }));

            try
            {
                _boundChannel = await bootstrap.BindAsync(new IPEndPoint(IPAddress.Parse(ServerIp), ServerPort));
                // await bootstrap.BindAsync(8882);
                NetLoggerViewModel.LogFront("成功开启服务器");

                IsServerRunning = true;
            }
            catch (Exception e)
            {
                NetLoggerViewModel.LogError(e.Message);
            }
        }
        [ICommand]
        public async Task CloseServerAsync()
        {
            try
            {
                NetLoggerViewModel.LogDebug("正在关闭服务器,请等待...");
                NetLoggerViewModel.LogFront("正在关闭服务器,请等待...");

                //主动关闭客户端链接,相当于发起socket.disconnect
                foreach (var item in ChannelHandlerContextCollection)
                {
                    await item.CloseAsync();

                }

                await _boundChannel.CloseAsync();
            }

            finally
            {
                await Task.WhenAll(
                                _bossGroup.ShutdownGracefullyAsync(),
                                _workerGroup.ShutdownGracefullyAsync()
                            );
                NetLoggerViewModel.LogFront("成功关闭服务器");
                NetLoggerViewModel.LogDebug("成功关闭服务器");
                IsServerRunning = false;
            }




        }
        [ICommand]
        public void IpDetect(string t)
        {
            IpDetectResult = PingIp(t);
        }

        /// <summary>
        /// <summary>
        /// ping ip,测试能否ping通
        /// </summary>
        /// <param name="strIP">IP地址</param>
        /// <returns></returns>
        private static bool PingIp(string strIP)
        {
            bool bRet = false;
            try
            {
                Ping pingSend = new Ping();
                PingReply reply = pingSend.Send(strIP, 1000);
                if (reply.Status == IPStatus.Success)
                    bRet = true;
            }
            catch (Exception)
            {
                bRet = false;
            }

            return bRet;
        }


        public static List<string> HostIPlList => GetHostIpList();




        /// <summary>
        /// 获取当前计算机的主机IPV4地址
        /// </summary>
        /// <returns></returns>
        public static List<string> GetHostIpList()
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress[] addressList = hostEntry.AddressList;

            List<string> stList = new List<string>();
            if (addressList.Length == 0)
            {
                return stList;
            }

            IPAddress[] array = addressList;
            foreach (IPAddress iPAddress in array)
            {
                if (iPAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    stList.Add(iPAddress.ToString());
                }
            }

            return stList;
        }
    }
}