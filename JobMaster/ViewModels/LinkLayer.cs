using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using MyDlmsStandard;
using MySerialPortMaster;
using System.IO.Ports;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace JobMaster.ViewModels
{
    public interface ILinkLayer
    {
        Task<byte[]> SendAsync(string sendHexString);
        Task<byte[]> SendAsync(byte[] sendBytes);
    }

    public class SerialPortLinkLayer : ILinkLayer
    {
        public SerialPortMaster PortMaster { get; set; }
        public readonly SerialPortConfigCaretaker _caretaker = new SerialPortConfigCaretaker();
        public SerialPortLinkLayer(SerialPortMaster portMaster)
        {
            PortMaster = portMaster;
            InitSerialPortParams(PortMaster);
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="serialPortMaster"></param>
        private void InitSerialPortParams(SerialPortMaster serialPortMaster)
        {
            serialPortMaster.DataBits = 8;
            serialPortMaster.StopBits = StopBits.One;
            serialPortMaster.Parity = Parity.None;
        }
        /// <summary>
        /// 初始化21E的串口实例
        /// </summary>
        public void Init21ESerialPort(int StartBaud)
        {
            PortMaster.BaudRate = StartBaud;
            PortMaster.DataBits = 7;
            PortMaster.StopBits = StopBits.One;
            PortMaster.Parity = Parity.Even;
        }

        /// <summary>
        /// 备份当前串口参数，用于后续恢复
        /// </summary>
        public void BackupPortPara()
        {
            var memento = PortMaster.CreateMySerialPortConfig;
            _caretaker.Dictionary["before"] = memento;
            PortMaster.SerialPortLogger.IsSendDataDisplayFormat16 = false;
            PortMaster.SerialPortLogger.IsReceiveFormat16 = false;
        }

        /// <summary>
        /// 恢复备份的串口参数
        /// </summary>
        public void LoadBackupPortPara()
        {
            PortMaster.LoadSerialPortConfig(_caretaker.Dictionary["before"]);
            PortMaster.SerialPortLogger.IsSendDataDisplayFormat16 = true;
            PortMaster.SerialPortLogger.IsReceiveFormat16 = true;
        }

        public async Task<byte[]> SendAsync(string sendHexString)
        {
            return await SendAsync(sendHexString.StringToByte());
        }

        public async Task<byte[]> SendAsync(byte[] sendBytes)
        {
            return await PortMaster.SendAndReceiveReturnDataAsync(sendBytes);
        }
    }

    public class NetLinkLayer : ILinkLayer
    {
        public TcpServerHelper TcpServerHelper { get; set; }
        public Socket CurrentSocket { get; set; }
        public NetLinkLayer(TcpServerHelper tcpServerHelper, Socket currentSocket)
        {
            TcpServerHelper = tcpServerHelper;
            CurrentSocket = currentSocket;
        }
        public async Task<byte[]> SendAsync(string sendHexString)
        {
            return await SendAsync(sendHexString.StringToByte());
        }

        public async Task<byte[]> SendAsync(byte[] sendBytes)
        {
            return await TcpServerHelper.SendDataToClientAndWaitReceiveDataAsync(CurrentSocket, sendBytes);
        }
    }

    public class NettyLinkLayer : ILinkLayer
    {
        public NettyLinkLayer(IChannelHandlerContext context)
        {
            Context = context;
        }

        public IChannelHandlerContext Context { get; }

        public Task<byte[]> SendAsync(string sendHexString)
        {
            throw new System.NotImplementedException();
        }

        public async Task<byte[]> SendAsync(byte[] sendBytes)
        {
            var t = Unpooled.Buffer();
            t.WriteBytes(sendBytes);
            await Context.WriteAndFlushAsync(t);
            return null;
        }
    }
    /// <summary>
    /// 数据层
    /// </summary>
    public class SendData : IToPduStringInHex
    {
        public SendData(IToPduStringInHex handlerData)
        {
            HandlerHexData = handlerData;
        }

        private IToPduStringInHex HandlerHexData;

        public string ToPduStringInHex()
        {
            return HandlerHexData.ToPduStringInHex();
        }
    }

}