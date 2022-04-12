using CommunityToolkit.Mvvm.ComponentModel;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using JobMaster.Handlers;

using JobMaster.Services;
using MyDlmsStandard.Wrapper;
using Newtonsoft.Json;
using Prism.Commands;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace JobMaster.ViewModels
{
    public partial class EnergyCaptureObjects1 : ObservableObject
    {
        [ObservableProperty]
        private DateTime dateTime = DateTime.Now;
        [JsonProperty("正向有功总")]
        [ObservableProperty]

        private string importActiveEnergyTotal = "0";
        [JsonProperty("正向有功尖")]
        [ObservableProperty]
        private string importActiveEnergyT1 = "0";
        [JsonProperty("正向有功峰")]
        [ObservableProperty]
        private string importActiveEnergyT2 = "0";
        [JsonProperty("正向有功平")]
        [ObservableProperty]
        private string importActiveEnergyT3 = "0";
        [JsonProperty("正向有功谷")]
        [ObservableProperty]
        private string importActiveEnergyT4 = "0";
        [JsonProperty("反向有功总")]
        [ObservableProperty]
        private string exportActiveEnergyTotal = "0";


        [JsonProperty("正向无功总")]
        [ObservableProperty]

        private string importReactiveEnergyTotal = "0";
        [JsonProperty("反向无功总")]
        [ObservableProperty]

        private string exportReactiveEnergyTotal = "0";
    }
    public partial class VirtualMeterClientViewModel : ObservableValidator
    {
        private readonly NetLoggerViewModel netLoggerViewModel;
        private readonly IProtocol protocol;
        [ObservableProperty]
        [Required]
        private string _serverIp = "192.168.1.155";
     
        [ObservableProperty]
        private EnergyCaptureObjects1 _energyCapture = new EnergyCaptureObjects1();

       
        [ObservableProperty]
        private int _serverPort = 8881;

      
        private string _meterId = "000000000001";

        [Required]
        [MinLength(12)]
        [MaxLength(12)]
       
        public string MeterId
        {
            get { return _meterId; }
            set { SetProperty(ref _meterId, value, true); }
        }

        public DelegateCommand ConnectToServer { get; set; }
        public DelegateCommand DisConnectToServer { get; set; }

        public DelegateCommand HeartBeatCommand { get; set; }
      

        private IChannel clientChannel;
        private MultithreadEventLoopGroup group;
        [ObservableProperty]
        private bool _isServerRunning;
        public VirtualMeterClientViewModel(NetLoggerViewModel netLoggerViewModel, IProtocol protocol
            )
        {
            this.netLoggerViewModel = netLoggerViewModel;
            this.protocol = protocol;
            ConnectToServer = new DelegateCommand(async () =>
            {
                netLoggerViewModel.LogFront("正在连接服务器");
                group = new MultithreadEventLoopGroup();
                Bootstrap bootstrap = new Bootstrap();
                bootstrap
                 .Group(group)
                 .Channel<TcpSocketChannel>()
                 .Option(ChannelOption.TcpNodelay, true)
                 .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                 {
                     IChannelPipeline pipeline = channel.Pipeline;
                     pipeline.AddLast(new LoggingHandler());
                     pipeline.AddLast("LengthField", new LengthFieldBasedFrameDecoder(1024, 6, 2));
                     pipeline.AddLast("echo", new EchoServerHandler(netLoggerViewModel));
                     pipeline.AddLast("Assiaction", new AssicationRequestHandler(netLoggerViewModel, protocol));
                     pipeline.AddLast("dayPorfile", new DayProfileDataProviderHandler(netLoggerViewModel, protocol, EnergyCapture));
                     pipeline.AddLast("Release", new ReleaseRequestHandler(netLoggerViewModel, protocol));
                 }));
                try
                {
                    clientChannel = await bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(ServerIp), ServerPort));
                    netLoggerViewModel.LogFront("成功连接服务器");
                    IsServerRunning = true;

                }
                catch (Exception e)
                {
                    netLoggerViewModel.LogError(e.Message);

                }


            });
            DisConnectToServer = new DelegateCommand(async () =>
            {

                netLoggerViewModel.LogFront("正在断开服务器");

                await clientChannel.CloseAsync();
                await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
                IsServerRunning = false;
                netLoggerViewModel.LogFront("成功断开服务器");
            });
            HeartBeatCommand = new DelegateCommand(async () =>
            {
                HeartBeatFrame heartBeatFrame = new HeartBeatFrame();
                heartBeatFrame.SetMeterAddressString(MeterId);
                var sendBytes = heartBeatFrame.ToPduStringInHex().StringToByte();
                var t = Unpooled.Buffer();
                t.WriteBytes(sendBytes);
                await clientChannel.WriteAndFlushAsync(t);
            });
        }
    }
}